using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum DirectionInho
{
    Right,
    Left,
    Top,
    Bottom,
    Count,
}

public class DunGeonMapGenerate : MonoBehaviour
{
    //private DungeonSystem dungeonSystem;

    private float distance = 2f;
    private int startIdx = 100;
    private int col = 20;

    private int roomCount = 6;
    private int remainMainRoom;

    public DirectionInho direction;

    //public RoomObject mainRoomPrefab;
    //public RoomObject roadPrefab;
    //public GameObject mapPos;

    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    //public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();

    //�׽�Ʈ�� (���� Ȯ�ο�)
    //public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    public void Init(DungeonSystem system)
    {
        //if(dungeonSystem == null)
        //    dungeonSystem = system;
    }

    public void DungeonGenerate(DungeonRoom[] mapArrayData, UnityAction action)
    {
        if(mapArrayData == null)
        {
            StartCoroutine(MapCorutine(action));
        }
        else
        {
            dungeonRoomArray = mapArrayData;
            //CreateMapObject();
            //dungeonSystem.DungeonSystemData.dungeonRoomObjectList = dungeonRoomObjectList;
            //action?.Invoke();
        }
        Vars.UserData.DungeonMapData = dungeonRoomArray;
        Vars.UserData.CurAllDungeonData.dungeonRoomArray = dungeonRoomArray;
        Vars.UserData.dungeonStartIdx = startIdx;
    }
    public void DungeonEventGenerate(DungeonRoom[] dungeonArray)
    {
        int curIdx = startIdx;
        while(dungeonArray[curIdx].nextRoomIdx != -1)
        {
            // ������� �̺�Ʈ ����Ʈ (1��~2��) ���鼭 �̺�Ʈ ������Ʈ ���� ��
            for (int i = 0; i < dungeonArray[curIdx].eventList.Count; i++)
            {
                EventDataInit(dungeonArray[curIdx], dungeonArray[curIdx].eventObjDataList, dungeonArray[curIdx].eventList[i]);
            }
            curIdx = dungeonArray[curIdx].nextRoomIdx;
        }
    }

    public void EventDataInit(DungeonRoom curRoom ,List<EventData> eventData, DunGeonEvent evnetTypeList)
    {
        switch (evnetTypeList)
        {
            case DunGeonEvent.Empty:
                break;
            case DunGeonEvent.Battle:
                var battleData = new BattleData();
                battleData.eventType = DunGeonEvent.Battle;
                battleData.roomIndex = curRoom.roomIdx;
                eventData.Add(battleData);
                break;
            case DunGeonEvent.Gathering:
                if (curRoom.RoomType == DunGeonRoomType.MainRoom)
                {
                    curRoom.gatheringCount = Random.Range(0, 3);
                    for (int i = 0; i < curRoom.gatheringCount; i++)
                    {
                        var gatheringData1 = new GatheringData();
                        gatheringData1.eventType = DunGeonEvent.Gathering;
                        gatheringData1.offSetBasePos = (-3 + (i * 3));
                        gatheringData1.roomIndex = curRoom.roomIdx;
                        eventData.Add(gatheringData1);
                    }
                }
                else
                {
                    var gatheringData = new GatheringData();
                    gatheringData.eventType = DunGeonEvent.Gathering;
                    gatheringData.roomIndex = curRoom.roomIdx;
                    curRoom.gatheringCount = 1;
                    gatheringData.offSetBasePos = 2;
                }
                break;
            case DunGeonEvent.Hunt:
                var huntingData = new HuntingData();
                huntingData.eventType = DunGeonEvent.Hunt;
                huntingData.roomIndex = curRoom.roomIdx;
                eventData.Add(huntingData);
                break;
            case DunGeonEvent.RandomIncount:
                break;
            case DunGeonEvent.SubStory:
                break;
            case DunGeonEvent.Count:
                break;
        }
    }

    IEnumerator MapCorutine(UnityAction action)
    {
        MapInit();
        //dungeonSystem.DungeonSystemData.dungeonStartIdx = startIdx;
        while (remainMainRoom > 0)
        {
            // �ٽ� �ʱ�ȭ
            MapInit();
            TestMapSet(startIdx, -1, DirectionInho.Right, 0);
            yield return null;
        }
        DunGeonRoomSetting.DungeonRoadCount(dungeonRoomArray[startIdx], dungeonRoomArray);
        DunGeonRoomSetting.DungeonPathRoomCountSet(dungeonRoomArray[startIdx], dungeonRoomArray);
        //DunGeonRoomSetting.DungeonLink(dungeonRoomArray, dungeonRoomList);

        DungeonEventGenerate(dungeonRoomArray);
        //DunGeonRoomSetting.DungeonReverseLink(dungeonRoomList);
        //CreateMapObject();
        //dungeonSystem.DungeonSystemData.dungeonRoomArray = dungeonRoomArray;
        //dungeonSystem.DungeonSystemData.dungeonRoomObjectList = dungeonRoomObjectList;
        action?.Invoke();
    }

