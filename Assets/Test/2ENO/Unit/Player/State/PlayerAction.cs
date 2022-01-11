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
    public bool isAttackMotionEnd; //Animation �̺�Ʈ �Լ� ȣ���� ���⼭ �Ұ�����. ���� �� ������ �ܺο��� �����.
    private bool waitDelay;
    private float afterAttackTimer;
    private const float attackDelay = 1.5f;


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
        playerAnimation.SetTrigger("Attack");
        TileMaker.Instance.GetTile(playerController.curCommand.target).CancleConfirmTarget(playerController.playerType); //���ǻ�����
    }

    public override void Release()
    {
        // ���� �� ����� ���� �ؾ��� �͵�
    }

    public override void Update()
    {
        // ���� �ϴ¼����� ��ٷȴٰ� �����ϸ� ���ʰ� ���� �ִϸ��̼� ����,
        // ���ݾִϸ��̼� ����� ������ �����ٴ� ���� �˷���

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

            // ���Ÿ�� OnAttacked ���� -> �̶�, OnAttacked�� �ð��� �ɸ��� ������ �ʿ��Ұ�� ��ٷȴ� ���� �����ϴ� ��� ���
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
                FSM.ChangeState(CharacterBattleState.Idle); // �� unit�� ���°� �ٲ�� ��Ʋ������ ������Ʈ���� üũ�ϴٰ� ��������
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