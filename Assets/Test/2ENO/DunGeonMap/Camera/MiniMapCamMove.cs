using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamMove : MonoBehaviour
{
    private Vector3 curRoomPos;

    void Update()
    {
        var curObj = DungeonSystem.Instance.dungeonRoomObjectList.Find(x => x.roomIdx == DungeonSystem.Instance.DungeonSystemData.curDungeonRoomData.roomIdx);
        if (curObj != null)
        {
            transform.position = new Vector3(curObj.gameObject.transform.position.x, curObj.gameObject.transform.position.y, -10f);
        }
    }
}
