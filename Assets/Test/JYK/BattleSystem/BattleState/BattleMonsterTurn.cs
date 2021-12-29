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
        
    }

    public override void Release()
    {
        Debug.Log("Battle Monster Release");
    }

    public override void Update()
    {
        Debug.Log("Battle Monster Update");
    }
    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
