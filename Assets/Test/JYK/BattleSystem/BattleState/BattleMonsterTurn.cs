using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

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
        // 1. 행동할 몬스터가 있는지 확인
        var list = manager.monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if (list.Count == 0)
        {
            manager.PrintMessage($"승리!", 2.5f, () => SceneManager.LoadScene("AS_RandomMap"));
        }
        else
        {
            manager.PrintMessage("몬스터 턴", 1f, null);

            manager.MonsterQueue.Clear();

            foreach (var monster in list)
            {
                var command = monster.SetActionCommand();
                if (command != null)
                    manager.MonsterQueue.Enqueue(command);
            }

            manager.MonsterQueue.OrderByDescending(x => x.Ordering);
        }
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
