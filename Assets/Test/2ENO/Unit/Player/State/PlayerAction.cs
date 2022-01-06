using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// 공격 전 상태
public class PlayerAction : State<CharacterBattleState>
{
    PlayerBattleController playerController;
    Animator playerAnimation;

    private string attackAnimationName;
    private bool isAttackMotionEnd = false;

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
        if (playerController.playerType == PlayerType.Boy)
        {
            attackAnimationName = "WTD_AttackA1";
        }
        else
        {
            attackAnimationName = "WTD_AttackA3";
        }
        playerAnimation.SetTrigger("Attack");
    }
    public override void Release()
    {
        // 공격 이 종료된 시점 해야할 것들
    }
    public override void Update()
    {
        // 공격 하는순간을 기다렸다가 공격하면 몇초간 공격 애니메이션 실행,
        // 공격애니메이션 종료시 공격이 종료됬다는 것을 알려줌

        if (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName) &&
            playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            isAttackMotionEnd = true;
        }

        if(isAttackMotionEnd)
        {
            var monsterList = GetTargetList(playerController.curCommand.target);
            var targetList = monsterList.Cast<TestMonster>().ToList();
            // 모든타겟 OnAttacked 실행 -> 이때, OnAttacked에 시간이 걸리는 동작이 필요할경우 기다렸다 다음 진행하는 방식 고려
            foreach (var target in targetList)
            {
                var UnitBase = new UnitBase();
                UnitBase.Atk = playerController.curCommand.skill.SkillTableElem.damage;
                target.OnAttacked(UnitBase);
            }

            FSM.ChangeState(CharacterBattleState.Idle); // 이 unit의 상태가 바뀌면 배틀상태의 업데이트에서 체크하다가 다음진행
            return;
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

//private IEnumerator CoAttackAction()
//{
//    //playerAnimation.SetTrigger()

//    // 공격 애니메이션 모션이 종료될때 
//    while (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName("WTD_AttackA3") &&
//        playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
//    {
//        yield return null;
//    }
//    var targetList = GetTargetList(playerUnit.curCommand.target);

//    // 모든타겟 OnAttacked 실행 -> 이때, OnAttacked 역시 코루틴 등의 동작이 필요할경우 기다렸다 콜백으로 다음 진행?
//    foreach(var target in targetList)
//    {

//    }

//    FSM.ChangeState(CharacterBattleState.Idle); // 이 unit의 상태가 바뀌면 배틀의 코루틴에서 체크하다가 역시 종료되서 다음진행
//}