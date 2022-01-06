using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Not use anymore.")]
public class DeathState : State<PlayerState>
{
    public override void Init()
    {
        Debug.Log("death init");
        var player = GameObject.FindWithTag("Player");
        var ren = player.gameObject.GetComponent<MeshRenderer>().material;
        ren.color = Color.blue;
    }
    public override void Release()
    {
        Debug.Log("death Release");
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
