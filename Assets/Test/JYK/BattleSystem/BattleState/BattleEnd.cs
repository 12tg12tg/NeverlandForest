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
        BottomUIManager.Instance.progress.SetActive(false);
    }

    public override void Release()
    {
        Debug.Log("Battle End Release");
    }

    public override void Update()
    {
        Debug.Log("Battle End Update");
    }
    public override void FixedUpdate()
    {
    }
    public override void LateUpdate()
    {
    }
}