    //public void OnGUI()
    //{
    //    if (GUILayout.Button("reStart"))
    //    {
    //        SceneManager.LoadScene(0);
    //    }
    //    if (GUILayout.Button("SaveMap"))
    //    {
    //        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.DungeonMap);
    //    }
    //    if (GUILayout.Button("LoadMap"))
    //    {
    //        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.DungeonMap);
    //        dungeonRoomArray = Vars.UserData.DungeonMapData;
    //        DunGeonRoomSetting.DungeonRoadCount(dungeonRoomArray[startId], dungeonRoomList);
    //        CreateMapObject();
    //    }
    //    if(GUILayout.Button("Success"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = true;
    //        SceneManager.LoadScene("WorldMap");
    //    }
    //    if (GUILayout.Button("Run"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = false;
    //        SceneManager.LoadScene("WorldMap");
    //    }
    //}

    public void MapInit()
    {
        for (int i = 0; i < dungeonRoomArray.Length; i++)
        {
            dungeonRoomArray[i] = new DungeonRoom();
            dungeonRoomArray[i].IsCheck = false;
            if (i == 0)
            {
                dungeonRoomArray[i].Pos = Vector2.zero;
            }
            else
            {
                var row = i % col;
                var colum = i / col;
                dungeonRoomArray[i].Pos = new Vector2(row * distance, colum * distance);
            }
        }
        remainMainRoom = roomCount;
    }

    public void TestMapSet(int curIdx,int beforeIdx, DirectionInho lastDir, int roadCount)
    {
        if (remainMainRoom <= 0)
            return;

        if (!RoomException(curIdx))
            return;

        // ���ι� ����
        if (roadCount <= 0)
        {
            dungeonRoomArray[curIdx].IsCheck = true;
            dungeonRoomArray[curIdx].RoomType = DunGeonRoomType.MainRoom;
            dungeonRoomArray[curIdx].roomIdx = curIdx;
            dungeonRoomArray[curIdx].beforeRoomIdx = beforeIdx;
            DunGeonRoomSetting.RoomEventSet(dungeonRoomArray[curIdx]);
            remainMainRoom--;

            // ���� ����
            var rnd = Random.Range(0, (int)DirectionInho.Count);
            while (rnd == (int)oppsiteDir(lastDir)) // �Դ��� �ǵ��ư��� ������ �׳� ���⼭ ����ó��
            {
                rnd = Random.Range(0, (int)DirectionInho.Count);
            }
            var curDir = (DirectionInho)rnd;
            // ���� ��� ���� ����
            var rndCount = Random.Range(2, 6);

            var nextId = NextRoomId(curIdx, curDir);
            
            dungeonRoomArray[curIdx].nextRoomIdx = nextId;
            if (remainMainRoom <= 0)
                dungeonRoomArray[curIdx].nextRoomIdx = -1;
            TestMapSet(nextId, curIdx, curDir, rndCount);
        }
        // ��� ����
        else
        {
            roadCount--;
            dungeonRoomArray[curIdx].IsCheck = true;
            dungeonRoomArray[curIdx].RoomType = DunGeonRoomType.SubRoom;
            dungeonRoomArray[curIdx].roomIdx = curIdx;
            dungeonRoomArray[curIdx].beforeRoomIdx = beforeIdx;
            DunGeonRoomSetting.RoomEventSet(dungeonRoomArray[curIdx]);

            var perCent = Random.Range(0, 10);
            // ���� ����� ���� ����, �� ����
            var curDir = lastDir;
            var nextId = NextRoomId(curIdx, curDir);
            // Ȯ���� ���� ����
            if (perCent > 7)
            {
                var rnd = Random.Range(0, (int)DirectionInho.Count);
                while (rnd == (int)oppsiteDir(lastDir))
                {
                    rnd = Random.Range(0, (int)DirectionInho.Count);
                }
                curDir = (DirectionInho)rnd;
                nextId = NextRoomId(curIdx, curDir);
            }
            dungeonRoomArray[curIdx].nextRoomIdx = nextId;
            TestMapSet(nextId, curIdx, curDir, roadCount);
        }
        return;
    }

