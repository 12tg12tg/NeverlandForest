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
        manager.Init();
        manager.PrintMessage("전투시작", startDelay, () => isMessageEnd = true);
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        /*메세지 띄운 후 시작 연출*/
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
