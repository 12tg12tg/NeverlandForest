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

    // ���� ����, �ҷ����⿡ �ʿ��� ��� �����͸� �̰����� ����!
    private DungeonData dungeonSystemData = new();
    public DungeonData DungeonSystemData
    {
        get => dungeonSystemData;
    }

    [Header("�÷��̾�, �ν��Ͻ�")]
    public PlayerDungeonUnit dungeonPlayerGirl;
    public PlayerDungeonUnit dungeonPlayerBoy;
    public GatheringSystem gatheringSystem;
    public NewRoomGenerate roomGenerate;
    public MinimapGenerate minimapGenerate;
    public EventObjectGenerate eventObjectGenerate;

    [Header("��Ÿ")]
    public Button campButton;
    public RandomEventUIManager rndUi;
    public MoveTest playerMove;
    public TutorialPlayerMove tutorialMove;
    public MiniMapCamMove minimapCam;
    public GameObject DungeonCanvas;
    public TutorialRandomEvent randomEventTutorial;

    // ������ �����⿡�� �Űܿ;� �Ǵ� ��ɵ�
    public WorldMapMaker worldMap;

    private TutorialManager tutorialManager;

    [Header("Ʃ�丮��")]
    public MoveTutorial moveTutorial;
    public GatheringTutorial gatherTutorial;
    public MainRoomTutorial mainRoomTutorial;

    // �ڵ� ���� ����ȭ �۾��� �ʿ��� �͵� - ������..
    private Vector2 curDungeonIndex;
    private int startIndex;
    private int lastIndex;
    //public void OnGUI()
    //{
    //    if(GUI.Button(new Rect(100, 100, 100, 75), "Clear"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = true;
    //        GameManager.Manager.LoadScene(GameScene.World);
    //        Vars.UserData.uData.Date++;
    //    }
    //    if (GUI.Button(new Rect(100, 200, 100, 75), "Run"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = false;
    //        GameManager.Manager.LoadScene(GameScene.World);
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
        if(Vars.UserData.mainTutorial == MainTutorialStage.Clear)
            Init();
    }

    public void EndInit()
    {
        dungeonPlayerGirl.gameObject.SetActive(false);
        dungeonPlayerBoy.gameObject.SetActive(false);
        eventObjectGenerate.EventObjectClear();
        roomGenerate.EndInit();
        DungeonCanvas.SetActive(false);
        //playerMove.gameObject.SetActive(false);
    }

    public void TutorialInit()
    {
        tutorialManager = GameManager.Manager.TutoManager;
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

            // ���� �ҷ��� �� �����Ͱ� ���� ��
            if (Vars.UserData.AllDungeonData.Count <= 0)
                GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);

            curDungeonIndex = Vars.UserData.curDungeonIndex;
            startIndex = Vars.UserData.dungeonStartIdx;
            lastIndex = Vars.UserData.dungeonLastIdx;
            // ù ���̺꿡�� �̰ž����� ����
            if (Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData == null)
            {
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
            }

            // ����ġ�ų� ���� �����Ҷ� �÷��̾� ����� ��ġ ó������
            if (Vars.UserData.isDungeonReStart)
            {
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
                Vars.UserData.isDungeonReStart = false;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerBoyData.curRoomNumber = -1;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerGirlData.curRoomNumber = -1;
            }
            // Ŭ������ �� �ٽ� ���ƿö�, �÷��̾� ��ġ�� �� ù ��ġ��
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
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
            // TODO: �ӽ�! ����� �س�����
            GameManager.Manager.State = GameState.Dungeon;
        }

        roomTool = new RoomTool();
        if (dungeonSystemData.curDungeonRoomData != null && Vars.UserData.mainTutorial == MainTutorialStage.Clear)
            ConvertEventDataType();
        DungeonRoomSetting();
    }

    // �������� �ϼ��� �Ŀ� ������ ���� �� ����
    private void DungeonRoomSetting()
    {
        if (Vars.UserData.mainTutorial == MainTutorialStage.Camp)
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

    // �渶�� ��ġ���ִ� Ʈ���� �ߵ��Ҷ� ����, �� �ٲ�
    public void ChangeRoomEvent(bool isRoomEnd, bool isGoForward)
    {
        Debug.Log(dungeonSystemData.curDungeonRoomData.roomIdx);
        if (isRoomEnd)
        {
            if (Vars.UserData.mainTutorial != MainTutorialStage.Clear
                && Vars.UserData.mainTutorial != MainTutorialStage.Camp)
            {
                TutorialStart();
                GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial(false);
            }
            eventObjectGenerate.EventObjectClear();

            if (dungeonSystemData.curDungeonRoomData.nextRoomIdx == -1)
            {
                if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
                {
                    dungeonSystemData = null;
                    GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial(false);
                    // ���ϰ�, ���׹̳ʰ�, HP�� ��� Ʃ�丮�󿡼� ����� ���� �ٽ� �ʱ�ȭ �ؾߵ�
                }
                else
                {
                    Vars.UserData.WorldMapPlayerData.isClear = true;
                    Vars.UserData.isDungeonClear = true;
                }
                GameManager.Manager.LoadScene(GameScene.World);
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
            if (isGoForward)
            {
                if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
                {
                    TutorialStart();
                    GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial(false);
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

        if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
        {
            Vars.UserData.tutorialDungeonData = dungeonSystemData;
        }
        else
        {
            Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
        }
        // TODO: �̰� ��Ǯ�� curDungeonData ���� ���� ����
        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }

    // ���̺� �� �ε��Ҷ� �̺�Ʈ Ÿ���� �θ�Ÿ������ �ٲ���ִ°� �ٽ� ����
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
            // TODO : ���� eventList[i]�� EventDataŸ���̶�, ÷ �����Ҷ� offsetBasePos�� gathring�� ���־��ִ� �� ���ܼ� ��� 000��ġ�� �����Ǵ� ����, ���� ������������ �ɾ�־��� �ʿ�����
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
