using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public enum MonsterState
{
    Idle, Attack, Move, DoSomething, Dead
}

public class MonsterUnit : UnitBase, IAttackable, IAttackReady
{
    // Component
    private Animator animator;
    public Collider trigger;
    public MonsterUILinker uiLinker;

    // Vars
    public int initHp;
    private int sheild;
    private int maxSheild;
    private int speed;
    private MonsterType type;
    protected MonsterTableElem baseElem;
    public MonsterCommand command;
    private BattleManager manager;
    public bool isActionDone;
    private float delayTimer;
    private const float actionDelay = 0.5f;
    [Header("�ݵ�� ���̺����� ���� ���̵� �Է�")]
    public string monsterID;
    public List<Obstacle> obstacles = new List<Obstacle>();

    // Property
    public int Sheild { get => sheild; set => sheild = value; }
    public int Speed { get => BaseElem.Speed; } // ������ �ּ� < �ӵ� < �ִ� ������ �̾��ִ� ������Ƽ��.
    public bool IsBind { get; set; } // ��ɲ� ��ų
    public MonsterState State { get; set; }
    public MonsterType Type { get => type; }
    public MonsterTableElem BaseElem { get => baseElem; }

    // Start & Awake
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        baseElem = DataTableManager.GetTable<MonsterTable>().GetData<MonsterTableElem>(monsterID);
        uiLinker = GetComponent<MonsterUILinker>();
        Init(baseElem);
    }

    // Update
    private void Update()
    {
        if(isActionDone)
        {
            delayTimer += Time.deltaTime;
            if(delayTimer > actionDelay)
            {
                delayTimer = 0f;
                isActionDone = false;
                State = MonsterState.Idle; // �� ������ �׼� ���� �˸��� ����.
            }
        }
    }

    // IAttackable
    public void OnAttacked(BattleCommand attacker)
    {
        var playerCommand = attacker as PlayerCommand;
        var damage = playerCommand.skill.SkillTableElem.Damage;

        // Damage
        CalcultateDamage(playerCommand.type, damage, out int curDamage, out int curSheildDamage);
        Debug.Log($"{Pos}{name} ���Ͱ� {baseElem.Name}���� {curDamage}�� Hp ���ؿ� {curSheildDamage}�� �ǵ� ���ظ� �޾Ҵ�.\n" +
            $"Hp : {Hp + curDamage} -> {Hp} // Sheild : {sheild + curSheildDamage} -> {sheild}");
        uiLinker.UpdateHpBar(Hp, initHp);
        uiLinker.UpdateSheild(sheild, maxSheild);
        DeadCheak();


        // ���� ��ɲ� �̶�� ���ε� ����� �߰�.
        if (playerCommand.type == PlayerType.Boy)
        {
            // �ΰ�ȿ��
            if (playerCommand.skill.SkillTableElem.name != "�ٰŸ�")
                IsBind = true;
            if (playerCommand.skill.SkillTableElem.name == "�� ��")
            {
                Tiles backTile = TileMaker.Instance.GetTile(new Vector2(CurTile.index.x, CurTile.index.y + 1));

                if (backTile != null)
                {
                    if (/*�ð��̰� �����鼭 �ݴ��� ���Ͱ� �ִٸ�*/ false)
                    {

                    }
                    else
                    {
                        BattleManager.Instance.MoveUnitOnTile(backTile.index, this, null, null, false);
                    }
                }
            }
        }
        else
        {
            LockTrigger();
        }
    }

    public void CalcultateDamage(PlayerType type, int damage, out int curDamage, out int curSheildDamage)
    {
        curDamage = 0;
        curSheildDamage = 0;

        if (type == PlayerType.Boy)
        {
            // ���������
            curDamage = damage - sheild < 0 ? 0 : damage - sheild;
            Hp -= curDamage;
        }
        else if(type == PlayerType.Girl)
        {
            sheild -= damage;
            if(sheild < 0)
            {
                curDamage = -sheild;
                sheild = 0;
                Hp -= curDamage;
            }
            curSheildDamage = damage - curDamage;
        }
    }

    public void DeadCheak()
    {
        if (Hp <= 0)
        {
            PlayDeadAnimation();
            State = MonsterState.Dead;
            Destroy(uiLinker.linkedUi?.gameObject);
        }
    }

    // �ʱ�ȭ
    public void Init(MonsterTableElem elem)
    {
        initHp = Hp = elem.hp;
        Atk = elem.atk;
        sheild = elem.sheild;
        maxSheild = sheild;
        type = elem.type;
        manager ??= BattleManager.Instance;
        command ??= new MonsterCommand(this);

        uiLinker.Init(elem.type);
        State = MonsterState.Idle;
    }
    private void EraseThis()
    {
        CurTile.RemoveUnit(this);
        manager.monsters.Remove(this);
    }


    // Action
    private bool CheckCanAttackPlayer()
    {
        // ���� ��Ÿ� ���� �÷��̾ �ִ��� �Ǵ�.
        var range = (int)type + 1;
        var dist = Pos.y;
        return dist <= range;
        // ���߿� �÷��̾ �������ִ��� �ƴ����� Ȯ��.
    }

    public MonsterCommand SetActionCommand()
    {
        // 1. ���� Ŀ�ǵ� �����
        command.Clear();

        // 2. �׾����� ���� Ȯ��
        if (State == MonsterState.Dead)
            return null;

        // 3. ������ ����� ��Ÿ����� �ִ��� Ȯ��. �ִٸ� ���� ��� ����.
        if (CheckCanAttackPlayer())
        {
            command.actionType = MonsterActionType.Attack;
            var randTarget = Random.Range(0, 2);
            command.target = randTarget == 0 ? manager.boy.Stats.Pos : manager.girl.Stats.Pos;
        }
        // 4. �ӹ� �������� Ȯ��
        else if(IsBind)
        {
            PlayHitAnimation(); // + �ӹ� ǥ��
            IsBind = false;
            command.actionType = MonsterActionType.None;         
        }
        // 5. ������ ��� Ÿ�� ã��
        else
        {
            // ���� �࿡�� ���� Ȯ��
            var movableTiles = TileMaker.Instance.GetMovableTilesInSameRow(CurTile);
            int countInRow = movableTiles.Length;
            if (countInRow != 0)
            {
                var rand = Random.Range(0, countInRow);
                command.actionType = MonsterActionType.Move;
                command.target = movableTiles[rand].index;
                uiLinker.UpdateDistance((int)(CurTile.index.y - command.target.y));
            }
            else
            {
                // �����࿡ ������ ��Ÿ����̾ ����.
                if (CurTile.index.y <= 2)
                {
                    command.actionType = MonsterActionType.Attack;
                    var randTarget = Random.Range(0, 2);
                    command.target = randTarget == 0 ? manager.boy.Stats.Pos : manager.girl.Stats.Pos;
                }
                else
                {
                    // ������� �ൿ��������.
                    command.actionType = MonsterActionType.None;
                }


                // �ٸ� �࿡���� Ȯ��
                {
                    var movableTilesOtherRow = TileMaker.Instance.GetMovableTiles(CurTile);
                    int count = movableTilesOtherRow.Length;
                    if (count != 0)
                    {
                        var rand = Random.Range(0, count);
                        command.actionType = MonsterActionType.Move;
                        command.target = movableTilesOtherRow[rand].index;
                    }
                    else
                    {
                        command.actionType = MonsterActionType.None;
                    }
                }


            }
        }
        State = MonsterState.DoSomething;
        return command;
    }

    public void Move()
    {
        ObstacleAdd();
        BattleManager.Instance.MoveUnitOnTile(command.target, this, PlayMoveAnimation,
            () => { isActionDone = true; PlayIdleAnimation(); });
    }

    // Animation
    public void PlayAttackAnimation()
    {
        State = MonsterState.Attack;
        animator.SetTrigger("Attack");
    }
    public void PlayDeadAnimation()
    {
        animator.SetTrigger("Die");
    }
    public void PlayHitAnimation()
    {
        animator.SetTrigger("Damaged");
    }
    public void PlayMoveAnimation()
    {
        animator.SetTrigger("Move");
    }
    public void PlayIdleAnimation()
    {
        animator.SetTrigger("Idle");
    }


    // Animation Tag Function
    public void TargetAttack()
    {
        var list = TileMaker.Instance.GetUnitsOnTile(command.target);
        foreach (var target in list)
        {
            var player = target as PlayerStats;
            player.OnAttacked(command);
        }
        isActionDone = true;
    }
    public void StartSinking()
    {
        EraseThis();
        var dest = transform.position;
        dest.y -= 3f;
        StartCoroutine(Utility.CoTranslate(transform, transform.position, dest, 1f, 
            () => { 
                int id = int.Parse(baseElem.id);
                MonsterPool.Instance.ReturnObject((MonsterPoolTag)id, gameObject);
            }));
    }

    // Trigger Enable
    public void EnableTrigger()
    {
        trigger.enabled = true;
    }

    public void LockTrigger()
    {
        trigger.enabled = false;
    }

    // Debuff
    public void ObstacleAdd()
    {
        var goalTile = TileMaker.Instance.GetTile(command.target);
        var movableTilesOtherRow = TileMaker.Instance.GetMovablePathTiles(CurTile, goalTile);
        var obstacleList = movableTilesOtherRow.Where(x => x.obstacle != null).Select(x => x).ToList();
        
        for (int i = obstacleList.Count - 1; i >= 0; i--)
        {
            var ob = new Obstacle(obstacleList[i].obstacle);
            obstacles.Add(ob);
            obstacleList[i].obstacle = null;
            
            if (ob.type == ObstacleType.BoobyTrap)
            {
                command.target = obstacleList[i].index;
                return;
            }
        }
    }

    public void ObstacleAdd(Vector2 pos)
    {
        var goalTile = TileMaker.Instance.GetTile(pos);

        if (goalTile.obstacle != null)
        {
            var ob = new Obstacle(goalTile.obstacle);
            obstacles.Add(ob);
            goalTile.obstacle = null;
        }
    }

    public void ObstacleHit()
    {
        var totalDamage = 0;
        var obs = obstacles.Where(x => x.trapDamage != 0)
                           .Select(x => x)
                           .ToList();

        if (obs.Count == 0) // �ð��̳� �庮�� �ִ� ���� ����
            return;

        obs.ForEach(x => totalDamage += x.trapDamage);
        Debug.Log($"���� Hp:{Hp} - {totalDamage} = {Hp -= totalDamage}");
        //monster[i].Hp -= totalDamage; // �� ����� ����� �� Ǯ�� ��

        // ��ֹ� �������� 0���ϰ� ���� �� ���ִ� �뵵
        DurationDecrease(obs);

        // ���� �׾����� üũ
        uiLinker.UpdateHpBar(Hp, initHp);
        DeadCheak();
    }

    private void DurationDecrease(List<Obstacle> obs)
    {
        obs.ForEach(x => x.duration -= 1);
        for (int i = 0; i < obs.Count; i++)
        {
            if (obs[i].duration < 1)
            {
                obstacles.Remove(obs[i]);
            }
        }
    }
}
