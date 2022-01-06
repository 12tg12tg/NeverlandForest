using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BattleMonsterTurn : State<BattleState>
{
    private BattleManager manager;
    private float timer = 0;
    public BattleMonsterTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        Debug.Log("Battle Monster Init");
        manager.PrintMessage("몬스터 턴", 1f, null);

        manager.MonsterQueue.Clear();
        

        foreach (var monster in manager.monster)
        {
            var randomTarget = Random.Range(0, 2);
            var targetPos = randomTarget == 0 ? manager.boy.Stats.Pos : manager.girl.Stats.Pos;
            var command = monster.SetActionCommand(targetPos);
            manager.MonsterQueue.Enqueue(command);
        }

        manager.MonsterQueue.OrderByDescending(x => x.Ordering);
    }

    public override void Release()
    {
        Debug.Log("Battle Monster Release");
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if(timer > 2f)
        {
            timer = 0f;
            FSM.ChangeState(BattleState.Action);
        }
    }
    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
