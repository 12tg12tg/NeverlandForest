using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// ���� �� ����
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
        // �׼� ����, Ŀ�ǵ忡 ���� �ִϸ��̼�, ����Ʈ ����,
        // �ִϸ��̼��� ������? Ÿ�ٵ��� OnAttacked ����
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
        // ���� �� ����� ���� �ؾ��� �͵�
    }
    public override void Update()
    {
        // ���� �ϴ¼����� ��ٷȴٰ� �����ϸ� ���ʰ� ���� �ִϸ��̼� ����,
        // ���ݾִϸ��̼� ����� ������ �����ٴ� ���� �˷���

        if (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName) &&
            playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            isAttackMotionEnd = true;
        }

        if(isAttackMotionEnd)
        {
            var monsterList = GetTargetList(playerController.curCommand.target);
            var targetList = monsterList.Cast<MonsterUnit>();

            // ���Ÿ�� OnAttacked ���� -> �̶�, OnAttacked�� �ð��� �ɸ��� ������ �ʿ��Ұ�� ��ٷȴ� ���� �����ϴ� ��� ���
            foreach (var target in targetList)
            {
                target.OnAttacked(playerController.curCommand);
            }

            FSM.ChangeState(CharacterBattleState.Idle); // �� unit�� ���°� �ٲ�� ��Ʋ������ ������Ʈ���� üũ�ϴٰ� ��������
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

//    // ���� �ִϸ��̼� ����� ����ɶ� 
//    while (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName("WTD_AttackA3") &&
//        playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
//    {
//        yield return null;
//    }
//    var targetList = GetTargetList(playerUnit.curCommand.target);

//    // ���Ÿ�� OnAttacked ���� -> �̶�, OnAttacked ���� �ڷ�ƾ ���� ������ �ʿ��Ұ�� ��ٷȴ� �ݹ����� ���� ����?
//    foreach(var target in targetList)
//    {

//    }

//    FSM.ChangeState(CharacterBattleState.Idle); // �� unit�� ���°� �ٲ�� ��Ʋ�� �ڷ�ƾ���� üũ�ϴٰ� ���� ����Ǽ� ��������
//}