    public bool RoomException(int curId)
    {
        if (curId == -1)
            return false;

        if (dungeonRoomArray[curId].IsCheck == true)
            return false;

        return true;
    }

    //public string GetText(DungeonRoom roomInfo)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    for (int i = 0; i < roomInfo.eventList.Count; i++)
    //    {
    //        sb.Append(roomInfo.eventList[i].ToString());
    //        sb.Append("\n");
    //    }
    //    return sb.ToString();
    //}

    //public void CreateMapObject()
    //{
    //    int curIdx = startIdx;
    //    while(dungeonRoomArray[curIdx].nextRoomIdx != -1)
    //    {
    //        var room = dungeonRoomArray[curIdx];
    //        if (room.RoomType == DunGeonRoomType.MainRoom)
    //        {
    //            var obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
    //                 , Quaternion.identity, mapPos.transform);
    //            var objectInfo = obj.GetComponent<RoomObject>();
    //            objectInfo.text.SetText(GetText(room));
    //            objectInfo.roomIdx = room.roomIdx;
    //            dungeonRoomObjectList.Add(obj);
    //        }
    //        else
    //        {
    //            var obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
    //            , Quaternion.identity, mapPos.transform);
    //            var objectInfo = obj.GetComponent<RoomObject>();
    //            objectInfo.text.SetText(GetText(room));
    //            objectInfo.roomIdx = room.roomIdx;
    //            dungeonRoomObjectList.Add(obj);
    //        }
    //        curIdx = dungeonRoomArray[curIdx].nextRoomIdx;
    //    }

    //    mapPos.transform.position = mapPos.transform.position + new Vector3(0f, 30f, 0f);
    //}
    public DirectionInho oppsiteDir(DirectionInho dir)
    {
        var direction = DirectionInho.Count;

        switch (direction)
        {
            case DirectionInho.Right:
                direction = DirectionInho.Left;
                break;
            case DirectionInho.Left:
                direction = DirectionInho.Right;
                break;
            case DirectionInho.Top:
                direction = DirectionInho.Bottom;
                break;
            case DirectionInho.Bottom:
                direction = DirectionInho.Top;
                break;
            case DirectionInho.Count:
                break;
        }

        return direction;
    }

    public int NextRoomId(int currentId, DirectionInho dir)
    {
        int result = -1;
        switch (dir)
        {
            case DirectionInho.Right:
                if (currentId % col == col - 1)
                {
                    break;
                }
                result = currentId + 1;
                break;
            case DirectionInho.Left:
                //if (currentId % col == 0)  �������� ������ (��ȹ ��û)
                //{
                //    break;
                //}
                //result = currentId - 1;
                break;
            case DirectionInho.Top:
                if (currentId < col)
                {
                    break;
                }
                result = currentId - col;
                break;
            case DirectionInho.Bottom:
                if (col * (col - 1) <= currentId && currentId < col * col)
                {
                    break;
                }
                result = currentId + col;
                break;
        }
        if (result < -1)
        {
            Debug.Log("d");
        }
        return result;
    }
}


// id�� 2���� �迭 ����,  �˻�����
// ��ĭ = ���� id - ��ü col / �Ʒ�ĭ = ���� id + ��ü col
// ��ĭ = ���� id - 1 / ����ĭ = ���� id + 1

// �����ڸ� �Ǵ�(nũ�� �簢��) ������ = id < n
// �Ʒ����� =  n*(n-1) <= id < n*n
// �޶��� = id % n == 0 ( id�� 0�ϋ��� ����ó�� )
// �������� = id % n == n-1 ( id�� 0�ϋ��� ����ó�� )

// ��ġ��ǥ = ���� position�������� ���⿡ ���� x��ǥ +-, z ��ǥ +-

// �ӽ÷� �켱���� ������ �ð���� ����

// ���簢 ���� id �˻�,  createId, 
// createid - col*2 - 2 ~ createid - col*2 + 2 
// createid - col*1 - 2 ~ createid - col*2 + 2 
// createid - col*0 - 2 ~ createid - col*2 + 2 
// createid - col*-1 - 2 ~ createid - col*2 + 2 
// createid - col*-2 - 2 ~ createid - col*2 + 2 



