using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction : State<BattleState>
{
    private BattleManager manager;
    public BattleAction(BattleManager manager)
    {
        this.manager = manager;
    }

    private PlayerCommand curCommand;
    private MonsterCommand curMonsterCommand;

    public override void Init()
    {
        if (manager.MonsterActionQueue.Count <= 0)
        {
            if (manager.isPlayerFirst)
                FSM.ChangeState(BattleState.Settlement);
            else
                FSM.ChangeState(BattleState.Player);
            return;
        }
        else
        {
            curMonsterCommand = manager.MonsterActionQueue.Dequeue();
            curMonsterCommand.attacker.DoCommand();
        }
    }

    public override void Release()
    {
        manager.monsters.ForEach(n => n.SetActionCommand());
    }

    public override void Update()
    {
        if (curMonsterCommand.attacker.State == MonsterState.Idle)
        {
            if (manager.MonsterActionQueue.Count <= 0)
            {
                if (manager.isPlayerFirst)
                    FSM.ChangeState(BattleState.Settlement);
                else
                    FSM.ChangeState(BattleState.Player);
                return;
            }

            curMonsterCommand = manager.MonsterActionQueue.Dequeue();
            curMonsterCommand.attacker.DoCommand();
        }

    }

    public override void FixedUpdate()
    {
    }

    public override void LateUpdate()
    {
    }
}
