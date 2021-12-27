using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurn : State<BattleState>
{
    public override void Init()
    {
        Debug.Log("Battle Player Init");
    }

    public override void Release()
    {
        Debug.Log("Battle Player Release");
    }

    public override void Update()
    {
        Debug.Log("Battle Player Update");
    }
    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
