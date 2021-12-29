using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : State<BattleState>
{
    private BattleManager manager;
    private float startDelay = 3f;
    private bool isMessageEnd;

    public BattleStart(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        isMessageEnd = false;
        manager.PrintMessage("��������", startDelay, () => isMessageEnd = true);
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        /*�޼��� ��� �� ���� ����*/
        if (isMessageEnd)
        {
            FSM.ChangeState(BattleState.Player);
        }
    }

    public override void LateUpdate()
    {
    }

    public override void FixedUpdate()
    {
    }
}
