using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleMonsterTurn : State<BattleState>
{
    private BattleManager manager;
    private float timer = 0;
    private bool canEndState;
    private bool noMonster;
    public BattleMonsterTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();
        // 1. 행동할 몬스터가 있는지 확인
        noMonster = false;
        var stageMonster = manager.monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if (stageMonster.Count == 0)
        {
            if (manager.waveLink.IsAllWaveClear())
            {
                manager.uiLink.PrintMessage($"승리!", 2.5f, () =>
                {
                    manager.uiLink.turnSkipTrans.SetActive(false);
                    manager.uiLink.progressTrans.SetActive(false);

                    manager.Win();

                });
            }
            else
            {
                noMonster = true;
                manager.uiLink.PrintMessage("행동할 몬스터 없음!", 1f,
                    () =>
                    {
                        canEndState = true;
                    });
            }
        }
        else
        {
            manager.uiLink.PrintMessage("몬스터 턴", 1f, () => canEndState = true);

            manager.MonsterActionQueue.Clear();

            foreach (var monster in stageMonster)
            {
                var command = monster.command;
                if (command != null)
                    manager.MonsterActionQueue.Enqueue(command);
            }

        }
    }

    public override void Release()
    {
        Debug.Log("Battle Monster Release");
    }

    public override void Update()
    {
        if (canEndState)
        {
            timer += Time.deltaTime;
            if (timer > 1.2f) // 1초에서 1.2초로 늘림. 창 닫기기전에 Settlement에서 창을 다시 호출해버려서 결국 바로 닫기고 코루틴 중단됬었음.
            {
                timer = 0f;
                if (!noMonster)
                {
                    FSM.ChangeState(BattleState.Action);
                }
                else
                {
                    if (manager.isPlayerFirst)
                        FSM.ChangeState(BattleState.Settlement);
                    else
                        FSM.ChangeState(BattleState.Player);
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
