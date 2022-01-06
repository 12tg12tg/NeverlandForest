using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterBattleState
{
    Idle,
    Death,
    Action,
}

public class MonsterFSM : FSM<MonsterBattleState>
{
    private void Start()
    {
        AddState(MonsterBattleState.Idle, new MonsterIdle());
        AddState(MonsterBattleState.Action, new MonsterAction());
        AddState(MonsterBattleState.Death, new MonsterDeath());

        SetState(MonsterBattleState.Idle);
    }

    public override void Update()
    {
        base.Update();
    }
}
