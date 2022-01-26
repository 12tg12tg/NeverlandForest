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
        if (FSM.preState == BattleState.Monster)
        {
            curMonsterCommand = manager.MonsterQueue.Dequeue();
            switch (curMonsterCommand.actionType)
            {
                case MonsterActionType.None:
                    curMonsterCommand.attacker.isActionDone = true;
                    break;
                case MonsterActionType.Attack:
                    curMonsterCommand.attacker.PlayAttackAnimation();
                    break;
                case MonsterActionType.Move:
                    curMonsterCommand.attacker.Move();
                    break;
            }
        }
    }

    public override void Release()
    {
    }

    public override void Update()
    {
        if (FSM.preState == BattleState.Monster)
        {
            if(curMonsterCommand.attacker.State == MonsterState.Idle)
            {
                curMonsterCommand.attacker.SetActionCommand();      // 행동 마치자마자 다음 행동 정하기.

                if(manager.MonsterQueue.Count <= 0)
                {
                    if (manager.isPlayerFirst)
                        FSM.ChangeState(BattleState.Settlement);
                    else
                        FSM.ChangeState(BattleState.Player);
                    return;
                }

                curMonsterCommand = manager.MonsterQueue.Dequeue();
                switch (curMonsterCommand.actionType)
                {
                    case MonsterActionType.None:
                        curMonsterCommand.attacker.isActionDone = true;
                        break;
                    case MonsterActionType.Attack:
                        curMonsterCommand.attacker.PlayAttackAnimation();
                        break;
                    case MonsterActionType.Move:
                        curMonsterCommand.attacker.Move();
                        break;
                }
            }
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void LateUpdate()
    {
    }
}
