using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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
        //몇턴.
        manager.uiLink.PrintMessage($"{manager.Turn}턴 끝", 0.8f, () =>
        {
            var turn = ++manager.Turn;

            // 웨이브 업데이트 ( 알아서 조건 확인 후웨이브 업데이트 함. )
            manager.waveLink.UpdateWave();

            manager.uiLink.PrintMessage($"{manager.Turn}턴 시작", 0.8f, () =>
            {
                if(manager.isPlayerFirst)
                    FSM.ChangeState(BattleState.Player);
                else
                    FSM.ChangeState(BattleState.Monster);
            });
        });

        // 약초학자 실드 회복 계산
        manager.monsters.ForEach(n =>
        {
            if (!n.IsBurn)
            {
                n.Shield++;
                if (n.Shield > n.maxSheild)
                    n.Shield = n.maxSheild;
                n.uiLinker.UpdateSheild(n.Shield);
            }
            else
            {
                n.IsBurn = false;
            }
        });

        // 실드깍. 디버프 피깍.
        manager.AllMonsterDebuffCheck();
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
