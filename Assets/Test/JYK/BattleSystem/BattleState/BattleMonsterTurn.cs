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
    public BattleMonsterTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        // 1. 행동할 몬스터가 있는지 확인
        var list = manager.monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if (list.Count == 0)
        {
            if(manager.waveLink.IsAllWaveClear())
                manager.uiLink.PrintMessage($"승리!", 2.5f, () => SceneManager.LoadScene("AS_RandomMap"));
            else
            {
                manager.uiLink.PrintMessage("행동할 몬스터 없음!", 1f, () => canEndState = true);
            }
        }
        else
        {
            manager.uiLink.PrintMessage("몬스터 턴", 1f, () => canEndState = true);

            manager.MonsterActionQueue.Clear();

            foreach (var monster in list)
            {
                var command = monster.command;
                if (command != null)
                    manager.MonsterActionQueue.Enqueue(command);
            }

            /*큐 정렬기준 에 따라 순서 정하기 - 조건 확인 후 업데이트*/
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
            if (timer > 1f)
            {
                timer = 0f;
                FSM.ChangeState(BattleState.Action);
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
