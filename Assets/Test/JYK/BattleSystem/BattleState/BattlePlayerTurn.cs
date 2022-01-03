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
        manager.ClearCommand();
        manager.PrintMessage("플레이어 턴", messageTime, () =>
        {
            isMessageOff = true;
            manager.playerTurnUI.SetActive(true);
        });
    }

    public override void Release()
    {
        manager.playerTurnUI.SetActive(false);
    }

    public override void Update()
    {
        if (!isMessageOff)
            return;

        //플레이어 클릭 대기

    }

    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
