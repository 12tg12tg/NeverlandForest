using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Idle, Player, Monster, Start, End,
}

public class BattleFSM : FSM<BattleState>
{
    private void Start()
    {
        AddState(BattleState.Idle, new BattleIdle());
        AddState(BattleState.Player, new BattlePlayerTurn());
        AddState(BattleState.Monster, new BattleMonsterTurn());
        AddState(BattleState.Start, new BattleStart());
        AddState(BattleState.End, new BattleEnd());
        SetState(BattleState.Start);
    }

    public override void Update()
    {
        base.Update();

        
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Idle"))
        {
            ChangeState(BattleState.Idle);
        }
        if (GUILayout.Button("Player Turn"))
        {
            ChangeState(BattleState.Player);
        }
        if (GUILayout.Button("Monster Turn"))
        {
            ChangeState(BattleState.Monster);
        }
        if (GUILayout.Button("Battle Start"))
        {
            ChangeState(BattleState.Start);
        }
        if (GUILayout.Button("Battle End"))
        {
            ChangeState(BattleState.End);
        }
    }
}
