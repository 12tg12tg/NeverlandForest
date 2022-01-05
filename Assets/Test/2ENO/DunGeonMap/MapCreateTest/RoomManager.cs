using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager
{
    private DungeonSystem dungeonSystem;

    // 방 프리팹
    public RoomCtrl mainRoomPrefab;
    public RoomCtrl roadRoom2Prefab;
    public RoomCtrl roadRoom3Prefab;
    public RoomCtrl roadRoom4Prefab;
    public RoomCtrl roadRoom5Prefab;

    private RoomCtrl curRoomObjectInfo;
    public DungeonRoom curDungeonInfo;
    public DungeonRoom beforeDungeonInfo;

    public DunGeonMapGenerate dungeonGen;
    public TextMeshProUGUI text;

    public GatheringSystem gatheringSystem;

    private int curRoadCount;
    // 던전 정보 = 던전맵 생성기에 의해 만들어진 각 던전맵에 대한 정보 ex 인덱스, 이벤트 타입 등등
    // 방 정보 = 던전정보를 이용할 방 프리팹, 메인부터 서브까지 총 5개

    // 미니맵 표시, 생성된 던전에 맞는 방 프리팹 세팅, 현재 던전 및 방 정보 초기화
    public void init(DungeonData data, DungeonSystem system)
    {
        if (dungeonSystem == null)
            dungeonSystem = system;
        mainRoomPrefab = dungeonSystem.roomPrefab[0];
        roadRoom2Prefab = dungeonSystem.roomPrefab[1];
        roadRoom3Prefab = dungeonSystem.roomPrefab[2];
        roadRoom4Prefab = dungeonSystem.roomPrefab[3];
        roadRoom5Prefab = dungeonSystem.roomPrefab[4];

        if (data.curDungeonData != null)
            curDungeonInfo = data.curDungeonData;
        else
            curDungeonInfo = data.dungeonRoomList[0];

        switch (curDungeonInfo.RoomType)
        {
            case DunGeonRoomType.MainRoom:
                curRoomObjectInfo = mainRoomPrefab;
                break;
            case DunGeonRoomType.SubRoom:
                if (data.curRoadCount == 0)
                    SetCurRoad(curDungeonInfo);
                else
                    SetCurRoad(data.curRoadCount);
                break;
        }
        beforeDungeonInfo = new DungeonRoom
        {
            IsCheck = false
        };

        data.curDungeonData = curDungeonInfo;
        data.curRoomData = curRoomObjectInfo;

        SetCheckRoom(curDungeonInfo, beforeDungeonInfo);
        curRoomObjectInfo.gameObject.SetActive(true);
    }

    public void SetCurRoad(DungeonRoom curRoom)
    {
        switch(curRoom.nextRoadCount)
        {
            case 2:
                curRoomObjectInfo = roadRoom2Prefab;
                curRoadCount = 2;
                break;
            case 3:
                curRoomObjectInfo = roadRoom3Prefab;
                curRoadCount = 3;
                break;
            case 4:
                curRoomObjectInfo = roadRoom4Prefab;
                curRoadCount = 4;
                break;
            case 5:
                curRoomObjectInfo = roadRoom5Prefab;
                curRoadCount = 5;
                break;
        }
        dungeonSystem.DungeonSystemData.curRoadCount = curRoadCount;
    }
    public void SetCurRoad(int curRoadCount)
    {
        switch (curRoadCount)
        {
            case 2:
                curRoomObjectInfo = roadRoom2Prefab;
                break;
            case 3:
                curRoomObjectInfo = roadRoom3Prefab;
                break;
            case 4:
                curRoomObjectInfo = roadRoom4Prefab;
                break;
            case 5:
                curRoomObjectInfo = roadRoom5Prefab;
                break;
        }
    }
    // 미니맵에 현재 있는 방 표시하기 위해
    public void SetCheckRoom(DungeonRoom curRoom, DungeonRoom beforeRoom)
    {
        var obj = dungeonSystem.DungeonSystemData.dungeonRoomObjectList.Find(x => x.roomInfo.Pos.Equals(curRoom.Pos));
        var mesh = obj.gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;

        if (beforeRoom.IsCheck == true)
        {
            var obj2 = dungeonSystem.DungeonSystemData.dungeonRoomObjectList.Find(x => x.roomInfo.Pos.Equals(beforeRoom.Pos));
            var mesh2 = obj2.gameObject.GetComponent<MeshRenderer>();

            mesh2.material.color = (beforeRoom.RoomType == DunGeonRoomType.MainRoom) ?
            new Color(0.962f, 0.174f, 0.068f) : new Color(0.472f, 0.389f, 0.389f);
        }
    }
    public void ChangeRoomForward(bool isEnd)
    {
        beforeDungeonInfo = curDungeonInfo;
        if (isEnd)
        {
            curRoomObjectInfo.gameObject.SetActive(false);
            if (curDungeonInfo.RoomType == DunGeonRoomType.MainRoom)
            {
                if(curDungeonInfo.nextRoomIdx == -1)
                {
                    Vars.UserData.WorldMapPlayerData.isClear = true;
                    SceneManager.LoadScene("WorldMap");
                    return;
                }
                SetCurRoad(curDungeonInfo);
            }
            else
            {
                curRoomObjectInfo = mainRoomPrefab;
            }
            GetRoomInfoList(curDungeonInfo);
            curRoomObjectInfo.gameObject.SetActive(true);
            curDungeonInfo = curDungeonInfo.nextRoom;

            dungeonSystem.DungeonSystemData.curRoomData = curRoomObjectInfo;
        }
        else
        {
            curDungeonInfo = curDungeonInfo.nextRoom;
        }
        
        SetText(curDungeonInfo);
        SetCheckRoom(curDungeonInfo, beforeDungeonInfo);
        dungeonSystem.DungeonSystemData.curDungeonData = curDungeonInfo;
    }
    public void ChangeRoomGoBack()
    {
        beforeDungeonInfo = curDungeonInfo;
        curDungeonInfo = curDungeonInfo.beforeRoom;

        SetText(curDungeonInfo);
        SetCheckRoom(curDungeonInfo, beforeDungeonInfo);
        dungeonSystem.DungeonSystemData.curRoomData = curRoomObjectInfo;
        dungeonSystem.DungeonSystemData.curDungeonData = curDungeonInfo;
    }


    public void SetText(DungeonRoom roomInfo)
    {
        StringBuilder sb = new StringBuilder();
        for(int i=0; i < roomInfo.eventList.Count; i++)
        {
            sb.Append(roomInfo.eventList[i].ToString());
            sb.Append("\n");
        }
        text.SetText(sb.ToString());
    }
    
    // 메인에 연결된 길 방들에 대한 정보를 리스트로 정리해서 넘겨주기 위한
    public List<DungeonRoom> GetRoomInfoList(DungeonRoom curRoomInfo)
    {
        var roomList = new List<DungeonRoom>();
        // 현재 end된 방이 메인 => 다음방은 서브룸, 반대경우도 마찬가지
        // 다음방이 서브룸의 경우 리스트를 뽑아서 한번에 이벤트 오브젝트 해야함

        if(curRoomInfo.RoomType == DunGeonRoomType.MainRoom)
        {
            curRoomInfo = curRoomInfo.nextRoom;
            while (curRoomInfo.RoomType != DunGeonRoomType.MainRoom)
            {
                roomList.Add(curRoomInfo);
                curRoomInfo = curRoomInfo.nextRoom;
            }
        }
        else
        {
            curRoomInfo = curRoomInfo.nextRoom;
            roomList.Add(curRoomInfo);
        }

        dungeonSystem.DungeonSystemData.curIncludeRoomList = roomList;
        return roomList;
    }
    // + 비교문에 next를 넣어 비교하는순간 추적이나 돌아가는거 파악 직관성이 떨어진다
}

// 1회성 초기화 코루틴 기다리기 위해, 나중에 옵저버 패턴 등으로 구현?
//if(dungeonGen.isSet)
//{
//    // 100 은 스타트 id

//    //DungeonStartSetUp();
//    //curDungeonRoom.CreateAllEventObject(GetRoomInfoList(curRoomInfo));

//    //dungeonGen.isSet = false;
//}