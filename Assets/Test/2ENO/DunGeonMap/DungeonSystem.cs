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
    public MiniMapCamMove minimapCam;
    public GameObject DungeonCanvas;

    // ������ �����⿡�� �Űܿ;� �Ǵ� ��ɵ�
    public WorldMapMaker worldMap;

    // �ڵ� ���� ����ȭ �۾��� �ʿ��� �͵� - ������..
    private Vector2 curDungeonIndex;
    private int startIndex;

    private void Awake()
    {
        instance = this;
        EndInit();
        //Init();
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
        eventObjectGenerate.Init();
        playerMove.Init();
        minimapCam.Init();
        roomGenerate.Init();
        DungeonCanvas.SetActive(true);
        playerMove.gameObject.SetActive(true);
        dungeonPlayerGirl.gameObject.SetActive(false);
        dungeonPlayerBoy.gameObject.SetActive(false);

        if(Vars.UserData.isTutorialDungeon)
        {
            dungeonSystemData = Vars.UserData.tutorialDungeonData;
            startIndex = 0;
        }
        else
        {
            rndUi.Init();

            // ���� �ҷ��� �� �����Ͱ� ���� ��
            if (Vars.UserData.AllDungeonData.Count <= 0)
                GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);

            curDungeonIndex = Vars.UserData.curDungeonIndex;
            startIndex = Vars.UserData.dungeonStartIdx;
            //if (Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData == null)
            //{
            //    //Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = Vars.UserData.AllDungeonData[curDungeonIndex].dungeonRoomArray[startIndex];
            //}
            // ����ġ�ų� ���� �����Ҷ� �÷��̾� ����� ��ġ ó������
            if (Vars.UserData.dungeonReStart)
            {
                Vars.UserData.AllDungeonData[curDungeonIndex].curDungeonRoomData = null;
                Vars.UserData.dungeonReStart = false;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerBoyData.curRoomNumber = -1;
                Vars.UserData.AllDungeonData[curDungeonIndex].curPlayerGirlData.curRoomNumber = -1;
            }
            // Ŭ������ �� �ٽ� ���ƿö�, �÷��̾� ��ġ�� �� ù ��ġ��
            else if (!Vars.UserData.dungeonReStart)
            {
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
        if (dungeonSystemData.curDungeonRoomData != null)
            ConvertEventDataType();

        DungeonRoomSetting();
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

    // �������� �ϼ��� �Ŀ� ������ ���� �� ����
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

    // �渶�� ��ġ���ִ� Ʈ���� �ߵ��Ҷ� ����, �� �ٲ�
    public void ChangeRoomEvent(bool isRoomEnd, bool isGoForward)
    {
        if (isRoomEnd)
        {
            if (Vars.UserData.isTutorialDungeon)
            {
                TutorialDungeonStep.Instance.NextStep();
            }

            eventObjectGenerate.EventObjectClear();

            if (dungeonSystemData.curDungeonRoomData.nextRoomIdx == -1)
            {
                //Vars.UserData.curDungeonIndex = Vector2.zero;
                //Vars.UserData.AllDungeonData.Clear();
                //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);

                if (Vars.UserData.isTutorialDungeon)
                {
                    dungeonSystemData = null;
                    // ���ϰ�, ���׹̳ʰ�, HP�� ��� Ʃ�丮�󿡼� ����� ���� �ٽ� �ʱ�ȭ �ؾߵ�
                }
                else
                {
                    Vars.UserData.WorldMapPlayerData.isClear = true;
                }
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
            if (isGoForward)
            {
                if (Vars.UserData.isTutorialDungeon)
                {
                    TutorialDungeonStep.Instance.NextStep();
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

        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;
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
