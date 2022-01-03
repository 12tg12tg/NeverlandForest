using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ������
// ���� ������ �Ѿ�� ��쿡�� �� ���� or ��������
public class PlayerIdle : State<CharacterBattleState>, IAttackable
{
    PlayerStats playerStat;
    Animator playerAnimation;
    PlayerBattleUnit playerUnit;

    public void SetPlayerStat(PlayerStats playerStat)
    {
        this.playerStat = playerStat;
    }
    public void SetPlayerAnimation(Animator playerAnimation)
    {
        this.playerAnimation = playerAnimation;
    }
    public void SetPlayerUnit(PlayerBattleUnit unit)
    {
        playerUnit = unit;
    }

    public void OnAttacked(UnitBase attacker)
    {
        // �ǰݴ����� �� �ǰ� �ִϸ��̼� ����
    }
    public override void Init()
    {
        // ������ �������� �� ���°� �ȴ�.
        playerAnimation.SetTrigger("Idle");
        playerUnit.SendAttackEndMessage();
        playerUnit.SendSkillUseMessage();

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
