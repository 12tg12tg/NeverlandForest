using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start, Player, Monster, Action, Settlement, End,
}

public class BattleFSM : FSM<BattleState>
{
    public BattleManager manager;

    private void Start()
    {
        AddState(BattleState.Start, new BattleStart(manager));
        AddState(BattleState.Player, new BattlePlayerTurn(manager));
        AddState(BattleState.Monster, new BattleMonsterTurn(manager));
        AddState(BattleState.Action, new BattleAction(manager));
        AddState(BattleState.Settlement, new BattleSettlement(manager));
        AddState(BattleState.End, new BattleEnd(manager));
        SetState(BattleState.End);
    }

    public override void Update()
    {
        base.Update();       
    }
    
}
