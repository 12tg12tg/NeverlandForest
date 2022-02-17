using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleStart : State<BattleState>
{
    private BattleManager manager;
    private float startDelay = 2.5f;
    
    private bool isReadyDone;

    public bool IsReadyDone { get => isReadyDone; set => isReadyDone = value; } 

    public BattleStart(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();
        BottomUIManager.Instance.ItemButtonInit();
        manager.uiLink.ResetProgress();
        manager.uiLink.ShowLanternRange();

        isReadyDone = false;
        if((int)Vars.UserData.uData.lanternState <= (int)LanternState.Level2 || BattleManager.initState == BattleInitState.Bluemoon)
        {
            // 몬스터 선공
            manager.uiLink.progressTrans.SetActive(true);
            manager.isPlayerFirst = false;
            manager.uiLink.PrintMessage("몬스터 습격!", startDelay, () => isReadyDone = true);

            
        }
        else
        {
            // 플레이어 선공
            manager.isPlayerFirst = true;
            manager.uiLink.PrintMessage("몬스터 습격 대비!", startDelay, () => manager.inputLink.WaitUntillSettingDone());

        }
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        /*메세지 띄운 후 시작 연출*/
        if (isReadyDone)
        {
            isReadyDone = false;

            UnityAction action = null;

            if (manager.isPlayerFirst)
            {
                action = () => FSM.ChangeState(BattleState.Player);
            }
            else
            {
                action = () => FSM.ChangeState(BattleState.Monster);
            }

            manager.uiLink.PrintMessage($"{manager.Turn}턴 시작", 0.8f,
                () => { manager.waveLink.UpdateWave(); manager.AllMonsterDebuffCheck(action); }); 
        }
    }

    public override void LateUpdate()
    {
    }

    public override void FixedUpdate()
    {
    }
}
