using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTool
{
    private DungeonSystem dungeonSystem;

    public DungeonRoom curDungeonInfo;
    public DungeonRoom beforeDungeonInfo;

    //public TextMeshProUGUI text;
    public void init(DungeonData data, DungeonSystem system)
    {
        if (dungeonSystem == null)
            dungeonSystem = system;
    }

    public DungeonRoom GetNextRoom(DungeonRoom curRoom)
    {
        if (curRoom.nextRoomIdx == -1)
            return null;
        var nextRoom = dungeonSystem.DungeonSystemData.dungeonRoomArray[curRoom.nextRoomIdx];
        return nextRoom;
    }

    public DungeonRoom GetBeforeRoom(DungeonRoom curRoom)
    {
        if (curRoom.beforeRoomIdx == -1)
            return null;
        var beforeRoom = dungeonSystem.DungeonSystemData.dungeonRoomArray[curRoom.beforeRoomIdx];
        return beforeRoom;
    }

    //public void SetText(DungeonRoom roomInfo)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    for(int i=0; i < roomInfo.eventList.Count; i++)
    //    {
    //        sb.Append(roomInfo.eventList[i].ToString());
    //        sb.Append("\n");
    //    }
    //    text.SetText(sb.ToString());
    //}
}

// 메인에 연결된 길 방들에 대한 정보를 리스트로 정리해서 넘겨주기 위한
//public void CurOnRoomList(DungeonRoom curRoomInfo)
//{
//    var roomList = new List<DungeonRoom>();
//    // 현재 end된 방이 메인 => 다음방은 서브룸, 반대경우도 마찬가지
//    // 다음방이 서브룸의 경우 리스트를 뽑아서 한번에 이벤트 오브젝트 해야함

//    if(curRoomInfo.RoomType == DunGeonRoomType.MainRoom)
//    {
//        curRoomInfo = GetNextRoom(curRoomInfo);
//        while (curRoomInfo.RoomType != DunGeonRoomType.MainRoom)
//        {
//            roomList.Add(curRoomInfo);
//            curRoomInfo = GetNextRoom(curRoomInfo);
//        }
//    }
//    else
//    {
//        curRoomInfo = GetNextRoom(curRoomInfo);
//        roomList.Add(curRoomInfo);
//    }

//    //dungeonSystem.DungeonSystemData.curIncludeRoomList = roomList;
//}
// + 비교문에 next를 넣어 비교하는순간 추적이나 돌아가는거 파악 직관성이 떨어진다

//public void ChangeRoomForward(bool isEnd)
//{
//    beforeDungeonInfo = curDungeonInfo;
//    if (isEnd)
//    {
//        //curRoomObjectInfo.gameObject.SetActive(false);
//        if (curDungeonInfo.RoomType == DunGeonRoomType.MainRoom)
//        {
//            if(curDungeonInfo.nextRoomIdx == -1)
//            {
//                Vars.UserData.WorldMapPlayerData.isClear = true;
//                SceneManager.LoadScene("AS_WorldMap");
//                return;
//            }
//            //SetCurRoad(curDungeonInfo);
//        }
//        else
//        {
//            //curRoomObjectInfo = mainRoomPrefab;
//        }
//        CurOnRoomList(curDungeonInfo);
//        //curRoomObjectInfo.gameObject.SetActive(true);
//        curDungeonInfo = GetNextRoom(curDungeonInfo);

//        //dungeonSystem.DungeonSystemData.curRoomData = curRoomObjectInfo;
//    }
//    else
//    {
//        curDungeonInfo = GetNextRoom(curDungeonInfo);
//    }

//    SetText(curDungeonInfo);
//    //SetCheckRoom(curDungeonInfo, beforeDungeonInfo);
//    //dungeonSystem.DungeonSystemData.curDungeonData = curDungeonInfo;
//}