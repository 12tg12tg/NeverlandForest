using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomInstance : MonoBehaviour
{
    [HideInInspector] public int prefabId;
    [HideInInspector] public bool isMain;
    [HideInInspector] public bool isActive;
    [HideInInspector] public Vector3 endPosVector;
    private void Start()
    {
        var endpos = gameObject.GetComponentInChildren<EndPos>();
        endPosVector = endpos.gameObject.transform.position;
    }
}
