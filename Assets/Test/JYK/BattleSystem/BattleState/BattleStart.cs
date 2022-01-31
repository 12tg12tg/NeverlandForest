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
        BottomUIManager.Instance.progress.SetActive(true);
        manager.uiLink.ResetProgress();
        isReadyDone = false;
        if((int)Vars.UserData.uData.lanternState <= (int)LanternState.Level2)
        {
            // ���� ����
            manager.isPlayerFirst = false;
            manager.uiLink.PrintMessage("���� ����!", startDelay, () => isReadyDone = true);

            
        }
        else
        {
            // �÷��̾� ����
            manager.isPlayerFirst = true;
            manager.uiLink.PrintMessage("���� ���� ���!", startDelay, () => manager.inputLink.WaitUntillSettingDone());

        }
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        /*�޼��� ��� �� ���� ����*/
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

            manager.uiLink.PrintMessage($"{manager.Turn}�� ����", 0.8f,
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
