using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start, Player, Monster, Action, End,
}

public class BattleFSM : FSM<BattleState>
{
    public BattleManager manager;
    public void GetManager() { }

    private void Start()
    {
        AddState(BattleState.Action, new BattleAction(manager));
        AddState(BattleState.Player, new BattlePlayerTurn(manager));
        AddState(BattleState.Monster, new BattleMonsterTurn(manager));
        AddState(BattleState.Start, new BattleStart(manager));
        AddState(BattleState.End, new BattleEnd(manager));
        SetState(BattleState.Start);
    }

    public override void Update()
    {
        base.Update();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 50, 100, 30), "playerTurn"))
        {
            ChangeState(BattleState.Player);
        }

        if(GUI.Button(new Rect(0, 90, 100, 30),"monsterTurn"))
        {
            ChangeState(BattleState.Monster);
        }
    }
}
