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
    public NewRoomGenerate roomGenerate;
    public MinimapGenerate minimapGenerate;
    public EventObjectGenerate eventObjectGenerate;

    [Header("기타")]
    public Button campButton;
    public RandomEventUIManager rndUi;
    public PlayerMoveControl playerMove;
    public TutorialPlayerMove tutorialMove;
    public MiniMapCamMove minimapCam;
    public GameObject DungeonCanvas;
    public TutorialRandomEvent randomEventTutorial;
    public bool isBattleObjectStart = false;
    // 던전맵 생성기에서 옮겨와야 되는 기능들
    public WorldMapMaker worldMap;

    [Header("튜토리얼")]
    public MoveTutorial moveTutorial;
    public GatheringTutorial gatherTutorial;
    public MainRoomTutorial mainRoomTutorial;

    // 코드 길이 간편화 작업에 필요한 것들 - 진행중..
    private Vector2 curDungeonIndex;
    private int startIndex;
    private int lastIndex;
    //public void OnGUI()
    //{
    //    if (GUI.Button(new Rect(100, 100, 100, 75), "Clear"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = true;
    //        GameManager.Manager.Production.FadeIn(() => GameManager.Manager.LoadScene(GameScene.World));
    //        Vars.UserData.uData.Date++;
    //    }
    //    if (GUI.Button(new Rect(100, 200, 100, 75), "Run"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = false;
    //        GameManager.Manager.Production.FadeIn(() => GameManager.Manager.LoadScene(GameScene.World));
    //        Vars.UserData.uData.Date++;
    //    }
    //    if (GUI.Button(new Rect(100, 400, 100, 150), "Start"))
    //    {
    //        Init();
    //    }
    //}
    private void Awake()
    {
        instance = this;
        EndInit();
    }

    private void Start()
    {
        if (Vars.UserData.mainTutorial == MainTutorialStage.Clear)
        {
            Init();
        }
        SoundManager.Instance.Play(SoundType.BG_Main);

    }

    public void EndInit()
    {
        dungeonPlayerGirl.gameObject.SetActive(false);
        dungeonPlayerBoy.gameObject.SetActive(false);
        eventObjectGenerate.EventObjectClear();
        roomGenerate.EndInit();
        DungeonCanvas.SetActive(false);
    }

    public void TutorialInit()
    {
        tutorialMove.Init();
        GameManager.Manager.State = GameState.Tutorial;
    }

    public void Init()
    {
        DungeonCanvas.SetActive(true);
        eventObjectGenerate.Init();
        minimapCam.Init();
        roomGenerate.Init();

        if(Vars.UserData.mainTutorial != MainTutorialStage.Clear)
        {
            dungeonSystemData = Vars.UserData.tutorialDungeonData;
            startIndex = 0;
        }
        else
        {
            playerMove.gameObject.SetActive(true);
            playerMove.Init();
            rndUi.Init();

            curDungeonIndex = Vars.UserData.curDungeonIndex;
            startIndex = Vars.UserData.dungeonStartIdx;
            lastIndex = Vars.UserData.dungeonLastIdx;
            // 첫 세이브에서 이거없으면 오류
            if (Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData == null)
            {
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
            }

            // 도망치거나 새로 도전할때 플레이어 현재방 위치 처음으로
            if (Vars.UserData.isDungeonReStart)
            {
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
                Vars.UserData.isDungeonReStart = false;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerBoyData.curRoomNumber = -1;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerGirlData.curRoomNumber = -1;
            }
            // 클리어한 방 다시 돌아올때, 플레이어 위치만 방 첫 위치로
            else if (Vars.UserData.isDungeonClear)
            {
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[lastIndex];
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData.eventList.Clear();
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData.eventObjDataList.Clear();
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerBoyData.curRoomNumber = -1;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerGirlData.curRoomNumber = -1;
            }

            if (Vars.UserData.AllDungeonData[curDungeonIndex] != null)
            {
                dungeonSystemData = Vars.UserData.AllDungeonData[curDungeonIndex];
            }
            worldMap.InitWorldMiniMap();

           // GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
            GameManager.Manager.State = GameState.Dungeon;
        }

        roomTool = new RoomTool();
        if (dungeonSystemData.curDungeonRoomData != null && Vars.UserData.mainTutorial == MainTutorialStage.Clear)
        {
            ConvertEventDataType();
        }
        GameManager.Manager.Production.FadeOut();
        DungeonRoomSetting();
    }

    // 던전맵이 완성된 후에 정보를 토대로 방 세팅
    private void DungeonRoomSetting()
    {
        if (Vars.UserData.mainTutorial == MainTutorialStage.Camp
            || Vars.UserData.mainTutorial == MainTutorialStage.Stamina)
        {
            dungeonSystemData.curDungeonRoomData = Vars.UserData.tutorialDungeonData.dungeonRoomArray[3];
        }

        minimapGenerate.CreateMiniMapObject();
        dungeonPlayerGirl.gameObject.SetActive(true);
        dungeonPlayerBoy.gameObject.SetActive(true);

        if (dungeonSystemData.curDungeonRoomData != null)
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


        if(Vars.UserData.mainTutorial == MainTutorialStage.Move)
        {
            ChangeRoomEvent(true, false);
        }
        else if(Vars.UserData.mainTutorial != MainTutorialStage.Clear)
        {
            TutorialStart();
        }
    }

    public void TutorialStart()
    {
        if (Vars.UserData.mainTutorial == MainTutorialStage.Clear)
            return;
        switch (Vars.UserData.mainTutorial)
        {
            case MainTutorialStage.Move:
                StartCoroutine(moveTutorial.CoMoveTutorial());
                break;
            case MainTutorialStage.Event:
                StartCoroutine(gatherTutorial.CoGatheringTutorial());
                break;
            case MainTutorialStage.Stamina:
                StartCoroutine(mainRoomTutorial.CoMainRoomTutorial());
                break;
            case MainTutorialStage.Camp:
                StartCoroutine(mainRoomTutorial.CoTutorialEnd());
                break;
            case MainTutorialStage.Clear:
                break;
        }
    }

    // 방마다 위치해있는 트리거 발동할때 실행, 방 바뀔때
    public void ChangeRoomEvent(bool isRoomEnd, bool isGoForward)
    {
        Debug.Log(dungeonSystemData.curDungeonRoomData.roomIdx);
        if (isRoomEnd)
        {
            eventObjectGenerate.EventObjectClear();
            roomGenerate.RoadListClear();

            if (dungeonSystemData.curDungeonRoomData.nextRoomIdx == -1)
            {
                if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
                {
                    dungeonSystemData = null;
                    GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial(false);
                    // 랜턴값, 스테미너값, HP값 등등 튜토리얼에서 변경된 값들 다시 초기화 해야됨
                    ConsumeManager.CostDataReset();
                    DataAllItem temp;
                    var tempInventory = new List<DataAllItem>(Vars.UserData.HaveAllItemList);
                    foreach (var item in tempInventory)
                    {
                        temp = new DataAllItem(item);
                        Vars.UserData.RemoveItemData(temp);
                    }
                    // 화살
                    var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
                    temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_20"));
                    temp.OwnCount = 40;
                    Vars.UserData.AddItemData(temp); // 40발

                    // 오일
                    allItemTable = DataTableManager.GetTable<AllItemDataTable>();
                    temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_19"));
                    temp.OwnCount = 4;
                    Vars.UserData.AddItemData(temp); // 4개

                    // 나무도막
                    allItemTable = DataTableManager.GetTable<AllItemDataTable>();
                    temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_1"));
                    temp.OwnCount = 3;
                    Vars.UserData.AddItemData(temp); // 3개
                }
                else
                {
                    Vars.UserData.WorldMapPlayerData.isClear = true;
                    Vars.UserData.isDungeonClear = true;
                }
                SoundManager.Instance.Play(SoundType.Se_win);
                GameManager.Manager.Production.FadeIn( () => GameManager.Manager.LoadScene(GameScene.World));
                return;
            }

            beforeDungeonRoom = dungeonSystemData.curDungeonRoomData;
            dungeonSystemData.curDungeonRoomData = roomTool.GetNextRoom(dungeonSystemData.curDungeonRoomData);

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

            if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
            {
                TutorialStart();
            }

            GameManager.Manager.Production.FadeOut();
        }
        else
        {
            if (isGoForward)
            {
                if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
                {
                    TutorialStart();
                }

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

        dungeonSystemData.curPlayerBoyData.SetUnitData(dungeonPlayerBoy);
        dungeonSystemData.curPlayerGirlData.SetUnitData(dungeonPlayerGirl);

        if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
        {
            Vars.UserData.tutorialDungeonData = dungeonSystemData;
        }
        else
        {
            Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
            ConsumeManager.SaveConsumableData();
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.item);
        }
    }

    // 세이브 후 로드할때 이벤트 타입이 부모타입으로 바뀌어있는걸 다시 변경
    public void ConvertEventDataType()
    {
        var array = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray;
        var curIdx = startIndex;

        while (curIdx != -1)
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
                    newData2.offSetBasePos = eventList[i].offSetBasePos;
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
        obj.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        if (beforeRoom == null)
            return;
        if (beforeRoom.IsCheck == true)
        {
            var obj2 = minimapGenerate.dungeonRoomObjectList.Find(x => x.roomIdx == beforeRoom.roomIdx);
            obj2.gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
