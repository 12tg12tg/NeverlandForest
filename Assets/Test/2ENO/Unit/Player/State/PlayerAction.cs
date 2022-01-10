using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// ���� �� ����
public class PlayerAction : State<CharacterBattleState>
{
    // Component
    private PlayerBattleController playerController;
    private Animator playerAnimation;

    // Vars
    private string attackAnimationName;
    private bool isAttackMotionEnd = false;

    // ����
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
        TileMaker.Instance.GetTile(playerController.curCommand.target).CancleConfirmTarget(); //���ǻ�����
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
            playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
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
                target.PlayHitAnimation();
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