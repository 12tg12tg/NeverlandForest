using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMonsterTurn : State<BattleState>
{
    private BattleManager manager;
    public BattleMonsterTurn(BattleManager manager)
    {
        this.manager = manager;
    }
    private float messageTime = 3f;
    private bool isMessageOff;

    public override void Init()
    {
        Debug.Log("Battle Monster Init");
        isMessageOff = false;
        manager.PrintMessage("���� ��", messageTime, () => isMessageOff = true);
        manager.TurnChage();
    }

    public override void Release()
    {
        Debug.Log("Battle Monster Release");
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
