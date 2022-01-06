using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// �ۻ�� ����
public class DungeonSystem : MonoBehaviour
{
    public RoomCtrl[] roomPrefab;
    public EventObject eventObjectPrefab;
    public GatheringObject gatheringObjPrefab;
    public TextMeshProUGUI text;

    public PlayerDungeonUnit dungeonPlayer;
    public GatheringSystem gatheringSystem;

    //DunGeonMapGenerate dungeonGenerator;
    RoomManager roomManager;

    public List<GameObject> eventObjInstanceList = new List<GameObject>();

    private DungeonRoom beforeDungeonRoom;

    // ���� ����, �ҷ����⿡ �ʿ��� ��� �����͸� �̰����� ����!
    private DungeonData dungeonSystemData = new ();
    public DungeonData DungeonSystemData
    {
        get => dungeonSystemData;
    }
    // ������ �����⿡�� �Űܿ;� �Ǵ� ��ɵ�
    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public GameObject mapPos;
    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();
    public WorldMap worldMap;

    public void OnGUI()
    {
        if(GUILayout.Button("Clear"))
        {
            Vars.UserData.WorldMapPlayerData.isClear = true;
            SceneManager.LoadScene("WorldMap");
        }
        if(GUILayout.Button("Run"))
        {
            Vars.UserData.WorldMapPlayerData.isClear = false;
            SceneManager.LoadScene("WorldMap");
        }
    }