//public void RandomMapInit()
//{ 
//    while (remainRoom > 0)
//    {
//        CreateRoom(startId, 0, Direction.Count);
//    }
//}

//public int CreateRoom(int curId, int count, Direction dir)
//{
//    // ��ŸƮ Direction�� �ƴҶ�
//    if(dir != Direction.Count)
//    {
//        if (remainRoom < 0)
//        {
//            return 0;
//        }
//        if(curId == -1)
//        {
//            return -1;
//        }
//        Debug.Log(curId);
//        if(DungeonRoomList[curId].IsCheck == true)
//        {
//            return -1;
//        }
//    }

//    // �� �������϶�
//    if (count > 0)
//    {
//        count--;
//        DungeonRoomList[curId].IsCheck = true;
//        DungeonRoomList[curId].IsMain = false;

//        var nextid = NextRoomId(curId, dir);
//        CreateRoom(nextid,count, dir);
//    }
//    // ���ι� �������϶�
//    else
//    {
//        DungeonRoomList[curId].IsCheck = true;
//        DungeonRoomList[curId].IsMain = true;

//        //if (curId == startId)
//        //{
//        //}
//        //else
//        //{
//        //}

//        remainRoom--;

//        Direction setDir;
//        bool noneDir = true;

//        // 50% Ȯ���� �ش� �������� �� �������� ���ϱ�
//        if(Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Right;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }
//        if (Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Left;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }
//        if (Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Bottom;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }
//        if (Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Top;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }

//        // 4���� �� ������ ��� �������� ��
//        if(noneDir)
//        {
//            for (int i = 0; i < (int)Direction.Count; i++)
//            {
//                // ���ݱ����� �������� �����ʴ��϶�? -> ���⼭ ���� ���°� 
//                if(dir != (Direction)i)
//                {
//                    var nextIdx = NextRoomId(curId, (Direction)i);
//                    CreateRoom(nextIdx, roadCount, (Direction)i);
//                    return 1;
//                }
//            }
//        }
//    }
//    return 1;
//}



//StringBuilder sb = new StringBuilder();
//for (int j = 1; j < (int)DunGeonEvent.Count;)
//{
//    DunGeonEvent curEvent = DunGeonEvent.Empty;
//    if(((DunGeonEvent)j & DungeonRoomList[i].GetEvent()) != 0)
//    {
//        curEvent = (DunGeonEvent)j;
//    }
//    switch (curEvent)
//    {
//        case DunGeonEvent.Battle:
//            sb.Append($"{DunGeonEvent.Battle.ToString()} + ");
//            break;
//        case DunGeonEvent.Gathering:
//            sb.Append($"{DunGeonEvent.Gathering.ToString()} + ");
//            break;
//        case DunGeonEvent.Hunt:
//            sb.Append($"{DunGeonEvent.Hunt.ToString()} + ");
//            break;
//        case DunGeonEvent.RandomIncount:
//            sb.Append($"{DunGeonEvent.RandomIncount.ToString()} + ");
//            break;
//        case DunGeonEvent.SubStory:
//            sb.Append($"{DunGeonEvent.SubStory.ToString()} + ");
//            break;
//    }

//    text.text.SetText(sb);
//    j <<= j;
//    exeptemp++;
//    if(exeptemp > 100)
//    {
//        Debug.Log("����");
//        break;
//    }    
//}


// �ʿ��������?
//public void MapMarking()
//{
//    for (int i = 0; i < DungeonRoomList.Length; i++)
//    {
//        if (testMap[i] == 3)
//        {
//            DungeonRoomList[i].IsCheck = true;
//            DungeonRoomList[i].RoomType = DunGeonRoomType.MainRoom;
//            DunGeonRoomSetting.RoomEventSet(DungeonRoomList[i]);
//        }
//        else if (testMap[i] == 4)
//        {
//            DungeonRoomList[i].IsCheck = true;
//            DungeonRoomList[i].RoomType = DunGeonRoomType.SubRoom;
//            DunGeonRoomSetting.RoomEventSet(DungeonRoomList[i]);
//        }
//    }
//}

//public void showMapTest()
//{
//    while (remainRoom > 0)
//    {
//        // �ٽ� �ʱ�ȭ
//        MapInit();
//        TestMapSet(startId, DirectionInho.Right, 0);
//    }

//    MapMarking();
//    CreateMapObject();
//}

//MapInit();
//showMapTest();
//RandomMapInit();
//showMap();