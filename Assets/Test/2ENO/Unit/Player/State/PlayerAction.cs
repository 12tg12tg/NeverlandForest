using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// 공격 전 상태
public class PlayerAction : State<CharacterBattleState>
{
    // Component
    private PlayerBattleController playerController;
    private Animator playerAnimation;

    // Vars
    public bool isAttackMotionEnd; //Animation 이벤트 함수 호출은 여기서 불가능함. 따라서 이 변수는 외부에서 변경됨.
    private bool waitDelay;
    private float afterAttackTimer;
    private const float attackDelay = 1.5f;


    // 생성
    public PlayerAction(PlayerBattleController controller, Animator playerAnimation)
    {
        this.playerController = controller;
        this.playerAnimation = playerAnimation;
    }

    public override void Init()
    {
        // 액션 상태, 커맨드에 따라 애니메이션, 이펙트 실행,
        // 애니메이션이 끝나면? 타겟들의 OnAttacked 실행
        isAttackMotionEnd = false;
        playerAnimation.SetTrigger("Attack");
        TileMaker.Instance.GetTile(playerController.curCommand.target).CancleConfirmTarget(playerController.playerType); //발판색삭제
    }

    public override void Release()
    {
        // 공격 이 종료된 시점 해야할 것들
    }

    public override void Update()
    {
        // 공격 하는순간을 기다렸다가 공격하면 몇초간 공격 애니메이션 실행,
        // 공격애니메이션 종료시 공격이 종료됬다는 것을 알려줌

        //if (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName) &&
        //    playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
        //{
        //    isAttackMotionEnd = true;
        //}

        if(isAttackMotionEnd)
        {
            isAttackMotionEnd = false;
            var monsterList = GetTargetList(playerController.curCommand.target);
            var targetList = monsterList.Cast<MonsterUnit>();

            // 모든타겟 OnAttacked 실행 -> 이때, OnAttacked에 시간이 걸리는 동작이 필요할경우 기다렸다 다음 진행하는 방식 고려
            foreach (var target in targetList)
            {
                target.PlayHitAnimation();
                target.OnAttacked(playerController.curCommand);
            }
            waitDelay = true;
        }

        if(waitDelay)
        {
            afterAttackTimer += Time.deltaTime;
            if(afterAttackTimer > attackDelay)
            {
                waitDelay = false;
                afterAttackTimer = 0f; 
                FSM.ChangeState(CharacterBattleState.Idle); // 이 unit의 상태가 바뀌면 배틀상태의 업데이트에서 체크하다가 다음진행
            }
        }
    }

    private List<UnitBase> GetTargetList(Vector2 targetPos)
    {
        var tiles = TileMaker.Instance.GetTile(targetPos);
        return tiles.units;
    }

    public override void FixedUpdate()
    {
    }

    public override void LateUpdate()
    {
    }
}