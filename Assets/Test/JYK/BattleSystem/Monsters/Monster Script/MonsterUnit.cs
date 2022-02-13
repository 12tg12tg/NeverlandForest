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
    [Header("반드시 테이블에서의 몬스터 아이디 설정")]
    public string monsterID;

    [Header("디버프 확인")]
    public List<ObstacleDebuff> obsDebuffs = new List<ObstacleDebuff>();
    public bool stepOnBoobyTrap;
    public Obstacle ownBoobyTrap;
    public bool isPause;
    public bool isMove;

    public Coroutine moveCoroutine;
    public UnityAction afterMove;

    // Property
    public int Shield { get => sheild; set => sheild = value; }
    public int Speed { get => BaseElem.Speed; } // 랜덤을 최소 < 속도 < 최대 내에서 뽑아주는 프로퍼티임.
    public bool IsBind { get; set; } // 사냥꾼 스킬
    public bool IsBurn { get; set; } // 약초학자 스킬
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
                State = MonsterState.Idle; // 이 몬스터의 액션 끝을 알리는 조건.
            }
        }
    }

    // IAttackable
    public void OnAttacked(BattleCommand attacker)
    {
        var playerCommand = attacker as PlayerCommand;

        // Damage
        CalcultateDamage(playerCommand, out int curDamage, out int curSheildDamage);
        Debug.Log($"{Pos}{name} 몬스터가 {playerCommand.type}에게 {curDamage}의 Hp 피해와 {curSheildDamage}의 실드 피해를 받았다.\n" +
            $"Hp : {Hp + curDamage} -> {Hp} // Sheild : {sheild + curSheildDamage} -> {sheild}");
        uiLinker.UpdateHpBar(Hp);
        uiLinker.UpdateSheild(sheild);
        DeathCheck();

        // 만약 사냥꾼 이라면 바인드 디버프 추가.
        if (playerCommand.type == PlayerType.Boy)
        {
            // 부가효과
            if (!manager.costLink.skillIDs_NearAttack.Contains(playerCommand.skill.SkillTableElem.id)) // 근거리아닐 때
            {
                if (command.actionType == MonsterActionType.Move)
                {
                    IsBind = true;
                    command.actionType = MonsterActionType.None;
                    uiLinker.SetCantMove();
                }
            }
            if (playerCommand.skill.SkillTableElem.id == manager.costLink.skillID_knockBackAttack) // 넉백
            {
                PushBack(true);
            }
        }
        // 약초학자
        else
        {
            if (!manager.costLink.skillIDs_NearAttack.Contains(playerCommand.skill.SkillTableElem.id)) // 근거리아닐 때
            {
                IsBurn = true;
            }
            if (playerCommand.skill.SkillTableElem.id == manager.costLink.skillID_threatEmission) // 위협발산
            {
                if(State != MonsterState.Dead)
                    KickoutAnyWhere();
            }
            triggerLinker.DisableHitTrigger();
            BattleManager.Instance.waveLink.SetAllMonsterInfoColliderEnable(true);
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
                // 위로 보내기
                destTile = new Vector2(x + 1, y);
            }
            else
            {
                // 아래로 보내기
                destTile = new Vector2(x - 1, y);
            }
        }
        else if ((movable & NearMovableTile.Up) != 0)
        {
            // 위로 보내기
            afterKickOut = () => { SetMoveUi(false); };
            destTile = new Vector2(x + 1, y);
        }
        else if ((movable & NearMovableTile.Down) != 0)
        {
            // 아래로 보내기
            afterKickOut = () => SetMoveUi(false);
            destTile = new Vector2(x - 1, y);
        }
        else if ((movable & NearMovableTile.Back) != 0)
        {
            // 뒤로 보내기
            afterKickOut = () => { SetMoveUi(false); AfterPushBack(false); };
            destTile = new Vector2(x, y + 1);
        }
        else
            return;

        SetMoveUi(true);
        BattleManager.Instance.tileLink.KickOutUnitOnTile(destTile, this, null,
            afterKickOut);
    }

    // 스택 방식
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
    //            Debug.Log($"{baseElem.Name}가 {CurTile.index}에서 {backTile.index}로 간다.");
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
        //스택형
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

        //하나씩 소모하는 방식

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
            // 하나만 소모
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
            // 주석친 부분은 올가미 모두 소모하며 한번만 밀리는 코드
            {
                //var linkedSnares = (from n in obsDebuffs
                //                    where n.elem.obstacleType == TrapTag.Snare && n.anotherUnit != null
                //                    select n).ToList();
                //var anothers = (from n in linkedSnares
                //                select n.anotherUnit).Distinct().ToList();

                //foreach (var mySnareDebuff in linkedSnares) // 서로의 디버프리스트에서 올가미 디버프 제거
                //{
                //    mySnareDebuff.anotherUnit.obsDebuffs.Remove(mySnareDebuff.another);
                //    mySnareDebuff.anotherUnit.uiLinker.UpdateDebuffs(mySnareDebuff.anotherUnit.obsDebuffs);
                //    obsDebuffs.Remove(mySnareDebuff);
                //    uiLinker.UpdateDebuffs(obsDebuffs);
                //}

                //foreach (var another in anothers) // 한번만 밀리도록 따로 순회
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
        var damageUiPos = uiLinker.linkedUi.rt.localPosition;

        curDamage = 0;
        curSheildDamage = 0;

        var damage = command.skill.SkillTableElem.Damage;

        if (command.type == PlayerType.Boy || manager.costLink.skillIDs_NearAttack.Contains(command.skill.SkillTableElem.id))
        {
            // 데미지계산
            curDamage = damage - sheild < 0 ? 0 : damage - sheild;
            Hp -= curDamage;

            // 데미지 UI 
            var damageUI = UIPool.Instance.GetObject(UIPoolTag.DamageTxt);
            damageUI.transform.SetParent(BattleManager.Instance.damageUiParent);
            var script = damageUI.GetComponent<DamageUI>();
            script.Init(damageUiPos, curDamage, DamageUI.DamageType.Hp);

        }
        else if(command.type == PlayerType.Girl)
        {
            sheild -= damage;

            if (sheild < 0)
            {
                curDamage = -sheild;
                sheild = 0;
                Hp -= curDamage;
            }
            curSheildDamage = damage - curDamage;

            // 데미지 UI 
            if (curSheildDamage != 0)
            {
                var damageUI = UIPool.Instance.GetObject(UIPoolTag.DamageTxt);
                damageUI.transform.SetParent(BattleManager.Instance.damageUiParent);
                var script = damageUI.GetComponent<DamageUI>();
                script.Init(damageUiPos, curSheildDamage, DamageUI.DamageType.Sheild);
            }

            if(curDamage != 0) // HP도 깎은 경우
            {
                var damageUI = UIPool.Instance.GetObject(UIPoolTag.DamageTxt);
                damageUI.transform.SetParent(BattleManager.Instance.damageUiParent);
                var script = damageUI.GetComponent<DamageUI>();
                script.Init(damageUiPos, curDamage, DamageUI.DamageType.Hp);
            }
        }
    }

    public void DeathCheck()
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

    // 초기화
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

        uiLinker.Init(this);
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
        State = MonsterState.Dead;
        UnlinkSnare();
        CurTile.RemoveUnit(this);
        int id = int.Parse(baseElem.id);
        MonsterPool.Instance.ReturnObject((MonsterPoolTag)id, gameObject);
    }

    // Action
    private bool CheckCanAttackPlayer()
    {
        // 몬스터 사거리 내에 플레이어가 있는지 판단.
        var range = (int)type + 1;
        var dist = Pos.y;
        return dist <= range;
        // 나중에 플레이어가 쓰러져있는지 아닌지도 확인.
    }

    public MonsterCommand SetActionCommand()
    {
        // 0. 튜토리얼
        if(manager.isTutorial)
        {
            if (CheckCanAttackPlayer())
            {
                command.actionType = MonsterActionType.Attack;
                var randTarget = Random.Range(0, 2);
                command.target = randTarget == 0 ? manager.boy.Stats.Pos : manager.girl.Stats.Pos;
            }
            else
            {
                int curMoveLen;
                if (BaseElem.id == "0")
                    curMoveLen = 2;
                else
                    curMoveLen = 1;

                int rangeTile = (int)type + 1; // 공격 가능한 사거리의 타일
                var dest = Pos.y - curMoveLen;
                if (dest < rangeTile)
                    curMoveLen = (int)Pos.y - rangeTile;
                command.nextMove = curMoveLen;
                command.actionType = MonsterActionType.Move;
            }
            State = MonsterState.DoSomething;
            uiLinker.UpdateCircleUI(command);
            return command;
        }

        // 1. 이전 커맨드 지우기
        command.Clear();

        // 2. 죽었는지 먼저 확인
        if (State == MonsterState.Dead)
            return null;

        // 3. 공격할 대상이 사거리내에 있는지 확인. 있다면 랜덤 대상 지정.
        if (CheckCanAttackPlayer())
        {
            command.actionType = MonsterActionType.Attack;
            var randTarget = Random.Range(0, 2);
            command.target = randTarget == 0 ? manager.boy.Stats.Pos : manager.girl.Stats.Pos;
        }
        // 4. 움직일 대상 타일 찾기
        else
        {
            // 움직일 거리 정하기
            // 1) 일단 최소 ~ 최대 이동거리 사잇 값으로 설정.
            // 2) 플레이어 영역을 넘어서는지 확인                 -- 4번을 확인하면 확인할 필요 없음.
            //      3) 넘어선다면 최대 사거리 까지로 정정.
            // 4) 최대 사거리를 넘어서는지 확인
            //      4) 넘어선다면 최대 사거리 까지로 정정.
            var curMoveLen = BaseElem.Speed;
            int rangeTile = (int)type + 1; // 공격 가능한 사거리의 타일
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

        // 우선 같은 행에서 이동거리 타일이 비어있는지 확인
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

            // 다른 행에서 비어있는지 확인
            var list = TileMaker.Instance.GetMovableTilesFoward(Pos, command.nextMove, true);
            int count = list.Count;
            if(count != 0)
            {
                var rand = Random.Range(0, count);
                command.target = list[rand].index;
            }
            else
            {
                // UI 증발하는 효과
                uiLinker.CantGoAnyWhere(() => isActionDone = true);
                
                // 행동하지 않음.
                return;
            }
        }

        if (moveFoward) // 직선이동할때만 트랩 계산
        {
            BattleManager.Instance.waveLink.SetAllMonsterInfoColliderEnable(false);
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
                {
                    triggerLinker.DisableMoveTrigger();
                    BattleManager.Instance.waveLink.SetAllMonsterInfoColliderEnable(true);
                }
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
        SoundManager.Instance.Play(baseElem.soundType);
    }
    public void PlayDeadAnimation()
    {
        animator.SetTrigger("Die");
    }
    public void PlayHitAnimation()
    {
        // 자신이 움직이다가 맞은 경우 - 멈춘다.
        if (isMove)
        {
            isPause = true;
        }
        animator.SetTrigger("Damaged");
        SoundManager.Instance?.Play(SoundType.Se_Monster_hitted);
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
        // 부비트랩 Release하고, 맞는 애니메이션 재생 및 이펙트 재생, 데미지 감소
        stepOnBoobyTrap = false;
        PlayHitAnimation();
        var damage = ownBoobyTrap.elem.damage;
        Hp -= damage;
        uiLinker.UpdateHpBar(Hp);
        DeathCheck();
        ownBoobyTrap.Release();
    }

    public void CheckBoobyTrapOnLoad()
    {
        // 부비트랩으로 인한 목적지 수정
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

    public void ObstacleAdd(Vector2 pos) // Wave 진입 시 체크
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
        // 올가미, 나무트랩, 가시트랩 중 트랩류만 가져오기.
        var totalDamage = 0;
        var debuffs = obsDebuffs.Where(x => x.trapDamage != 0)
                           .Select(x => x)
                           .ToList();

        if (debuffs.Count == 0)
            return;

        debuffs.ForEach(x => totalDamage += x.trapDamage);
        Debug.Log($"트랩 데미지 : {totalDamage}");
        Hp -= totalDamage;

        //애니메이션 재생 & 파티클 재생
        PlayHitAnimation();

        // 장애물 지속턴이 0이하가 됐을 때 없애는 용도
        DurationDecrease(debuffs);

        // 몬스터 죽었는지 체크
        uiLinker.UpdateHpBar(Hp);
        DeathCheck();
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
