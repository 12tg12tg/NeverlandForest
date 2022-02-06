using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnd : State<BattleState>
{
    private BattleManager manager;
    public BattleEnd(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();
        manager.uiLink.progressTrans.SetActive(false);
    }

    public override void Release()
    {
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
