using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction : State<BattleState>
{
    private BattleManager manager;
    public Queue<PlayerCommand> actionQueue;
    public BattleAction(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        Debug.Log("Battle Action Init");
        actionQueue = manager.comandQueue;
    }

    public override void Release()
    {
        Debug.Log("Battle Action Release");
    }
    public override void Update()
    {
        Debug.Log("Battle Action Update");
    }

    public override void FixedUpdate()
    {
        
    }

    public override void LateUpdate()
    {
        
    }
}
