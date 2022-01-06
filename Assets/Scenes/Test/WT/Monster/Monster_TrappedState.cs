using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Not use anymore.")]
public class Monster_TrappedState : State<TestMonsterState>
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
