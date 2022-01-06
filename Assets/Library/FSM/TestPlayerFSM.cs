using System;
using UnityEngine;

[Obsolete("Not use anymore.")]
public enum PlayerState
{
    Idle,
    Move,
    Death
}

[Obsolete("Not use anymore.")]
public class TestPlayerFSM : FSM<PlayerState>
{
    private PlayerState curstate;
    private float Timer;
    public void Start()
    {
        AddState(PlayerState.Move, new MoveState());
        AddState(PlayerState.Idle, new IdleState());
        AddState(PlayerState.Death, new DeathState());
        curstate = PlayerState.Idle;
        SetState(curstate);
        
    }
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetState(PlayerState.Idle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetState(PlayerState.Move);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetState(PlayerState.Death);
        }

        Timer += Time.deltaTime;

        if (Timer > 3f)
        {
            curstate = PlayerState.Move;
            ChangeState(curstate);
        }
        if (Timer > 7f)
        {
            curstate = PlayerState.Death;
            ChangeState(curstate);
        }

        if (Timer > 10f)
        {
            Timer = 0f;
            curstate = PlayerState.Idle;
            ChangeState(curstate);
        }
    }
}
