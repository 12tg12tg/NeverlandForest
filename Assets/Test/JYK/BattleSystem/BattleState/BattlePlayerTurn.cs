using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerTurn : State<BattleState>
{
    private BattleManager manager;
    private BottomUIManager bottomUiManager;
    public BattlePlayerTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    private float messageTime = 1.5f;

    public override void Init()
    {
        bottomUiManager ??= BottomUIManager.Instance;
        bottomUiManager.SkillButtonInit();
        bottomUiManager.ItemListInit();
        manager.uiLink.turnSkipTrans.SetActive(true);
        manager.uiLink.ResetProgress();
        manager.ClearCommand();
        manager.uiLink.PrintMessage("플레이어 턴", messageTime, () =>
        {
            bottomUiManager.IsSkillLock = false;
        });
    }

    public override void Release()
    {
        bottomUiManager.IsSkillLock = true;
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
