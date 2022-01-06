using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Not use anymore.")]
public class IdleState : State<PlayerState>
{
    public override void Init()
    {
        Debug.Log("init state");
        var player = GameObject.FindWithTag("Player");
        var ren = player.gameObject.GetComponent<MeshRenderer>().material;
        ren.color = Color.black;
    }

    public override void Release()
    {
        Debug.Log("init release");
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
