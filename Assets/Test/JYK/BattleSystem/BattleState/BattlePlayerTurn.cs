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
        isMessageOff = false;
        manager.PrintMessage("플레이어 턴", messageTime, () => isMessageOff = true);
    }

    public override void Release()
    {

    }

    public override void Update()
    {
        if(isMessageOff)
        {
            //플레이어 클릭 대기

        }
    }




    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
