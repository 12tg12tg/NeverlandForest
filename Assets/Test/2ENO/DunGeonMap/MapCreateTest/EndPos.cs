using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isLastPos;

    private MapManagerTest mapManager;

    void Start()
    {
        mapManager = GameObject.FindWithTag("MapManager").GetComponent<MapManagerTest>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(isLastPos)
        {
            mapManager.ChangeRoom(true);
        }
        else
        {
            mapManager.ChangeRoom(false);
        }
    }

    //public Vector3 getPosition()
    //{
    //    return transform.position;
    //}
}