    void Start()
    {
        dungeonPlayer.gameObject.SetActive(false);
        for (int i = 0; i < roomPrefab.Length; i++)
        {
            roomPrefab[i].gameObject.SetActive(false);
        }
        if (Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex] != null)
        {
            dungeonSystemData = Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex];
        }
        else
        {
            Debug.Log("���� ����! Ȯ�ο��");
            return;
        }

        roomManager = new RoomManager();
        roomManager.text = text;

        dungeonSystemData.dungeonRoomArray = Vars.UserData.DungeonMapData;
        dungeonSystemData.dungeonStartIdx = Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].dungeonStartIdx;
        RoomSetMethod();
        worldMap.InitWorldMiniMap();
    }
    // �������� �ϼ��� �Ŀ� ������ ���� �� ����, �ݹ� �޼ҵ�� ����
    private void RoomSetMethod()
    {
        CreateMapObject();
        dungeonPlayer.gameObject.SetActive(true);
        roomManager.init(DungeonSystemData, this);
        if(dungeonSystemData.curDungeonData != null)
        {
            RoomPrefabSet(dungeonSystemData.curDungeonData);
        }
        else
        {
            RoomPrefabSet(dungeonSystemData.dungeonRoomArray[dungeonSystemData.dungeonStartIdx]);
            dungeonSystemData.curDungeonData = dungeonSystemData.dungeonRoomArray[dungeonSystemData.dungeonStartIdx];
        }

        EventObjectCreate(dungeonSystemData.curDungeonData);
        SetCheckRoom(dungeonSystemData.curDungeonData, beforeDungeonRoom);


        if (dungeonSystemData.curPlayerData == null)
        {
            dungeonPlayer.transform.position = roomPrefab[0].spawnPos.transform.position;
        }
        else
        {
            dungeonPlayer.SetPlayerData(dungeonSystemData.curPlayerData);
        }
    }

    // �渶�� ��ġ���ִ� Ʈ���� �ߵ��Ҷ� ����
    public void ChangeRoomEvent(bool isRoomEnd, bool isGoForward)
    {        
        //ConsumeManager.TimeUp(1,0);
        if (isRoomEnd)
        {
            foreach(var obj in eventObjInstanceList)
            {
                Destroy(obj);
            }
            eventObjInstanceList.Clear();

            if(dungeonSystemData.curDungeonData.nextRoomIdx == -1)
            {
                Vars.UserData.WorldMapPlayerData.isClear = true;
                SceneManager.LoadScene("WorldMap");
                Vars.UserData.curDungeonIndex = Vector2.zero;
                Vars.UserData.CurAllDungeonData.Clear();
                Vars.UserData.curLevelDungeonMaps.Clear();
                return;
            }

            beforeDungeonRoom = dungeonSystemData.curDungeonData;
            dungeonSystemData.curDungeonData = roomManager.GetNextRoom(dungeonSystemData.curDungeonData);

            RoomPrefabSet(dungeonSystemData.curDungeonData);
            EventObjectCreate(dungeonSystemData.curDungeonData);
            dungeonPlayer.transform.position = dungeonSystemData.curRoomData.spawnPos.transform.position;


            SetCheckRoom(dungeonSystemData.curDungeonData, beforeDungeonRoom);
            //roomManager.ChangeRoomForward(isRoomEnd);
        }
        else
        {
            if(isGoForward)
            {
                //roomManager.ChangeRoomForward(isRoomEnd);
                beforeDungeonRoom = dungeonSystemData.curDungeonData;
                dungeonSystemData.curDungeonData = roomManager.GetNextRoom(dungeonSystemData.curDungeonData);
                SetCheckRoom(dungeonSystemData.curDungeonData, beforeDungeonRoom);
            }
            else
            {
                //roomManager.ChangeRoomGoBack();
                beforeDungeonRoom = dungeonSystemData.curDungeonData;
                dungeonSystemData.curDungeonData = roomManager.GetBeforeRoom(dungeonSystemData.curDungeonData);
                SetCheckRoom(dungeonSystemData.curDungeonData, beforeDungeonRoom);
            }
        }

        Vars.UserData.DungeonMapData = dungeonSystemData.dungeonRoomArray;
        Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
    }
    // �̺�Ʈ ������Ʈ Ŭ����, ����
    public void EventObjectClickEvent(DunGeonEvent eventType, EventObject eventObject)
    {
        dungeonSystemData.curPlayerData.SetUnitData(dungeonPlayer);

        switch (eventType)
        {
            case DunGeonEvent.Empty:
                break;
            case DunGeonEvent.Battle:
                break;
            case DunGeonEvent.Gathering:
                gatheringSystem.GoGatheringObject(eventObject.gameObject.transform.position);
                break;
            case DunGeonEvent.Hunt:
                Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
                SceneManager.LoadScene("Hunting");
                break;
            case DunGeonEvent.RandomIncount:
                break;
            case DunGeonEvent.SubStory:
                break;
            case DunGeonEvent.Count:
                break;
        }
    }

    public void RoomPrefabSet(DungeonRoom roomData)
    {
        for (int i = 0; i < roomPrefab.Length; i++)
        {
            roomPrefab[i].gameObject.SetActive(false);
        }
        if (roomData.RoomType == DunGeonRoomType.MainRoom)
        {
            // ���ι� ������ ����, �� ���� �� �̺�Ʈ �����͵鿡 basePos ����
            roomPrefab[0].gameObject.SetActive(true);
            foreach(var obj in roomData.eventObjDataList)
            {
                obj.eventBasePos = roomPrefab[0].objPosList[0];
            }
            dungeonSystemData.curRoomData = roomPrefab[0];
        }
        else
        {
            // 2�� �����պ��� 5�� �����ձ��� ��ȸ
            for (int i = 1; i < roomPrefab.Length; i++)
            {
                // ���� ���� ��� ���� �������� ��Ī
                if(roomData.roadCount - 1 == i)
                {
                    roomPrefab[i].gameObject.SetActive(true);
                    DungeonRoom curRoom = roomData;
                    // �� ����� ������Ʈ �������� �ش� �� �����Ϳ� ��´�
                    for (int j = 0; j < roomPrefab[i].objPosList.Count; j++)
                    {
                        foreach(var obj in curRoom.eventObjDataList)
                        {
                            obj.eventBasePos = roomPrefab[i].objPosList[j];
                        }
                        curRoom = roomManager.GetNextRoom(curRoom);
                    }
                    dungeonSystemData.curRoomData = roomPrefab[i];
                }
            }
        }
    }

    // ���� �� ������ ������ ��� ���� �����ϰ� ����� �ϴ� ����
    //public void RoomCreate(DungeonRoom roomData)
    //{
    //    if(roomData.RoomType == DunGeonRoomType.MainRoom)
    //    {
    //        // ���ι� ������ ����
    //    }
    //    else
    //    {
    //        if (dungeonSystemData.dungeonRoomArray[roomData.beforeRoomIdx].RoomType == DunGeonRoomType.SubRoom)
    //            return;
    //        // ��� ������ ���� -> ����� ��� ��
    //        //int curIdx = roomData.roomIdx;
    //        //while (dungeonSystemData.dungeonRoomArray[curIdx])
    //    }
    //}

    // ���� �� ������ ������ �̺�Ʈ ������Ʈ ����
    public void EventObjectCreate(DungeonRoom roomData)
    {
        // ����� ������Ʈ�� ����
        if (roomData.RoomType == DunGeonRoomType.SubRoom)
        {
            while (roomData.RoomType != DunGeonRoomType.MainRoom)
            {
                foreach (var eventObj in roomData.eventObjDataList)
                {
                    {
                        switch (eventObj.eventType)
                        {
                            case DunGeonEvent.Battle:
                                var createBt = eventObj as BattleData;
                                var obj = createBt.CreateObj(eventObjectPrefab, this);
                                eventObjInstanceList.Add(obj.gameObject);
                                break;
                            case DunGeonEvent.Gathering:
                                var createGt = eventObj as GatheringData;
                                var obj2 = createGt.Createobj(gatheringObjPrefab, gatheringSystem, this);
                                eventObjInstanceList.Add(obj2.gameObject);
                                break;
                            case DunGeonEvent.Hunt:
                                var createHt = eventObj as HuntingData;
                                var obj3 = createHt.Createobj(eventObjectPrefab, this);
                                eventObjInstanceList.Add(obj3.gameObject);
                                break;
                            case DunGeonEvent.RandomIncount:
                                break;
                            case DunGeonEvent.SubStory:
                                break;
                        }
                    }
                }
                roomData = roomManager.GetNextRoom(roomData);
            }
        }
        else
        {
            foreach(var eventObj in roomData.eventObjDataList)
            {
                {
                    switch (eventObj.eventType)
                    {
                        case DunGeonEvent.Battle:
                            var createBt = eventObj as BattleData;
                            var obj = createBt.CreateObj(eventObjectPrefab, this);
                            eventObjInstanceList.Add(obj.gameObject);
                            break;
                        case DunGeonEvent.Gathering:
                            var createGt = eventObj as GatheringData;
                            var obj2 = createGt.Createobj(gatheringObjPrefab, gatheringSystem, this);
                            eventObjInstanceList.Add(obj2.gameObject);
                            break;
                        case DunGeonEvent.Hunt:
                            var createHt = eventObj as HuntingData;
                            var obj3 = createHt.Createobj(eventObjectPrefab, this);
                            eventObjInstanceList.Add(obj3.gameObject);
                            break;
                        case DunGeonEvent.RandomIncount:
                            break;
                        case DunGeonEvent.SubStory:
                            break;
                    }
                }
            }
        }
    }

    public void SetCheckRoom(DungeonRoom curRoom, DungeonRoom beforeRoom)
    {
        var obj = dungeonSystemData.dungeonRoomObjectList.Find(x => x.roomIdx == curRoom.roomIdx);
        var mesh = obj.gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;

        if (beforeRoom == null)
            return;
        if (beforeRoom.IsCheck == true)
        {
            var obj2 = dungeonSystemData.dungeonRoomObjectList.Find(x => x.roomIdx == beforeRoom.roomIdx);
            var mesh2 = obj2.gameObject.GetComponent<MeshRenderer>();

            mesh2.material.color = (beforeRoom.RoomType == DunGeonRoomType.MainRoom) ?
            new Color(0.962f, 0.174f, 0.068f) : new Color(0.472f, 0.389f, 0.389f);
        }
    }

    public void CreateMapObject()
    {
        int curIdx = dungeonSystemData.dungeonStartIdx;
        while (dungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx != -1)
        {
            var room = dungeonSystemData.dungeonRoomArray[curIdx];
            if (room.RoomType == DunGeonRoomType.MainRoom)
            {
                var obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
                     , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                //objectInfo.text.SetText(GetText(room));
                objectInfo.roomIdx = room.roomIdx;
                dungeonRoomObjectList.Add(obj);
            }
            else
            {
                var obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
                , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                //objectInfo.text.SetText(GetText(room));
                objectInfo.roomIdx = room.roomIdx;
                dungeonRoomObjectList.Add(obj);
            }
            curIdx = dungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx;
        }
        var lastRoom = dungeonSystemData.dungeonRoomArray[curIdx];
        var lastObj = Instantiate(mainRoomPrefab, new Vector3(lastRoom.Pos.x, lastRoom.Pos.y, 0f)
                     , Quaternion.identity, mapPos.transform);
        var objectInfo2 = lastObj.GetComponent<RoomObject>();
        objectInfo2.roomIdx = lastRoom.roomIdx;
        dungeonRoomObjectList.Add(lastObj);

        mapPos.transform.position = mapPos.transform.position + new Vector3(0f, 30f, 0f);
        dungeonSystemData.dungeonRoomObjectList = dungeonRoomObjectList;
    }


    //public void GatheringEventObject(List<int> gatheringEvent)
    //{
    //    for (int i = 0; i < gatheringEvent.Count; i++)
    //    {

    //    }
    //}
}
//switch(roomData.roadCount)
//{
//    case 2:
//        roomPrefab[1].gameObject.SetActive(true);
//        for (int i = 0; i < roomPrefab[1].objPosList.Count; i++)
//        {
//            roomData.eventObjPosBase = roomPrefab[1].objPosList[i];
//            roomData = dungeonSystemData.dungeonRoomArray[roomData.nextRoomIdx];
//        }
//        dungeonSystemData.curRoomData = roomPrefab[1];
//        break;
//    case 3:
//        roomPrefab[2].gameObject.SetActive(true);
//        for (int i = 0; i < roomPrefab[2].objPosList.Count; i++)
//        {
//            roomData.eventObjPosBase = roomPrefab[2].objPosList[i];
//            roomData = dungeonSystemData.dungeonRoomArray[roomData.nextRoomIdx];
//        }
//        dungeonSystemData.curRoomData = roomPrefab[2];
//        break;
//    case 4:
//        roomPrefab[3].gameObject.SetActive(true);
//        for (int i = 0; i < roomPrefab[3].objPosList.Count; i++)
//        {
//            roomData.eventObjPosBase = roomPrefab[3].objPosList[i];
//            roomData = dungeonSystemData.dungeonRoomArray[roomData.nextRoomIdx];
//        }
//        dungeonSystemData.curRoomData = roomPrefab[3];
//        break;
//    case 5:
//        roomPrefab[4].gameObject.SetActive(true);
//        for (int i = 0; i < roomPrefab[4].objPosList.Count; i++)
//        {
//            roomData.eventObjPosBase = roomPrefab[4].objPosList[i];
//            roomData = dungeonSystemData.dungeonRoomArray[roomData.nextRoomIdx];
//        }
//        dungeonSystemData.curRoomData = roomPrefab[4];
//        break;
//}

