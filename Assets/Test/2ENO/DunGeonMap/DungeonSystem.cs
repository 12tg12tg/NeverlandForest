using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class DungeonSystem : MonoBehaviour
{
    private static DungeonSystem instance;
    public static DungeonSystem Instance => instance;

    public RoomTool roomTool;
    private DungeonRoom beforeDungeonRoom;

    // 던전 세팅, 불러오기에 필요한 모든 데이터를 이걸통해 관리!
    private DungeonData dungeonSystemData = new();
    public DungeonData DungeonSystemData
    {
        get => dungeonSystemData;
    }

    [Header("플레이어, 인스턴스")]
    public PlayerDungeonUnit dungeonPlayerGirl;
    public PlayerDungeonUnit dungeonPlayerBoy;
    public GatheringSystem gatheringSystem;
    public NewRoomControl roomGenerate;
    public MinimapGenerate minimapGenerate;
    public EventObjectGenerate eventObjectGenerate;

    [Header("기타")]
    public Button campButton;
    public RandomEventUIManager rndUi;
    public MoveTest playerMove;
    public MiniMapCamMove minimapCam;
    public GameObject DungeonCanvas;

    // 던전맵 생성기에서 옮겨와야 되는 기능들
    public WorldMapMaker worldMap;

    // 코드 길이 간편화 작업에 필요한 것들 - 진행중..
    private Vector2 curDungeonIndex;
    private int startIndex;

    private void Start()
    {
        instance = this;
        eventObjectGenerate.Init();
        Init();
        rndUi.Init();
        playerMove.Init();
        minimapCam.Init();
    }

    public void EndInit()
    {
        dungeonPlayerGirl.gameObject.SetActive(false);
        dungeonPlayerBoy.gameObject.SetActive(false);
        eventObjectGenerate.EventObjectClear();
        roomGenerate.EndInit();
        DungeonCanvas.SetActive(false);
        playerMove.gameObject.SetActive(false);
    }

    public void Init()
    {
        DungeonCanvas.SetActive(true);
        playerMove.gameObject.SetActive(true);

        // 현재 불러올 맵 데이터가 없을 때
        if (Vars.UserData.AllDungeonData.Count <= 0)
            GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);

        dungeonPlayerGirl.gameObject.SetActive(false);
        dungeonPlayerBoy.gameObject.SetActive(false);

        curDungeonIndex = Vars.UserData.curDungeonIndex;
        startIndex = Vars.UserData.dungeonStartIdx;
        if (Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData == null)
        {
            Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
        }
        // 도망치거나 새로 도전할때 플레이어 현재방 위치 처음으로
        else if (Vars.UserData.dungeonReStart)
        {
            Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
            Vars.UserData.dungeonReStart = false;
            Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerBoyData.curRoomNumber = -1;
            Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerGirlData.curRoomNumber = -1;
        }
        // 클리어한 방 다시 돌아올때, 플레이어 위치만 방 첫 위치로
        else if(!Vars.UserData.dungeonReStart)
        {
        }

        if (Vars.UserData.AllDungeonData[curDungeonIndex] != null)
        {
            dungeonSystemData = Vars.UserData.AllDungeonData[curDungeonIndex];
        }

        roomTool = new RoomTool();
        if (dungeonSystemData.curDungeonRoomData != null)
            ConvertEventDataType();

        DungeonRoomSetting();
        worldMap.InitWorldMiniMap();

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
        // TODO: 임시! 가라로 해놓은거
        GameManager.Manager.State = GameState.Dungeon;
    }

    public void OnGUI()
    {
        if(GUI.Button(new Rect(100, 100, 100, 75), "Clear"))
        {
            Vars.UserData.WorldMapPlayerData.isClear = true;
            SceneManager.LoadScene("AS_WorldMap");
            Vars.UserData.uData.Date++;
        }
        if (GUI.Button(new Rect(100, 200, 100, 75), "Run"))
        {
            Vars.UserData.WorldMapPlayerData.isClear = false;
            SceneManager.LoadScene("AS_WorldMap");
            Vars.UserData.uData.Date++;
        }
    }

    // 던전맵이 완성된 후에 정보를 토대로 방 세팅
    private void DungeonRoomSetting()
    {
        minimapGenerate.CreateMiniMapObject();
        dungeonPlayerGirl.gameObject.SetActive(true);
        dungeonPlayerBoy.gameObject.SetActive(true);

        if(dungeonSystemData.curDungeonRoomData != null)
        {
            roomGenerate.RoomPrefabSet(dungeonSystemData.curDungeonRoomData);
        }
        else
        {
            roomGenerate.RoomPrefabSet(dungeonSystemData.dungeonRoomArray[startIndex]);
            dungeonSystemData.curDungeonRoomData = dungeonSystemData.dungeonRoomArray[startIndex];
        }

        eventObjectGenerate.EventObjectCreate(dungeonSystemData.curDungeonRoomData);
        CurrentRoomInMinimap(dungeonSystemData.curDungeonRoomData, beforeDungeonRoom);

        if (dungeonSystemData.curPlayerGirlData.curRoomNumber == -1)
        {
            var newPos1 = new Vector3(roomGenerate.spawnPos.x, roomGenerate.spawnPos.y, roomGenerate.spawnPos.z + 0.5f);
            var newPos2 = new Vector3(roomGenerate.spawnPos.x, roomGenerate.spawnPos.y, roomGenerate.spawnPos.z - 0.5f);
            dungeonPlayerGirl.transform.position = newPos2;
            dungeonPlayerBoy.transform.position = newPos1;
        }
        else
        {
            dungeonPlayerGirl.SetPlayerData(dungeonSystemData.curPlayerGirlData);
            dungeonPlayerBoy.SetPlayerData(dungeonSystemData.curPlayerBoyData);
        }

        if (dungeonSystemData.curDungeonRoomData.RoomType == DunGeonRoomType.MainRoom)
            campButton.interactable = true;
        else
            campButton.interactable = false;
    }

    // 방마다 위치해있는 트리거 발동할때 실행, 방 바뀔때
    public void ChangeRoomEvent(bool isRoomEnd, bool isGoForward)
    {
        if (isRoomEnd)
        {
            eventObjectGenerate.EventObjectClear();

            if (dungeonSystemData.curDungeonRoomData.nextRoomIdx == -1)
            {
                Vars.UserData.WorldMapPlayerData.isClear = true;
                //Vars.UserData.curDungeonIndex = Vector2.zero;
                //Vars.UserData.AllDungeonData.Clear();
                //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
                SceneManager.LoadScene("AS_WorldMap");
                return;
            }
            beforeDungeonRoom = dungeonSystemData.curDungeonRoomData;
            dungeonSystemData.curDungeonRoomData = roomTool.GetNextRoom(dungeonSystemData.curDungeonRoomData);

            roomGenerate.RoadListClear();
            roomGenerate.RoomPrefabSet(dungeonSystemData.curDungeonRoomData);
            eventObjectGenerate.EventObjectCreate(dungeonSystemData.curDungeonRoomData);

            var newPos1 = new Vector3(roomGenerate.spawnPos.x, roomGenerate.spawnPos.y, roomGenerate.spawnPos.z + 0.5f);
            var newPos2 = new Vector3(roomGenerate.spawnPos.x, roomGenerate.spawnPos.y, roomGenerate.spawnPos.z - 0.5f);
            dungeonPlayerGirl.transform.position = newPos2;
            dungeonPlayerBoy.transform.position = newPos1;

            CurrentRoomInMinimap(dungeonSystemData.curDungeonRoomData, beforeDungeonRoom);
            if (dungeonSystemData.curDungeonRoomData.RoomType == DunGeonRoomType.MainRoom)
                campButton.interactable = true;
            else
                campButton.interactable = false;

            ConsumeManager.TimeUp(0, 1);
        }
        else
        {
            if(isGoForward)
            {
                // 방 한칸 지날때마다 30분씩 지남
                ConsumeManager.TimeUp(0, 1);
                beforeDungeonRoom = dungeonSystemData.curDungeonRoomData;
                dungeonSystemData.curDungeonRoomData = roomTool.GetNextRoom(dungeonSystemData.curDungeonRoomData);
                CurrentRoomInMinimap(dungeonSystemData.curDungeonRoomData, beforeDungeonRoom);
            }
            else
            {
                beforeDungeonRoom = dungeonSystemData.curDungeonRoomData;
                dungeonSystemData.curDungeonRoomData = roomTool.GetBeforeRoom(dungeonSystemData.curDungeonRoomData);
                CurrentRoomInMinimap(dungeonSystemData.curDungeonRoomData, beforeDungeonRoom);
            }
        }

        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
        // TODO: 이거 안풀면 curDungeonData 저장 재대로 못함
        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }

    // 세이브 후 로드할때 이벤트 타입이 부모타입으로 바뀌어있는걸 다시 변경
    public void ConvertEventDataType()
    {
        var array = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray;
        var curIdx = startIndex;

        while (array[curIdx].nextRoomIdx != -1)
        {
            EventDataTypeInit(array[curIdx].eventObjDataList);
            curIdx = array[curIdx].nextRoomIdx;
        }

        dungeonSystemData.curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData;
    }
    public void EventDataTypeInit(List<EventData> eventList)
    {
        List<EventData> newList = new List<EventData>();
        for (int i = 0; i < eventList.Count; i++)
        {
            // TODO : 현재 eventList[i]가 EventData타입이라, 첨 실행할때 offsetBasePos를 gathring에 못넣어주는 일 생겨서 계속 000위치에 생성되는 버그, 여기 들어오는조건을 걸어둬야할 필요잇음
            switch (eventList[i].eventType)
            {
                case DunGeonEvent.Battle:
                    var newData = new BattleData();
                    newData.eventType = DunGeonEvent.Battle;
                    newData.isCreate = eventList[i].isCreate;
                    newData.eventBasePos = eventList[i].eventBasePos;
                    newData.roomIndex = eventList[i].roomIndex;
                    newData.objectPosition = eventList[i].objectPosition;
                    newList.Add(newData);
                    break;
                case DunGeonEvent.Gathering:
                    var newData2 = new GatheringData();
                    newData2.eventType = DunGeonEvent.Gathering;
                    newData2.isCreate = eventList[i].isCreate;
                    newData2.eventBasePos = eventList[i].eventBasePos;
                    newData2.offSetBasePos = (eventList[i] as GatheringData) == null ? 0 : (eventList[i] as GatheringData).offSetBasePos;
                    newData2.roomIndex = eventList[i].roomIndex;
                    newData2.objectPosition = eventList[i].objectPosition;
                    newData2.gatheringtype = eventList[i].gatheringtype;
                    newList.Add(newData2);
                    break;
                case DunGeonEvent.Hunt:
                    var newData3 = new HuntingData();
                    newData3.eventType = DunGeonEvent.Hunt;
                    newData3.isCreate = eventList[i].isCreate;
                    newData3.eventBasePos = eventList[i].eventBasePos;
                    newData3.roomIndex = eventList[i].roomIndex;
                    newData3.objectPosition = eventList[i].objectPosition;
                    newList.Add(newData3);
                    break;
                case DunGeonEvent.RandomIncount:
                    var newData4 = new RandomIncountData();
                    newData4.eventType = DunGeonEvent.RandomIncount;
                    newData4.isCreate = eventList[i].isCreate;
                    newData4.eventBasePos = eventList[i].eventBasePos;
                    newData4.roomIndex = eventList[i].roomIndex;
                    newData4.objectPosition = eventList[i].objectPosition;
                    newData4.randomEventID = eventList[i].randomEventID;
                    newList.Add(newData4);
                    break;
                case DunGeonEvent.SubStory:
                    break;
            }
        }
        eventList.Clear();
        eventList.AddRange(newList);
    }

    public void CurrentRoomInMinimap(DungeonRoom curRoom, DungeonRoom beforeRoom)
    {
        var obj = minimapGenerate.dungeonRoomObjectList.Find(x => x.roomIdx == curRoom.roomIdx);
        var mesh = obj.gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;

        if (beforeRoom == null)
            return;
        if (beforeRoom.IsCheck == true)
        {
            var obj2 = minimapGenerate.dungeonRoomObjectList.Find(x => x.roomIdx == beforeRoom.roomIdx);
            var mesh2 = obj2.gameObject.GetComponent<MeshRenderer>();

            mesh2.material.color = (beforeRoom.RoomType == DunGeonRoomType.MainRoom) ?
            new Color(0.962f, 0.174f, 0.068f) : new Color(0.472f, 0.389f, 0.389f);
        }
    }
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

// add컴포넌트 오류나는 코드
//public void EventObjectCreate(DungeonRoom roomData)
//{
//    // 생성된 방 프리팹의 오브젝트 놓을수 있는 포지션 리스트 (방당 1개, 즉 메인 = 1번, 길 = 현재 길 방 횟수만큼 )
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
