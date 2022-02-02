using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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
    private float distance = 2f;
    private int col = 20;

    private int roomCount = 4;
    private int remainMainRoom;

    public DirectionInho direction;

    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    //�׽�Ʈ�� (���� Ȯ�ο�)
    //public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    public void DungeonGenerate(int range , DungeonRoom[] mapArrayData, UnityAction action)
    {
        Debug.Log(range);
        switch (range)
        {
            case 1:
                roomCount = 4;
                break;
            case 2:
                roomCount = 6;
                break;
            case 3:
                roomCount = 8;
                break;
        }
        if(mapArrayData == null)
        {
            StartCoroutine(MapCorutine(action));
        }
        else
        {
            dungeonRoomArray = mapArrayData;
            action?.Invoke();
        }
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray = dungeonRoomArray;
        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }
    public void DungeonEventGenerate(DungeonRoom[] dungeonArray)
    {
        int curIdx = Vars.UserData.dungeonStartIdx;
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
                    curRoom.gatheringCount = 2/*Random.Range(0, 3)*/;
                    for (int i = 0; i < curRoom.gatheringCount; i++)
                    {
                        var gatheringData1 = new GatheringData();
                        gatheringData1.eventType = DunGeonEvent.Gathering;
                        gatheringData1.offSetBasePos = (-3 + (i * 3));
                        gatheringData1.roomIndex = curRoom.roomIdx;
                        gatheringData1.gatheringtype = (GatheringObjectType)Random.Range(0, 4);
                        eventData.Add(gatheringData1);
                    }
                }
                else
                {
                    var gatheringData = new GatheringData();
                    gatheringData.eventType = DunGeonEvent.Gathering;
                    gatheringData.roomIndex = curRoom.roomIdx;
                    gatheringData.gatheringtype =(GatheringObjectType)Random.Range(0, 4);
                    curRoom.gatheringCount = 1;
                    gatheringData.offSetBasePos = 2;
                    eventData.Add(gatheringData);
                }
                break;
            case DunGeonEvent.Hunt:
                var huntingData = new HuntingData();
                huntingData.eventType = DunGeonEvent.Hunt;
                huntingData.roomIndex = curRoom.roomIdx;
                eventData.Add(huntingData);
                break;
            case DunGeonEvent.RandomIncount:
                var randomData = new RandomIncountData();
                randomData.eventType = DunGeonEvent.RandomIncount;
                randomData.roomIndex = curRoom.roomIdx;
                eventData.Add(randomData);
                RandomEventManager.Instance.CreateRandomEvent(randomData);
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
        while (remainMainRoom > 0)
        {
            // �ٽ� �ʱ�ȭ
            MapInit();
            TestMapSet(Vars.UserData.dungeonStartIdx, -1, DirectionInho.Right, 0);
            yield return null;
        }


        DunGeonRoomSetting.DungeonRoadCount(dungeonRoomArray[Vars.UserData.dungeonStartIdx], dungeonRoomArray);
        DunGeonRoomSetting.DungeonPathRoomCountSet(dungeonRoomArray[Vars.UserData.dungeonStartIdx], dungeonRoomArray);
        DungeonEventGenerate(dungeonRoomArray);

        //var list = dungeonRoomArray.Where(x => x.eventList.FindIndex(y => y == DunGeonEvent.RandomIncount) != -1).ToList();

        //// TODO : �ڷ�ƾ�ȿ� �ڷ�ƾ..
        //for (int i = 0; i < list.Count;)
        //{
        //    yield return StartCoroutine(RandomEventManager.Instance.CreateRandomEvent(list[i].randomEventData));
        //    i++;
        //}
        action?.Invoke();

        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }
    public void MapInit()
    {
        for (int i = 0; i < dungeonRoomArray.Length; i++)
        {
            dungeonRoomArray[i] = new DungeonRoom();
            dungeonRoomArray[i].IsCheck = false;
            if (i == 0)
            {
                dungeonRoomArray[i].Pos = new Vector2(-500f, -1200f);
            }
            else
            {
                if(i==100)
                {
                    int a = 100;
                }
                var row = i % col;
                var colum = i / col;
                dungeonRoomArray[i].Pos = new Vector2(-500f + row * distance * 15f, -1200f + colum * distance * 15f);
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
                if (rnd == 1) // ���ʹ��⵵ �׳� ����ó��
                    rnd = (int)oppsiteDir(lastDir);
            }
            var curDir = (DirectionInho)rnd;
            // ���� ��� ���� ����
            var rndCount = Random.Range(2, 6);

            var nextId = NextRoomId(curIdx, curDir);
            
            dungeonRoomArray[curIdx].nextRoomIdx = nextId;
            // ������ ��
            if (remainMainRoom <= 0)
            {
                dungeonRoomArray[curIdx].nextRoomIdx = -1;
                dungeonRoomArray[curIdx].eventList.Clear();
                dungeonRoomArray[curIdx].eventList.Add(DunGeonEvent.Battle);
                return;
            }
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
                    if (rnd == 1) // ���ʹ��⵵ �׳� ����ó��
                        rnd = (int)oppsiteDir(lastDir);
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

    public DirectionInho oppsiteDir(DirectionInho dir)
    {
        var direction = dir;
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
