using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ����
public class PlayerDeath : State<CharacterBattleState>
{
    PlayerStats playerStat;
    Animator playerAnimation;

    public override void Init()
    {
        // ĳ���� �ִϸ��̼��� ������ ������ �ٲٰ�
        // ��Ʋ�ý��� � �׾��ٴ� �޽��� ������
    }
    public override void Release()
    {
        // �߿������� �ٽ� ��Ȱ��ų�� ���۵�?
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
