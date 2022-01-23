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
        //Debug.Log($"{Pos}{name} 몬스터가 {type}에게 hp : {curDamage} 의 피해를 받다. {Hp} -> {Hp - decrease}");

        // 만약 사냥꾼 이라면 바인드 디버프 추가.
        if (playerCommand.type == PlayerType.Boy)
        {
            // 데미지계산
            Debug.Log($"{Pos} 몬스터가 {playerCommand.type}에게 {damage}의 피해를 받다. {Hp} -> {Hp - damage}");
            Hp -= damage;
            if (Hp <= 0)
            {
                PlayDeadAnimation();
                State = MonsterState.Dead;
            }


            // 부가효과
            if (playerCommand.skill.SkillTableElem.name != "근거리")
                IsBind = true;
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

        // DeathCheck
        if (Hp <= 0)
        {
            PlayDeadAnimation();
            State = MonsterState.Dead;
        }
    }

    // 초기화
    public void Init(MonsterTableElem elem)
    {
        Hp = elem.hp;
        Atk = elem.atk;
        sheild = elem.sheild;
        maxSheild = sheild;
        type = elem.type;
        manager ??= BattleManager.Instance;
        command ??= new MonsterCommand(this);
    }
    private void EraseThis()
    {
        CurTile.RemoveUnit(this);
        manager.monsters.Remove(this);
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
        // 4. 속박 상태인지 확인
        else if(IsBind)
        {
            PlayHitAnimation(); // + 속박 표시
            IsBind = false;
            command.actionType = MonsterActionType.None;         
        }
        // 5. 움직일 대상 타일 찾기
        else
        {
            // 같은 행에서 먼저 확인
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
                // 다른 행에서도 확인
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
            //Destroy(obstacleList[i].obstacle.prefab); // TODO : -> 부딪혔을 때(콜라이더 트리거)로 변경해야함
            obstacleList[i].obstacle = null;
            // 부비트랩이 있으면 command.target 해당 부비트랩이 있는 곳으로 변경 해야함
            // 멈춰야함
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
            //Destroy(goalTile.obstacle.prefab); // TODO : -> 부딪혔을 때(콜라이더 트리거)로 변경해야함
            goalTile.obstacle = null;
        }
    }
}
