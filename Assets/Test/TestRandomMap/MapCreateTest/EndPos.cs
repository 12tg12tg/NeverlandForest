using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isLastPos;

    private MapManagerTest mapMgr;
    private void Start()
    {
        mapMgr = GameObject.FindWithTag("MapManager").GetComponent<MapManagerTest>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("왜안대");
        if(isLastPos)
        {
            mapMgr.changeRoom(true);
        }
        else
        {
            mapMgr.changeRoom(false);
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("왜안대2");
    //    if (isLastPos)
    //    {
    //        mapMgr.changeRoom(true);
    //    }
    //    else
    //    {
    //        mapMgr.changeRoom(false);
    //    }
    //}
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("이건되나");
    //    if (isLastPos)
    //    {
    //        mapMgr.changeRoom(true);
    //    }
    //    else
    //    {
    //        mapMgr.changeRoom(false);
    //    }
    //}
}
