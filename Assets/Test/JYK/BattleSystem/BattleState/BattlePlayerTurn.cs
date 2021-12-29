using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurn : State<BattleState>
{
    private BattleManager manager;
    public BattlePlayerTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    private float messageTime = 3f;
    private bool isMessageOff;


    public override void Init()
    {
        Debug.Log("Battle Player Init");
        isMessageOff = false;
        manager.PrintMessage("�÷��̾� ��", messageTime, () => isMessageOff = true);
    }

    public override void Release()
    {
        Debug.Log("Battle Player Release");
    }

    public override void Update()
    {
        if(isMessageOff)
        {
            //�÷��̾� Ŭ�� ���

        }
    }




    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
