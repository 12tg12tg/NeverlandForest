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
        Debug.Log("�־ȴ�");
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
    //    Debug.Log("�־ȴ�2");
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
    //    Debug.Log("�̰ǵǳ�");
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
