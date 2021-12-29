using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamMove : MonoBehaviour
{
    public MapManagerTest mapManager;
    private Vector3 curRoomPos;
    void Start()
    {
        
    }

    void Update()
    {
        
        var curObj = mapManager.dungeonGen.dungeonRoomObjectList.Find(x => x.roomInfo.Pos.Equals(mapManager.curRoomInfo.Pos));
        if (curObj != null)
        {
            transform.position = new Vector3(curObj.gameObject.transform.position.x, curObj.gameObject.transform.position.y, -10f);
        }
    }
}
