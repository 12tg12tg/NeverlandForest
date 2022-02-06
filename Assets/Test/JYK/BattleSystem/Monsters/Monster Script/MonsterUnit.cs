using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public enum MonsterState
{
    Idle, DoSomething, Dead
}

[System.Flags]
public enum NearMovableTile
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Back = 1 << 2,
    ALL = Up | Down | Back
}

public class MonsterUnit : UnitBase, IAttackable
{
    // Component
    public SkinnedMeshRenderer mesh;
    [SerializeField] private Animator animator;
    public MonsterUILinker uiLinker;
    public MonsterTrigger triggerLinker;

    // Vars
    public int initHp;
    private int sheild;
    public int maxSheild;
    private MonsterType type;
    protected MonsterTableElem baseElem;
    public MonsterCommand command;
    private BattleManager manager;
    public bool isActionDone;
    private float delayTimer;
    private const float actionDelay = 0.5f;
    [Header("�ݵ�� ���̺����� ���� ���̵� ����")]
    public string monsterID;

    [Header("����� Ȯ��")]
    public List<ObstacleDebuff> obsDebuffs = new List<ObstacleDebuff>();
    public bool stepOnBoobyTrap;
    public Obstacle ownBoobyTrap;
    public bool isPause;
    public bool isMove;

    public Coroutine moveCoroutine;
    public UnityAction afterMove;

    // Property
    public int Shield { get => sheild; set => sheild = value; }
    public int Speed { get => BaseElem.Speed; } // ������ �ּ� < �ӵ� < �ִ� ������ �̾��ִ� ������Ƽ��.
    public bool IsBind { get; set; } // ��ɲ� ��ų
    public bool IsBurn { get; set; } // �������� ��ų
    public MonsterState State { get; set; }
    public MonsterType Type { get => type; }
    public MonsterTableElem BaseElem { get => baseElem; }

