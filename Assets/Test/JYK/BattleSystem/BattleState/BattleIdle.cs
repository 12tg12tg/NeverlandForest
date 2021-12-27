using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleIdle : State<BattleState>
{

    public override void Init()
    {
        Debug.Log("Battle Idle Init");
    }

    public override void Release()
    {
        Debug.Log("Battle Idle Release");
    }
    public override void Update()
    {
        Debug.Log("Battle Idle Update");
    }

    public override void FixedUpdate()
    {
        
    }

    public override void LateUpdate()
    {
        
    }
}
