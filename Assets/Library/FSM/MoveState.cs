using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State<PlayerState>
{
    private float speed = 3f;
   
    public override void Init()
    {
        Debug.Log("Move Init");
        var player = GameObject.FindWithTag("Player");
        var ren = player.gameObject.GetComponent<MeshRenderer>().material;
        ren.color = Color.red;
    }

    public override void Release()
    {
        Debug.Log("Move Release");
    }

    public override void Update()
    {

    }
    public override void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    public override void LateUpdate()
    {
    }

}
