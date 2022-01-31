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

    private float messageTime = 2f;
    private bool isMessageOff;

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();
        isMessageOff = false;
        manager.uiLink.ResetProgress();
        manager.ClearCommand();
        manager.uiLink.PrintMessage("플레이어 턴", messageTime, () =>
        {
            isMessageOff = true;
        });
    }

    public override void Release()
    {
    }

    public override void Update()
    {
        if (!isMessageOff)
            return;

        

    }

    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
