using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Do not call this method.")]
public class Monster_AttackState : State<TestMonsterState>
{
    public override void Init()
    {
        Debug.Log("MonsterAttack");
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
