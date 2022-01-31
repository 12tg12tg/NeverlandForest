using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public enum MonsterState
{
    Idle, DoSomething, Dead
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
    public int maxSheild;
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
    public List<ObstacleDebuff> obsDebuffs = new List<ObstacleDebuff>();

    // Property
    public int Shield { get => sheild; set => sheild = value; }
    public int Speed { get => BaseElem.Speed; } // ������ �ּ� < �ӵ� < �ִ� ������ �̾��ִ� ������Ƽ��.
    public bool IsBind { get; set; } // ��ɲ� ��ų
    public bool IsBurn { get; set; } // �������� ��ų
    public MonsterState State { get; set; }
    public MonsterType Type { get => type; }
    public MonsterTableElem BaseElem { get => baseElem; }

    // Start & Awake
    private void Awake()
    {
        animator = GetComponent<Animator>();
        uiLinker = GetComponent<MonsterUILinker>();
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

        // Damage
        CalcultateDamage(playerCommand, out int curDamage, out int curSheildDamage);
        Debug.Log($"{Pos}{name} ���Ͱ� {playerCommand.type}���� {curDamage}�� Hp ���ؿ� {curSheildDamage}�� �ǵ� ���ظ� �޾Ҵ�.\n" +
            $"Hp : {Hp + curDamage} -> {Hp} // Sheild : {sheild + curSheildDamage} -> {sheild}");
        uiLinker.UpdateHpBar(Hp);
        uiLinker.UpdateSheild(sheild);
        DeadCheak();

        // ���� ��ɲ� �̶�� ���ε� ����� �߰�.
        if (playerCommand.type == PlayerType.Boy)
        {
            // �ΰ�ȿ��
            if (playerCommand.skill.SkillTableElem.name != "�ٰŸ�")
            {
                if (command.actionType == MonsterActionType.Move)
                {
                    IsBind = true;
                    command.actionType = MonsterActionType.None;
                    uiLinker.SetCantMove();
                }
            }
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
                        SetMoveUi(true);
                        BattleManager.Instance.tileLink.MoveUnitOnTile(backTile.index, this, null, () => SetMoveUi(false), false);
                    }
                }
            }
        }
        // ��������
        else
        {
            if(playerCommand.skill.SkillTableElem.name != "�ٰŸ�")
            {
                IsBurn = true;
            }
            DisableHitTrigger();
        }
    }

    public void CalcultateDamage(PlayerCommand command, out int curDamage, out int curSheildDamage)
    {
        curDamage = 0;
        curSheildDamage = 0;

        var damage = command.skill.SkillTableElem.Damage;

        if (command.type == PlayerType.Boy || command.skill.SkillTableElem.name == "�ٰŸ�")
        {
            // ���������
            curDamage = damage - sheild < 0 ? 0 : damage - sheild;
            Hp -= curDamage;
        }
        else if(command.type == PlayerType.Girl)
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
            uiLinker.Release();
        }      
    }

    // �ʱ�ȭ
    public void Init()
    {
        baseElem = DataTableManager.GetTable<MonsterTable>().GetData<MonsterTableElem>(monsterID);

        initHp = Hp = baseElem.hp;
        Atk = baseElem.atk;
        sheild = baseElem.sheild;
        maxSheild = sheild;
        type = baseElem.type;
        manager ??= BattleManager.Instance;
        command ??= new MonsterCommand(this);

        uiLinker.Init(baseElem);
        State = MonsterState.Idle;
    }
    private void EraseThis()
    {
        CurTile.RemoveUnit(this);
        manager.monsters.Remove(this);
    }

    public void Release()
    {
        uiLinker.Release();
        EraseThis();
        int id = int.Parse(baseElem.id);
        MonsterPool.Instance.ReturnObject((MonsterPoolTag)id, gameObject);
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
        // 4. ������ ��� Ÿ�� ã��
        else
        {
            // ������ �Ÿ� ���ϱ�
            // 1) �ϴ� �ּ� ~ �ִ� �̵��Ÿ� ���� ������ ����.
            // 2) �÷��̾� ������ �Ѿ���� Ȯ��                 -- 4���� Ȯ���ϸ� Ȯ���� �ʿ� ����.
            //      3) �Ѿ�ٸ� �ִ� ��Ÿ� ������ ����.
            // 4) �ִ� ��Ÿ��� �Ѿ���� Ȯ��
            //      4) �Ѿ�ٸ� �ִ� ��Ÿ� ������ ����.
            var curMoveLen = BaseElem.Speed;
            int rangeTile = (int)BaseElem.type + 1; // ���� ������ ��Ÿ��� Ÿ��
            var dest = Pos.y - curMoveLen;
            if (dest < rangeTile)
            {
                curMoveLen = (int)Pos.y - rangeTile;
            }
            command.nextMove = curMoveLen;
            command.actionType = MonsterActionType.Move;
        }
        State = MonsterState.DoSomething;

        uiLinker.UpdateCircleUI(command);
        return command;
    }

    public void DoCommand()
    {
        switch (command.actionType)
        {
            case MonsterActionType.None:
                DoBindEffect();
                uiLinker.DisapearCircleUI(command,() => isActionDone = true);
                break;
            case MonsterActionType.Attack:
                uiLinker.DisapearCircleUI(command, PlayAttackAnimation);
                
                break;
            case MonsterActionType.Move:
                Move();
                break;
        }
    }

    public void DoBindEffect()
    {
        PlayHitAnimation();
    }

    public void Move()
    {
        // �켱 ���� �࿡�� �̵��Ÿ� Ÿ���� ����ִ��� Ȯ��
        var indexX = Pos.x;
        var indexY = Pos.y - command.nextMove;
        var destTile = TileMaker.Instance.GetTile(new Vector2(indexX, indexY));
        bool moveFoward = false;

        if(destTile.CanStand)
        {
            moveFoward = true;

            command.target = destTile.index;
        }
        else
        {
            moveFoward = false;

            // �ٸ� �࿡�� ����ִ��� Ȯ��
            var list = TileMaker.Instance.GetMovableTilesFoward(Pos, command.nextMove, true);
            int count = list.Count;
            if(count != 0)
            {
                var rand = Random.Range(0, count);
                command.target = list[rand].index;
            }
            else
            {
                // UI �����ϴ� ȿ��
                uiLinker.CantGoAnyWhere(() => isActionDone = true);
                
                // �ൿ���� ����.
                return;
            }
        }

        if(moveFoward) // �����̵��Ҷ��� Ʈ�� ���
            ObstacleAdd();

        SetMoveUi(true);
        BattleManager.Instance.tileLink.MoveUnitOnTile(command.target, this, PlayMoveAnimation,
            () => { uiLinker.DisapearCircleUI(command, null); isActionDone = true; PlayIdleAnimation(); SetMoveUi(false); });
    }

    public void SetMoveUi(bool moveUi)
    {
        uiLinker.linkedUi.UpdateUi = moveUi; 
    }

    // Animation
    public void PlayAttackAnimation()
    {
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
    public void EnableHitTrigger()
    {
        trigger.enabled = true;
    }

    public void DisableHitTrigger()
    {
        trigger.enabled = false;
    }

    // Debuff
    public void ObstacleAdd() // �̵��� üũ
    {
        var goalTile = TileMaker.Instance.GetTile(command.target);
        var movableTilesOtherRow = TileMaker.Instance.GetMovablePathTiles(CurTile, goalTile);
        var obstacleList = movableTilesOtherRow.Where(x => x.obstacle != null).ToList();
        
        for (int i = obstacleList.Count - 1; i >= 0; i--)
        {
            var ob = new ObstacleDebuff(obstacleList[i].obstacle, this);
            obsDebuffs.Add(ob);
            obstacleList[i].obstacle = null;
            
            if (ob.elem.obstacleType == TrapTag.BoobyTrap)
            {
                command.target = obstacleList[i].index;
                return;
            }
        }
    }

    public void ObstacleAdd(Vector2 pos) // Wave ���� �� üũ
    {
        var goalTile = TileMaker.Instance.GetTile(pos);

        if (goalTile.obstacle != null)
        {
            var ob = new ObstacleDebuff(goalTile.obstacle, this);
            obsDebuffs.Add(ob);
            goalTile.obstacle = null;
        }
    }

    public void ObstacleHit()
    {
        // �ð���, ����Ʈ��, ����Ʈ�� �� Ʈ������ ��������.
        var totalDamage = 0;
        var debuffs = obsDebuffs.Where(x => x.trapDamage != 0)
                           .Select(x => x)
                           .ToList();

        if (debuffs.Count == 0)
            return;

        debuffs.ForEach(x => totalDamage += x.trapDamage);
        Debug.Log($"Ʈ�� ������ : {totalDamage}");
        Hp -= totalDamage;

        //�ִϸ��̼� ��� & ��ƼŬ ���
        PlayHitAnimation();

        // ��ֹ� �������� 0���ϰ� ���� �� ���ִ� �뵵
        DurationDecrease(debuffs);

        // ���� �׾����� üũ
        uiLinker.UpdateHpBar(Hp);
        DeadCheak();
    }

    private void DurationDecrease(List<ObstacleDebuff> debuffs)
    {
        debuffs.ForEach(x => x.duration -= 1);
        for (int i = 0; i < debuffs.Count; i++)
        {
            if (debuffs[i].duration < 1)
            {
                obsDebuffs.Remove(debuffs[i]);
            }
        }
    }
}
