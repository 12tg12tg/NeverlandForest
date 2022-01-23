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
        playerAnimation.SetTrigger(playerController.command.skill.SkillTableElem.aniTrigger);
    }

    public override void Release()
    {
        // 공격 이 종료된 시점 해야할 것들
    }

    public override void Update()
    {
    }


    public override void FixedUpdate()
    {
    }

    public override void LateUpdate()
    {
    }
}