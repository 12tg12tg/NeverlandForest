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

    public override void Init()
    {
        Debug.Log("Battle Monster Init");
        manager.PrintMessage("몬스터 턴", 1f, null);

        manager.MonsterQueue.Clear();
        //manager.MonsterQueue.Enqueue(manager.monster[0]);
        //manager.MonsterQueue.Enqueue(manager.monster[1]);

        FSM.ChangeState(BattleState.Action);
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
