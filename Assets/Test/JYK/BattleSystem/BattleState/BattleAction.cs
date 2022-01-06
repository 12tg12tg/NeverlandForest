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
    private MonsterCommand curMonster;

    bool onceTry = true;
    float timer = 0;
    public override void Init()
    {
        Debug.Log("Battle Action Init");

        if (FSM.preState == BattleState.Player)
        {
            curCommand = manager.CommandQueue.Dequeue();
            // 커맨드 큐에 있는 명령들을 모두 실행시킨 후에 상태 변경
            curCommand.attacker.TurnInit(curCommand);
        }
        else if (FSM.preState == BattleState.Monster)
        {
            curMonster = manager.MonsterQueue.Dequeue();
        }
    }

    public override void Release()
    {
        Debug.Log("Battle Action Release");
    }
    public override void Update()
    {
        if (FSM.preState == BattleState.Player)
        {
            if (curCommand.attacker.FSM.curState == CharacterBattleState.Idle)
            {
                if (manager.CommandQueue.Count <= 0)
                {
                    FSM.ChangeState(BattleState.Monster);
                    return;
                }

                curCommand = manager.CommandQueue.Dequeue();
                curCommand.attacker.TurnInit(curCommand);

            }
        }
        else if (FSM.preState == BattleState.Monster)
        {
            //    var tile = curMonster.CurTile;
            //    Tiles foward;
            //    if (tile.TryGetFowardTile(out foward, 1))
            //    {
            //        var tiles = TileMaker.Instance.GetTile(foward.index);
            //        if (onceTry)
            //        {

            //            manager.PlaceUnitOnTile(foward.index, curMonster, true);
            //            onceTry = false;
            //        }
            //        if(tiles.transform.position)
            //        {

            //        }
            //    }
            timer += Time.deltaTime;
            if(timer > 3f)
            {
                timer = 0f;
                FSM.ChangeState(BattleState.Player);
                return;
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

//IEnumerator CoAction(Queue<PlayerComand> commandQueue)
//{
//    var command = commandQueue.Dequeue();
//    while (true)
//    {
//        yield return CoCommandExcute(command);
//        if (commandQueue.Count <= 0)
//            break;
//        command = commandQueue.Dequeue();
//    }

//    // 턴이 모두 사용됏다.
//    FSM.ChangeState(BattleState.Monster);
//}

//IEnumerator CoCommandExcute(PlayerComand command)
//{
//    command.attacker.TurnInit(command);
//    while (command.attacker.playerState.curState == CharacterBattleState.Action)
//    {
//        yield return null;
//    }
//}
