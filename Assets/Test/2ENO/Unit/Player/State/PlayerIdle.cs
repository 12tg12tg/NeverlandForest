using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ������
// ���� ������ �Ѿ�� ��쿡�� �� ���� or ��������
public class PlayerIdle : State<CharacterBattleState>, IAttackable
{
    PlayerBattleController playerUnit;
    Animator playerAnimation;

    public PlayerIdle(PlayerBattleController unit, Animator playerAnimation)
    {
        this.playerUnit = unit;
        this.playerAnimation = playerAnimation;
    }
    public void OnAttacked(BattleCommand attacker)
    {
        // �ǰݴ����� �� �ǰ� �ִϸ��̼� ����
    }
    public override void Init()
    {
        // ������ �������� �� ���°� �ȴ�.
        playerAnimation.SetTrigger("Idle");
    }
    public override void Release()
    {
        // ���� �Ѱܹ޾��� �� �����غ�� ���º�ȭ or �ǰ��߿� �׾��� ��쵵 �� �Լ� ����
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
