using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSettlement : State<BattleState>
{
    private BattleManager manager;
    private float timer;
    readonly float time = 3f;
    public BattleSettlement(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        /*방금 몬스터의 공격을 받았으므로, 플레이어 패배 판단*/

        //몇턴.
        manager.PrintMessage($"{manager.Turn}턴 끝", 0.8f, () =>
        {
            var turn = ++manager.Turn;

            // 웨이브 업데이트 ( 알아서 조건 확인 후웨이브 업데이트 함. )
            manager.UpdateWave();

            manager.PrintMessage($"{manager.Turn}턴 시작", 0.8f, () =>
            {
                if(manager.isPlayerFirst)
                    FSM.ChangeState(BattleState.Player);
                else
                    FSM.ChangeState(BattleState.Monster);
            });
        });

        //실드깍. 디버프 피깍.

        //다시 플레이어턴으로

    }

    public override void Release()
    {

    }

    public override void Update()
    {
        //timer += Time.deltaTime;
        //if(timer > time)
        //{
        //    FSM.ChangeState(BattleState.Player);
        //}
    }
    public override void LateUpdate()
    {
    }
    public override void FixedUpdate()
    {
    }
}
