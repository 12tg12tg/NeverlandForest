using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �� ����
public class PlayerAttackReady : State<CharacterBattleState>
{
    PlayerStats playerStat;
    Animator playerAnimation;

    public void SetPlayerStat(PlayerStats playerStat)
    {
        this.playerStat = playerStat;
    }
    public void SetPlayerAnimation(Animator playerAnimation)
    {
        this.playerAnimation = playerAnimation;
    }
    public override void Init()
    {
        // ���� �غ����, ��ų���� UI�� Ȱ��ȭ�ǰ� ������ ��밡��
    }
    public override void Release()
    {
        // ���� �� ����� ���� �ؾ��� �͵�
    }
    public override void Update()
    {
        // ���� �ϴ¼����� ��ٷȴٰ� �����ϸ� ���ʰ� ���� �ִϸ��̼� ����,
        // ���ݾִϸ��̼� ����� ������ �����ٴ� ���� �˷���
    }





    public override void FixedUpdate()
    {
    }
    public override void LateUpdate()
    {
    }
}