//for (int i = 0; i < roomData.eventList.Count; i++)
//{
//    switch (roomData.eventList[i])
//    {
//        case DunGeonEvent.Empty:
//            break;
//        case DunGeonEvent.Battle:
//            break;
//        case DunGeonEvent.Gathering:

//            break;
//        case DunGeonEvent.Hunt:
//            break;
//        case DunGeonEvent.RandomIncount:
//            break;
//        case DunGeonEvent.SubStory:
//            break;
//        case DunGeonEvent.Count:
//            break;
//    }
//}
//GatheringSystem.DeleteObj();
//dungeonSystemData.curRoomData.DestroyAllEventObject();
//dungeonSystemData.curEventObjList.Clear();
//dungeonSystemData.curRoomData.CreateAllEventObject(dungeonSystemData.curIncludeRoomList, eventObjectPrefab);

//GatheringSystem.CreateGathering(DungeonSystemData.curDungeonData.RoomType,
//DungeonSystemData.curRoomData.evnetObjPos, DungeonSystemData.curIncludeRoomList);

// add������Ʈ �������� �ڵ�
//public void EventObjectCreate(DungeonRoom roomData)
//{
//    // ������ �� �������� ������Ʈ ������ �ִ� ������ ����Ʈ (��� 1��, �� ���� = 1��, �� = ���� �� �� Ƚ����ŭ )
//    for (int i = 0; i < dungeonSystemData.curRoomData.objPosList.Count; i++)
//    {
//        for (int j = 0; j < roomData.eventObjDataList.Count; j++)
//        {
//            var eventData = roomData.eventObjDataList[j];
//            Vector3 position;
//            GameObject instanceObj;
//            switch (eventData.eventType)
//            {
//                case DunGeonEvent.Empty:
//                    break;
//                case DunGeonEvent.Battle:
//                    position = roomData.eventObjPosBase;
//                    instanceObj = Instantiate(eventObjectPrefab, new Vector3(position.x, position.y + 1f, position.z), Quaternion.identity);
//                    instanceObj.AddComponent<EventObject>().Init(roomData, DunGeonEvent.Battle, this, position);
//                    eventObjInstanceList.Add(instanceObj);
//                    break;
//                case DunGeonEvent.Gathering:
//                    var gatherData = eventData as GatheringData;
//                    for (int k = 0; k < gatherData.count; k++)
//                    {
//                        position = roomData.eventObjPosBase;
//                        var instanceObj2 = Instantiate(eventObjectPrefab, new Vector3(position.x, position.y + 1f, position.z + gatherData.offSetBasePos[k]), Quaternion.identity);
//                        instanceObj2.AddComponent<GatheringObject>().Init(gatheringSystem);
//                        instanceObj2.AddComponent<EventObject>().Init(roomData, DunGeonEvent.Gathering, this, position);
//                        gatherData.objectPosition[k] = instanceObj2.transform.position;
//                        eventObjInstanceList.Add(instanceObj2);
//                    }
//                    break;
//                case DunGeonEvent.Hunt:
//                    var huntingData = eventData;
//                    position = roomData.eventObjPosBase;
//                    instanceObj = Instantiate(eventObjectPrefab, new Vector3(position.x, position.y + 1f, position.z - 1), Quaternion.identity);
//                    instanceObj.AddComponent<EventObject>().Init(roomData, DunGeonEvent.Hunt, this, position);
//                    eventObjInstanceList.Add(instanceObj);
//                    break;
//                case DunGeonEvent.RandomIncount:

//                    break;
//                case DunGeonEvent.SubStory:

//                    break;
//                case DunGeonEvent.Count:
//                    break;
//            }

//        }
//        roomData = dungeonSystemData.dungeonRoomArray[roomData.nextRoomIdx];
//    }
//}