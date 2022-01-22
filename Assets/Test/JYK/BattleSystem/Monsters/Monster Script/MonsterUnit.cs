using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum MonsterState
{
    Idle, Attack, Move, DoSomething, Dead
}

public class MonsterUnit : UnitBase, IAttackable, IAttackReady
{
    // Component
    private Animator animator;
    public Collider trigger;

    // Vars
    private int sheild;
    private int speed;
    private MonsterType type;
    protected MonsterTableElem baseElem;
    public PlayerCommand hitInfo;
    public MonsterCommand command;
    private BattleManager manager;
    private bool isActionDone;
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
        Init(baseElem);
        State = MonsterState.Idle;
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
        var playerStats = attacker as PlayerCommand;
        var damage = playerStats.skill.SkillTableElem.Damage;
        Debug.Log($"{Pos} ���Ͱ� {playerStats.type}���� {damage}�� ���ظ� �޴�. {Hp} -> {Hp - damage}");
        Hp -= damage;
        if (Hp <= 0)
        {
            PlayDeadAnimation();
            State = MonsterState.Dead;
        }
    }

    // �ʱ�ȭ
    public void Init(MonsterTableElem elem)
    {
        Hp = elem.hp;
        Atk = elem.atk;
        sheild = elem.sheild;
        type = elem.type;
        manager ??= BattleManager.Instance;
        command ??= new MonsterCommand(this);
    }
    private void EraseThis()
    {
        CurTile.RemoveUnit(this);
        manager.monster.Remove(this);
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
                Debug.Log((int)(command.target.y - CurTile.index.y));
            }
            else
            {
                // �ٸ� �࿡���� Ȯ��
                var movableTilesOtherRow = TileMaker.Instance.GetMovableTiles(CurTile);
                int count = movableTilesOtherRow.Length;
                if(count != 0)
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

    public void Ready(PlayerCommand command)
    {
        trigger.enabled = true;
        hitInfo = command;
    }

    public void ObstacleAdd()
    {
        var goalTile = TileMaker.Instance.GetTile(command.target);
        var movableTilesOtherRow = TileMaker.Instance.GetMovablePathTiles(CurTile, goalTile);
        var obstacleList = movableTilesOtherRow.Where(x => x.obstacle != null).Select(x => x).ToList();
        
        for (int i = obstacleList.Count - 1; i >= 0; i--)
        {
            var ob = new Obstacle(obstacleList[i].obstacle);
            obstacles.Add(ob);
            //Destroy(obstacleList[i].obstacle.prefab); // TODO : -> �ε����� ��(�ݶ��̴� Ʈ����)�� �����ؾ���
            obstacleList[i].obstacle = null;
            // �κ�Ʈ���� ������ command.target �ش� �κ�Ʈ���� �ִ� ������ ���� �ؾ���
            // �������
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
            //Destroy(goalTile.obstacle.prefab); // TODO : -> �ε����� ��(�ݶ��̴� Ʈ����)�� �����ؾ���
            goalTile.obstacle = null;
        }
    }
}
