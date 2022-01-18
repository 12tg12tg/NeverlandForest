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

// ���ο� ����� �� ��鿡 ���� ������ ����Ʈ�� �����ؼ� �Ѱ��ֱ� ����
//public void CurOnRoomList(DungeonRoom curRoomInfo)
//{
//    var roomList = new List<DungeonRoom>();
//    // ���� end�� ���� ���� => �������� �����, �ݴ��쵵 ��������
//    // �������� ������� ��� ����Ʈ�� �̾Ƽ� �ѹ��� �̺�Ʈ ������Ʈ �ؾ���

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
// + �񱳹��� next�� �־� ���ϴ¼��� �����̳� ���ư��°� �ľ� �������� ��������

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