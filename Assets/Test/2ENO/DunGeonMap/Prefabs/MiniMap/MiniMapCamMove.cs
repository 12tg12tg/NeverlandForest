using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamMove : MonoBehaviour
{
    private DungeonSystem dungeonSystem;
    private Vector3 curRoomPos;
    //void enable()
    //{
    //    dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();
    //}
    private void OnEnable()
    {
        dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();
    }

    void Update()
    {
        
        var curObj = dungeonSystem.DungeonSystemData.dungeonRoomObjectList.Find(x => x.roomInfo.Pos.Equals(dungeonSystem.DungeonSystemData.curDungeonData.Pos));
        if (curObj != null)
        {
            transform.position = new Vector3(curObj.gameObject.transform.position.x, curObj.gameObject.transform.position.y, -10f);
        }
    }
}
