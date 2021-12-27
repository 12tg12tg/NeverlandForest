using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : State<BattleState>
{
    private float startDelay = 3f;
    private float timer;

    public override void Init()
    {
        Debug.Log("Battle Start Init");

        timer = 0f;
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        Debug.Log("Battle Start Update");

        timer += Time.deltaTime;
        if(timer > startDelay)
        {
            FSM.ChangeState(BattleState.Idle);
        }
    }

    public override void LateUpdate()
    {
    }

    public override void FixedUpdate()
    {
    }
}
