using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : State<BattleState>
{
    private BattleManager manager;
    private float startDelay = 2.5f;
    private bool isAnimationDone;
    private bool isTrapSet;

    public bool IsTrapDone { get => isTrapSet; set => isTrapSet = value; } 

    public BattleStart(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        isAnimationDone = false;
        isTrapSet = false;

        // �帧
        //  ������ ���� �� ĳ���� �θ� Ŭ����� ���� �� �η��� �ִϸ��̼� �� ȭ�� ��� �ִϸ��̼� 
        //  �� ���� ������ ī�޶� �̵� �� Ÿ�� ������ �� ���� �غ�ܰ� �޼��� �� ���� ��ġ
        //  �� (��ġ �Ϸ� �Է½�) �� FSM.ChangeState(BattleState.Player);

        //  ���� �κ� [������ ���� �� ... �� Ÿ�� ������] ������ �ڷ�ƾ���� �ѹ��� �ϳ��� �Լ��� ���ʷ� ����.
        //  �װ� ���� �������� [���� �غ�ܰ� �޼���] ���� �Ʒ� �ڵ� ����.

        manager.PrintMessage("���� �غ� �ܰ�", startDelay, () => manager.ActivateTrapSetUI());
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        /*�޼��� ��� �� ���� ����*/
        if (isTrapSet)
        {
            isTrapSet = false;
            FSM.ChangeState(BattleState.Player);
            manager.UpdateWave();
        }
    }

    public override void LateUpdate()
    {
    }

    public override void FixedUpdate()
    {
    }
}