    private void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update
    private void Update()
    {
        if (isActionDone)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > actionDelay)
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
            if (!manager.costLink.skillIDs_NearAttack.Contains(playerCommand.skill.SkillTableElem.id)) // �ٰŸ��ƴ� ��
            {
                if (command.actionType == MonsterActionType.Move)
                {
                    IsBind = true;
                    command.actionType = MonsterActionType.None;
                    uiLinker.SetCantMove();
                }
            }
            if (playerCommand.skill.SkillTableElem.id == manager.costLink.skillID_knockBackAttack) // �˹�
            {
                PushBack(true);
            }
        }
        // ��������
        else
        {
            if (!manager.costLink.skillIDs_NearAttack.Contains(playerCommand.skill.SkillTableElem.id)) // �ٰŸ��ƴ� ��
            {
                IsBurn = true;
            }
            if (playerCommand.skill.SkillTableElem.id == manager.costLink.skillID_threatEmission) // �����߻�
            {
                if(State != MonsterState.Dead)
                    KickoutAnyWhere();
            }
            triggerLinker.DisableHitTrigger();
        }
    }

    private NearMovableTile FindNearMovableTile()
    {
        TileMaker tm = TileMaker.Instance;
        int x = (int)CurTile.index.x;
        int y = (int)CurTile.index.y;

        NearMovableTile e = NearMovableTile.None;
        var upTile = tm.GetTile(new Vector2(x + 1, y));
        var downTile = tm.GetTile(new Vector2(x - 1, y));
        var backTile = tm.GetTile(new Vector2(x, y + 1));
        if (upTile != null && upTile.CanStand)
        {
            e |= NearMovableTile.Up;
        }
        if (downTile != null && downTile.CanStand)
        {
            e |= NearMovableTile.Down;
        }
        if (backTile != null && backTile.CanStand)
        {
            e |= NearMovableTile.Back;
        }

        return e;
    }

    private void KickoutAnyWhere()
    {
        UnityAction afterKickOut;
        Vector2 destTile;
        int x = (int)CurTile.index.x;
        int y = (int)CurTile.index.y;

        var movable = FindNearMovableTile();
        if ((movable & NearMovableTile.Up) != 0
            && (movable & NearMovableTile.Down) != 0)
        {
            afterKickOut = () => { SetMoveUi(false); };
            var rand = Random.Range(0, 2);
            if (rand == 0)
            {
                // ���� ������
                destTile = new Vector2(x + 1, y);
            }
            else
            {
                // �Ʒ��� ������
                destTile = new Vector2(x - 1, y);
            }
        }
        else if ((movable & NearMovableTile.Up) != 0)
        {
            // ���� ������
            afterKickOut = () => { SetMoveUi(false); };
            destTile = new Vector2(x + 1, y);
        }
        else if ((movable & NearMovableTile.Down) != 0)
        {
            // �Ʒ��� ������
            afterKickOut = () => SetMoveUi(false);
            destTile = new Vector2(x - 1, y);
        }
        else if ((movable & NearMovableTile.Back) != 0)
        {
            // �ڷ� ������
            afterKickOut = () => { SetMoveUi(false); AfterPushBack(false); };
            destTile = new Vector2(x, y + 1);
        }
        else
            return;

        SetMoveUi(true);
        BattleManager.Instance.tileLink.KickOutUnitOnTile(destTile, this, null,
            afterKickOut);
    }

    // ���� ���
    //private bool CanMoveBackTile(int distance, out Tiles backTile)
    //{
    //    backTile = TileMaker.Instance.GetTile(new Vector2(CurTile.index.x, CurTile.index.y + distance));
    //    return backTile != null && backTile.CanStand;
    //}

    //private void PushBack_UseAllSnareStack(bool isOwner)
    //{
    //    var list = (from n in obsDebuffs
    //               where n.elem.obstacleType == TrapTag.Snare && n.anotherUnit != null
    //               select n).ToList();

    //    int count = list.Count();
    //    count = count == 0 ? 1 : count;


    //    for (int i = count; i > 0; i--)
    //    {
    //        if(CanMoveBackTile(i, out Tiles backTile))
    //        {
    //            Debug.Log($"{baseElem.Name}�� {CurTile.index}���� {backTile.index}�� ����.");
    //            SetMoveUi(true);
    //            BattleManager.Instance.tileLink.MoveUnitOnTile(backTile.index, this, null,
    //                () => { SetMoveUi(false); AfterPushBack(isOwner); }, false);
    //            break;
    //        }
    //    }

    //    foreach (var snare in list)
    //    {
    //        obsDebuffs.Remove(snare);
    //    }
    //    uiLinker.UpdateDebuffs(obsDebuffs);
    //}

    //private void FindSnarelinkedUnit(List<MonsterUnit> linker)
    //{
    //    var list = from n in obsDebuffs
    //               where n.elem.obstacleType == TrapTag.Snare && n.anotherUnit != null
    //               select n.anotherUnit;

    //    foreach (var unit in list)
    //    {
    //        if(!linker.Contains(unit))
    //        {
    //            linker.Add(unit);
    //            unit.FindSnarelinkedUnit(linker);
    //        }
    //    }
    //}

    private void PushBack(bool isOwner)
    {
        //������
        {
            //List<MonsterUnit> linkers = new List<MonsterUnit>();
            //linkers.Add(this);

            //FindSnarelinkedUnit(linkers);
            //foreach (var unit in linkers)
            //{
            //    if(unit == this)
            //    {
            //        unit.PushBack_UseAllSnareStack(true);
            //    }
            //    else
            //    {
            //        unit.PushBack_UseAllSnareStack(false);
            //    }
            //}
        }

        //�ϳ��� �Ҹ��ϴ� ���

        UnityAction afterPushBack = () => 
            {
                SetMoveUi(false);
                AfterPushBack(isOwner); 
                if(State == MonsterState.Dead)
                    CurTile.RemoveUnit(this);
            };

        Tiles backTile = TileMaker.Instance.GetTile(new Vector2(CurTile.index.x, CurTile.index.y + 1));
        if (backTile != null && backTile.CanStand)
        {
            SetMoveUi(true);
            BattleManager.Instance.tileLink.MoveUnitOnTile(backTile.index, this, null,
                afterPushBack, false);
        }

        if (isOwner)
        {
            // �ϳ��� �Ҹ�
            var linkedSnares = from n in obsDebuffs
                              where n.elem.obstacleType == TrapTag.Snare && n.anotherUnit != null
                              select n;
            if (linkedSnares.Count() != 0)
            {
                var firstSnare = linkedSnares.First();
                var anotherDebuff = firstSnare.another;
                var anotherUnit = firstSnare.anotherUnit;

                anotherUnit.obsDebuffs.Remove(anotherDebuff);
                anotherUnit.uiLinker.UpdateDebuffs(anotherUnit.obsDebuffs);

                obsDebuffs.Remove(firstSnare);
                uiLinker.UpdateDebuffs(obsDebuffs);

                anotherUnit.PushBack(false);
            }
            // �ּ�ģ �κ��� �ð��� ��� �Ҹ��ϸ� �ѹ��� �и��� �ڵ�
            {
                //var linkedSnares = (from n in obsDebuffs
                //                    where n.elem.obstacleType == TrapTag.Snare && n.anotherUnit != null
                //                    select n).ToList();
                //var anothers = (from n in linkedSnares
                //                select n.anotherUnit).Distinct().ToList();

                //foreach (var mySnareDebuff in linkedSnares) // ������ ���������Ʈ���� �ð��� ����� ����
                //{
                //    mySnareDebuff.anotherUnit.obsDebuffs.Remove(mySnareDebuff.another);
                //    mySnareDebuff.anotherUnit.uiLinker.UpdateDebuffs(mySnareDebuff.anotherUnit.obsDebuffs);
                //    obsDebuffs.Remove(mySnareDebuff);
                //    uiLinker.UpdateDebuffs(obsDebuffs);
                //}

                //foreach (var another in anothers) // �ѹ��� �и����� ���� ��ȸ
                //{
                //    another.PushBack(false);
                //}
            }
        }
    }

    private void AfterPushBack(bool isOwner)
    {
        if(command.actionType == MonsterActionType.Attack)
        {
            var range = (int)type + 1;
            if(CurTile.index.y > range)
            {
                if (isOwner)
                {
                    command.actionType = MonsterActionType.None;
                    uiLinker.SetCantMove();
                }
                else
                {
                    SetActionCommand();
                }
            }
        }
    }

    public void CalcultateDamage(PlayerCommand command, out int curDamage, out int curSheildDamage)
    {
        curDamage = 0;
        curSheildDamage = 0;

        var damage = command.skill.SkillTableElem.Damage;

        if (command.type == PlayerType.Boy || manager.costLink.skillIDs_NearAttack.Contains(command.skill.SkillTableElem.id))
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
            UnlinkSnare();
            uiLinker.Release();
            CurTile.RemoveUnit(this);
        }      
    }

    private void UnlinkSnare()
    {
        int count = obsDebuffs.Count;
        for (int i = 0; i < count; i++)
        {
            if(obsDebuffs[i].elem.obstacleType == TrapTag.Snare)
            {
                if (obsDebuffs[i].anotherUnit != null)
                {
                    var anotherUnit = obsDebuffs[i].anotherUnit;
                    anotherUnit.obsDebuffs.Remove(obsDebuffs[i].another);
                    anotherUnit.uiLinker.UpdateDebuffs(anotherUnit.obsDebuffs);
                }
                else if(obsDebuffs[i].another == null)
                {
                    obsDebuffs[i].oppositeSnare.Release();
                }
            }
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
        obsDebuffs.Clear();

        uiLinker.Init(baseElem);
        State = MonsterState.Idle;
    }
    private void EraseThis()
    {
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
            int rangeTile = (int)type + 1; // ���� ������ ��Ÿ��� Ÿ��
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
        isMove = true;
        stepOnBoobyTrap = false;

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

        if (moveFoward) // �����̵��Ҷ��� Ʈ�� ���
        {
            triggerLinker.EnableMoveTrigger();
            CheckBoobyTrapOnLoad();
            //ObstacleAdd();
        }

        SetMoveUi(true);
        BattleManager.Instance.tileLink.MoveUnitOnTile(command.target, this, PlayMoveAnimation,
            () => { 
                uiLinker.DisapearCircleUI(command, null);
                isActionDone = true;
                PlayIdleAnimation();
                SetMoveUi(false);
                isMove = false;
                if (stepOnBoobyTrap)
                    BoobyTrap();
                if (moveFoward)
                    triggerLinker.DisableMoveTrigger();
            });
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
        // �ڽ��� �����̴ٰ� ���� ��� - �����.
        if (isMove)
        {
            isPause = true;
        }
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

    public void EndHit()
    {
        if (isPause)
        {
            isPause = false;
            PlayMoveAnimation();
        }
    }

    // Debuff
    public void BoobyTrap()
    {
        // �κ�Ʈ�� Release�ϰ�, �´� �ִϸ��̼� ��� �� ����Ʈ ���, ������ ����
        stepOnBoobyTrap = false;
        PlayHitAnimation();
        var damage = ownBoobyTrap.elem.damage;
        Hp -= damage;
        uiLinker.UpdateHpBar(Hp);
        DeadCheak();
        ownBoobyTrap.Release();
    }

    public void CheckBoobyTrapOnLoad()
    {
        // �κ�Ʈ������ ���� ������ ����
        var goalTile = TileMaker.Instance.GetTile(command.target);
        var movableTilesOtherRow = TileMaker.Instance.GetMovablePathTiles(CurTile, goalTile);
        var obstacleList = movableTilesOtherRow.Where(x => x.obstacle != null).ToList();

        for (int i = obstacleList.Count - 1; i >= 0; i--)
        {
            if (obstacleList[i].obstacle.elem.obstacleType == TrapTag.BoobyTrap)
            {
                stepOnBoobyTrap = true;
                ownBoobyTrap = obstacleList[i].obstacle;
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
                uiLinker.UpdateDebuffs(obsDebuffs);
            }
        }
    }
}
