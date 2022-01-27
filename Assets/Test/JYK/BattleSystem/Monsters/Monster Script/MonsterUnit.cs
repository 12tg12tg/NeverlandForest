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
    private int maxSheild;
    private int speed;
    private MonsterType type;
    protected MonsterTableElem baseElem;
    public MonsterCommand command;
    private BattleManager manager;
    public bool isActionDone;
    private float delayTimer;
    private const float actionDelay = 0.5f;
    [Header("반드시 테이블에서의 몬스터 아이디 입력")]
    public string monsterID;
    public List<Obstacle> obstacles = new List<Obstacle>();

    // Property
    public int Sheild { get => sheild; set => sheild = value; }
    public int Speed { get => BaseElem.Speed; } // 랜덤을 최소 < 속도 < 최대 내에서 뽑아주는 프로퍼티임.
    public bool IsBind { get; set; } // 사냥꾼 스킬
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
                State = MonsterState.Idle; // 이 몬스터의 액션 끝을 알리는 조건.
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
        Debug.Log($"{Pos}{name} 몬스터가 {baseElem.Name}에게 {curDamage}의 Hp 피해와 {curSheildDamage}의 실드 피해를 받았다.\n" +
            $"Hp : {Hp + curDamage} -> {Hp} // Sheild : {sheild + curSheildDamage} -> {sheild}");
        uiLinker.UpdateHpBar(Hp, initHp);
        uiLinker.UpdateSheild(sheild, maxSheild);
        DeadCheak();


        // 만약 사냥꾼 이라면 바인드 디버프 추가.
        if (playerCommand.type == PlayerType.Boy)
        {
            // 부가효과
            if (playerCommand.skill.SkillTableElem.name != "근거리")
            {
                if (command.actionType == MonsterActionType.Move)
                {
                    IsBind = true;
                    command.actionType = MonsterActionType.None;
                    uiLinker.SetCantMove();
                }
            }
            if (playerCommand.skill.SkillTableElem.name == "넉 백")
            {
                Tiles backTile = TileMaker.Instance.GetTile(new Vector2(CurTile.index.x, CurTile.index.y + 1));

                if (backTile != null)
                {
                    if (/*올가미가 있으면서 반대쪽 몬스터가 있다면*/ false)
                    {

                    }
                    else
                    {
                        SetMoveUi(true);
                        BattleManager.Instance.MoveUnitOnTile(backTile.index, this, null, () => SetMoveUi(false), false);
                    }
                }
            }
        }
        else
        {
            DisableHitTrigger();
        }
    }

    public void CalcultateDamage(PlayerType type, int damage, out int curDamage, out int curSheildDamage)
    {
        curDamage = 0;
        curSheildDamage = 0;

        if (type == PlayerType.Boy)
        {
            // 데미지계산
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
            uiLinker.Release();
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

        uiLinker.Init(baseElem.type);
        State = MonsterState.Idle;
    }
    private void EraseThis()
    {
        CurTile.RemoveUnit(this);
        manager.monsters.Remove(this);
    }

    private void Release()
    {
        
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
            int rangeTile = (int)BaseElem.type + 1; // 공격 가능한 사거리의 타일
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

        if(moveFoward) // 직선이동할때만 트랩 계산
            ObstacleAdd();

        SetMoveUi(true);
        BattleManager.Instance.MoveUnitOnTile(command.target, this, PlayMoveAnimation,
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

        if (obs.Count == 0) // 올가미나 장벽만 있는 경우는 리턴
            return;

        obs.ForEach(x => totalDamage += x.trapDamage);
        Debug.Log($"현재 Hp:{Hp} - {totalDamage} = {Hp -= totalDamage}");
        //monster[i].Hp -= totalDamage; // 위 디버그 지우면 얘 풀면 됨

        // 장애물 지속턴이 0이하가 됐을 때 없애는 용도
        DurationDecrease(obs);

        // 몬스터 죽었는지 체크
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
