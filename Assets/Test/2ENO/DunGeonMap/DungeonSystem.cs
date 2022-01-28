using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class DungeonSystem : MonoBehaviour
{
    private static DungeonSystem instance;
    public static DungeonSystem Instance => instance;

    RoomTool roomManager;
    private List<GameObject> eventObjInstanceList = new List<GameObject>();
    private DungeonRoom beforeDungeonRoom;

    // 던전 세팅, 불러오기에 필요한 모든 데이터를 이걸통해 관리!
    private DungeonData dungeonSystemData = new();
    public DungeonData DungeonSystemData
    {
        get => dungeonSystemData;
    }

    [Header("프리팹, 인스턴스")]
    public NewRoomControl roomGenerate;
    public BattleObject battleObjPrefab;
    //public HuntingObject huntingObjPrefab;
    public GameObject huntingObjPrefab;
    public RandomEventObject randomEventObjPrefab;
    public GatheringObject treeObj;
    public GatheringObject pitObj;
    public GatheringObject herbsObj;
    public GatheringObject mushroomObj;
    

    [Header("플레이어, 다른시스템")]
    public PlayerDungeonUnit dungeonPlayerGirl;
    public PlayerDungeonUnit dungeonPlayerBoy;
    public GatheringSystem gatheringSystem;

    // 던전맵 생성기에서 옮겨와야 되는 기능들
    [Header("맵 생성(오브젝트)관련")]
    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public GameObject mapPos;
    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();
    public WorldMapMaker worldMap;

    [Header("기타")]
    public Button campButton;
    public RandomEventUIManager rndUi;
    public MoveTest playerMove;
    public MiniMapCamMove minimapCam;

    // 코드 길이 간편화 작업에 필요한 것들 - 진행중..
    private Vector2 curDungeonIndex;
    private int startIndex;
    private int curDungeonRoomIndex;

    private void Awake()
    {
        instance = this;
        Init();
        rndUi.Init();
        playerMove.Init();
        minimapCam.Init();
    }

    public void Init()
    {
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
        if (Vars.UserData.dungeonReStart)
        {
            Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
            Vars.UserData.dungeonReStart = false;
        }
        if (Vars.UserData.AllDungeonData[curDungeonIndex] != null)
        {
            dungeonSystemData = Vars.UserData.AllDungeonData[curDungeonIndex];
        }

        roomManager = new RoomTool();
        //roomManager.text = text;

        //if (dungeonSystemData.curDungeonData != null)
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

    //void Start()
    //{
    //    // 현재 불러올 맵 데이터가 없을 때
    //    if (Vars.UserData.AllDungeonData.Count <= 0)
    //        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);

    //    dungeonPlayerGirl.gameObject.SetActive(false);
    //    dungeonPlayerBoy.gameObject.SetActive(false);

    //    curDungeonIndex = Vars.UserData.curDungeonIndex;
    //    startIndex = Vars.UserData.dungeonStartIdx;
    //    if(Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData == null)
    //    {
    //        Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
    //    }

    //    // 도망치거나 새로 도전할때 플레이어 현재방 위치 처음으로
    //    if (Vars.UserData.dungeonReStart)
    //    {
    //        Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
    //        Vars.UserData.dungeonReStart = false;
    //    }
    //    if (Vars.UserData.AllDungeonData[curDungeonIndex] != null)
    //    {
    //        dungeonSystemData = Vars.UserData.AllDungeonData[curDungeonIndex];
    //    }

    //    roomManager = new RoomTool();
    //    //roomManager.text = text;

    //    //if (dungeonSystemData.curDungeonData != null)
    //    ConvertEventDataType();
    //    DungeonRoomSetting();
    //    worldMap.InitWorldMiniMap();


    //    GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    //    // TODO: 임시! 가라로 해놓은거
    //    GameManager.Manager.State = GameState.Dungeon;
    //}

    // 던전맵이 완성된 후에 정보를 토대로 방 세팅
    private void DungeonRoomSetting()
    {
        CreateMiniMapObject();
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

        EventObjectCreate(dungeonSystemData.curDungeonRoomData);
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
            foreach (var obj in eventObjInstanceList)
            {
                Destroy(obj);
            }
            eventObjInstanceList.Clear();

            if(dungeonSystemData.curDungeonRoomData.nextRoomIdx == -1)
            {
                Vars.UserData.WorldMapPlayerData.isClear = true;
                Vars.UserData.curDungeonIndex = Vector2.zero;
                Vars.UserData.AllDungeonData.Clear();
                //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
                SceneManager.LoadScene("AS_WorldMap");
                return;
            }
            beforeDungeonRoom = dungeonSystemData.curDungeonRoomData;
            dungeonSystemData.curDungeonRoomData = roomManager.GetNextRoom(dungeonSystemData.curDungeonRoomData);

            roomGenerate.RoomPrefabSet(dungeonSystemData.curDungeonRoomData);
            EventObjectCreate(dungeonSystemData.curDungeonRoomData);

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
                dungeonSystemData.curDungeonRoomData = roomManager.GetNextRoom(dungeonSystemData.curDungeonRoomData);
                CurrentRoomInMinimap(dungeonSystemData.curDungeonRoomData, beforeDungeonRoom);
            }
            else
            {
                beforeDungeonRoom = dungeonSystemData.curDungeonRoomData;
                dungeonSystemData.curDungeonRoomData = roomManager.GetBeforeRoom(dungeonSystemData.curDungeonRoomData);
                CurrentRoomInMinimap(dungeonSystemData.curDungeonRoomData, beforeDungeonRoom);
            }
        }

        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
        // TODO: 이거 안풀면 curDungeonData 저장 재대로 못함
        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }

    //Vars.UserData.curDungeonRoomIdx = dungeonSystemData.curDungeonRoomData.roomIdx;

    // 세이브 후 로드할때 이벤트 타입이 부모타입으로 바뀌어있는걸 다시 변경
    public void ConvertEventDataType()
    {
        var array = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray;
        var curIdx = startIndex;

        while (array[curIdx].nextRoomIdx != -1)
        {
            EventDataInit(array[curIdx].eventObjDataList);
            curIdx = array[curIdx].nextRoomIdx;
        }

        dungeonSystemData.curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData;
    }
    public void EventDataInit(List<EventData> eventList)
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

    // 현재 방 정보만 가지고 이벤트 오브젝트 생성
    public void EventObjectCreate(DungeonRoom roomData)
    {
        // 길방의 오브젝트들 생성
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
                                var obj = createBt.CreateObj(battleObjPrefab);
                                eventObjInstanceList.Add(obj.gameObject);
                                break;
                            case DunGeonEvent.Gathering:
                                var createGt = eventObj as GatheringData;
                                GatheringObject obj2;
                                switch (eventObj.gatheringtype)
                                {
                                    case GatheringObjectType.Tree:
                                        obj2 = createGt.CreateObj(treeObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                    case GatheringObjectType.Pit:
                                        obj2 = createGt.CreateObj(pitObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                    case GatheringObjectType.Herbs:
                                        obj2 = createGt.CreateObj(herbsObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                    case GatheringObjectType.Mushroom:
                                        obj2 = createGt.CreateObj(mushroomObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                }
                                break;
                            case DunGeonEvent.Hunt:
                                var createHt = eventObj as HuntingData;
                                var obj3 = createHt.CreateObj(huntingObjPrefab);
                                eventObjInstanceList.Add(obj3.gameObject);
                                break;
                            case DunGeonEvent.RandomIncount:
                                var createRi = eventObj as RandomIncountData;
                                var obj4 = createRi.CreateObj(randomEventObjPrefab);
                                eventObjInstanceList.Add(obj4.gameObject);
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
            foreach (var eventObj in roomData.eventObjDataList)
            {
                {
                    switch (eventObj.eventType)
                    {
                        case DunGeonEvent.Battle:
                            //var createBt = eventObj as BattleData;
                            //var obj = createBt.CreateObj(battleObjPrefab);
                            //eventObjInstanceList.Add(obj.gameObject);
                            break;
                        case DunGeonEvent.Gathering:
                            var createGt = eventObj as GatheringData;
                            GatheringObject obj2;
                            switch (eventObj.gatheringtype)
                            {
                                case GatheringObjectType.Tree:
                                    obj2 = createGt.CreateObj(treeObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                                case GatheringObjectType.Pit:
                                    obj2 = createGt.CreateObj(pitObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                                case GatheringObjectType.Herbs:
                                    obj2 = createGt.CreateObj(herbsObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                                case GatheringObjectType.Mushroom:
                                    obj2 = createGt.CreateObj(mushroomObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                            }

                            break;
                        case DunGeonEvent.Hunt:
                            var createHt = eventObj as HuntingData;
                            var obj3 = createHt.CreateObj(huntingObjPrefab);
                            eventObjInstanceList.Add(obj3.gameObject);
                            break;
                        case DunGeonEvent.RandomIncount:
                            var createRi = eventObj as RandomIncountData;
                            var obj4 = createRi.CreateObj(randomEventObjPrefab);
                            eventObjInstanceList.Add(obj4.gameObject);
                            break;
                        case DunGeonEvent.SubStory:
                            break;
                    }
                }
            }
        }
    }

    public void CurrentRoomInMinimap(DungeonRoom curRoom, DungeonRoom beforeRoom)
    {
        var obj = dungeonRoomObjectList.Find(x => x.roomIdx == curRoom.roomIdx);
        var mesh = obj.gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;

        if (beforeRoom == null)
            return;
        if (beforeRoom.IsCheck == true)
        {
            var obj2 = dungeonRoomObjectList.Find(x => x.roomIdx == beforeRoom.roomIdx);
            var mesh2 = obj2.gameObject.GetComponent<MeshRenderer>();

            mesh2.material.color = (beforeRoom.RoomType == DunGeonRoomType.MainRoom) ?
            new Color(0.962f, 0.174f, 0.068f) : new Color(0.472f, 0.389f, 0.389f);
        }
    }

    public void CreateMiniMapObject()
    {

        int curIdx = startIndex;
        int left, right, top, bottom;
        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos = Vector3.zero;
        Vector3 topPos = Vector3.zero;
        Vector3 bottomPos = Vector3.zero;

        left = curIdx % 20;
        right = curIdx % 20;
        top = curIdx / 20;
        bottom = curIdx / 20;

        /*dungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx*/
        while ( curIdx != -1)
        {
            var room = dungeonSystemData.dungeonRoomArray[curIdx];
            RoomObject obj;
            if (room.RoomType == DunGeonRoomType.MainRoom)
            {
                obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, 0f, room.Pos.y)
                     , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                dungeonRoomObjectList.Add(obj);
            }
            else
            {
                obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, 0f, room.Pos.y)
                , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                dungeonRoomObjectList.Add(obj);
            }
            if(curIdx == startIndex)
            {
                leftPos = obj.transform.position;
                rightPos = obj.transform.position;
                topPos = obj.transform.position;
                bottomPos = obj.transform.position;
            }

            if (curIdx != 0)
            {
                if (left > curIdx % 20)
                {
                    left = curIdx % 20;
                    leftPos = obj.transform.position;
                }
                if (right < curIdx % 20)
                {
                    right = curIdx % 20;
                    rightPos = obj.transform.position;
                }
                if (top > curIdx / 20)
                {
                    top = curIdx / 20;
                    topPos = obj.transform.position;
                }
                if (bottom < curIdx / 20)
                {
                    bottom = curIdx / 20;
                    bottomPos = obj.transform.position;
                }
            }
            curIdx = dungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx;
        }

        minimapCam.leftVec = leftPos;
        minimapCam.rightVec = rightPos;
        minimapCam.topVec = topPos;
        minimapCam.bottomVec = bottomPos;

        //var lastRoom = dungeonSystemData.dungeonRoomArray[curIdx];
        //var lastObj = Instantiate(mainRoomPrefab, new Vector3(lastRoom.Pos.x, lastRoom.Pos.y, 0f)
        //             , Quaternion.identity, mapPos.transform);
        //var objectInfo2 = lastObj.GetComponent<RoomObject>();
        //objectInfo2.roomIdx = lastRoom.roomIdx;
        //dungeonRoomObjectList.Add(lastObj);

        //mapPos.transform.position = mapPos.transform.position + new Vector3(0f, 3000f, 0f);
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

//foreach (var data in array)
//{
//    if (data.IsCheck == true)
//    {
//        for (int i = 0; i < data.eventObjDataList.Count; i++)
//        {
//            switch (data.eventObjDataList[i].eventType)
//            {
//                case DunGeonEvent.Battle:
//                    var newData = new BattleData();
//                    newData.eventType = DunGeonEvent.Battle;
//                    newData.isCreate = data.eventObjDataList[i].isCreate;
//                    newData.eventBasePos = data.eventObjDataList[i].eventBasePos;
//                    newData.roomIndex = data.eventObjDataList[i].roomIndex;
//                    newData.objectPosition = data.eventObjDataList[i].objectPosition;
//                    data.eventObjDataList.Remove(data.eventObjDataList[i]);
//                    data.eventObjDataList.Add(newData);
//                    break;
//                case DunGeonEvent.Gathering:
//                    var newData2 = new GatheringData();
//                    newData2.eventType = DunGeonEvent.Battle;
//                    newData2.isCreate = data.eventObjDataList[i].isCreate;
//                    newData2.eventBasePos = data.eventObjDataList[i].eventBasePos;
//                    newData2.roomIndex = data.eventObjDataList[i].roomIndex;
//                    newData2.objectPosition = data.eventObjDataList[i].objectPosition;
//                    data.eventObjDataList.Remove(data.eventObjDataList[i]);
//                    data.eventObjDataList.Add(newData2);
//                    break;
//                case DunGeonEvent.Hunt:
//                    var newData3 = new HuntingData();
//                    newData3.eventType = DunGeonEvent.Battle;
//                    newData3.isCreate = data.eventObjDataList[i].isCreate;
//                    newData3.eventBasePos = data.eventObjDataList[i].eventBasePos;
//                    newData3.roomIndex = data.eventObjDataList[i].roomIndex;
//                    newData3.objectPosition = data.eventObjDataList[i].objectPosition;
//                    data.eventObjDataList.Remove(data.eventObjDataList[i]);
//                    data.eventObjDataList.Add(newData3);
//                    break;
//                case DunGeonEvent.RandomIncount:
//                    break;
//                case DunGeonEvent.SubStory:
//                    break;
//            }
//        }
//    }
//}

//// 이벤트 오브젝트 클릭시, 실행
//public void EventObjectClickEvent(DunGeonEvent eventType, EventObject eventObject)
//{
//    dungeonSystemData.curPlayerData.SetUnitData(dungeonPlayer);

//    switch (eventType)
//    {
//        case DunGeonEvent.Empty:
//            break;
//        case DunGeonEvent.Battle:
//            break;
//        case DunGeonEvent.Gathering:
//            gatheringSystem.GoGatheringObject(eventObject.gameObject.transform.position);
//            break;
//        case DunGeonEvent.Hunt:
//            Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
//            //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
//            SceneManager.LoadScene("Hunting");
//            break;
//        case DunGeonEvent.RandomIncount:
//            break;
//        case DunGeonEvent.SubStory:
//            break;
//        case DunGeonEvent.Count:
//            break;
//    }
//}

//public void RoomPrefabSetting(DungeonRoom roomData)
//{
//    for (int i = 0; i < roomPrefab.Length; i++)
//    {
//        roomPrefab[i].gameObject.SetActive(false);
//    }
//    if (roomData.RoomType == DunGeonRoomType.MainRoom)
//    {
//        // 메인방 프리팹 생성, 및 현재 방 이벤트 데이터들에 basePos 세팅
//        roomPrefab[0].gameObject.SetActive(true);
//        foreach(var obj in roomData.eventObjDataList)
//        {
//            obj.eventBasePos = roomPrefab[0].objPosList[0];
//        }
//        dungeonSystemData.curRoomInstanceData = roomPrefab[0];
//    }
//    else
//    {
//        // 2개 프리팹부터 5개 프리팹까지 순회
//        for (int i = 1; i < roomPrefab.Length; i++)
//        {
//            // 현재 방의 길방 수와 프리팹을 매칭
//            if(roomData.roadCount - 1 == i)
//            {
//                roomPrefab[i].gameObject.SetActive(true);
//                DungeonRoom curRoom = roomData;
//                // 각 길방의 오브젝트 포지션을 해당 방 데이터에 담는다
//                for (int j = 0; j < roomPrefab[i].objPosList.Count; j++)
//                {
//                    foreach(var obj in curRoom.eventObjDataList)
//                    {
//                        obj.eventBasePos = roomPrefab[i].objPosList[j];
//                    }
//                    curRoom = roomManager.GetNextRoom(curRoom);
//                }
//                dungeonSystemData.curRoomInstanceData = roomPrefab[i];
//            }
//        }
//    }
//}

// 현재 방 정보만 가지고 모두 생성 가능하게 만들기 일단 보류
//public void RoomCreate(DungeonRoom roomData)
//{
//    if(roomData.RoomType == DunGeonRoomType.MainRoom)
//    {
//        // 메인방 프리팹 생성
//    }
//    else
//    {
//        if (dungeonSystemData.dungeonRoomArray[roomData.beforeRoomIdx].RoomType == DunGeonRoomType.SubRoom)
//            return;
//        // 길방 프리팹 생성 -> 연결된 모든 길
//        //int curIdx = roomData.roomIdx;
//        //while (dungeonSystemData.dungeonRoomArray[curIdx])
//    }
//}