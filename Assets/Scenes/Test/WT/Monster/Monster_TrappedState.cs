using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_TrappedState : State<MonsterState>
{
    public override void Init()
    {
        Debug.Log("MonsterTrapped");
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
