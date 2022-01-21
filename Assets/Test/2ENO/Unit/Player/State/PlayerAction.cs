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
    }

    public override void Release()
    {
        // ���� �� ����� ���� �ؾ��� �͵�
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