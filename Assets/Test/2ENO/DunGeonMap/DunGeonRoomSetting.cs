using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DunGeonRoomType
{
    MainRoom,
    SubRoom,
}

[Flags]
public enum DunGeonEvent
{
    SubStory = 0b_0000_0000,
    Battle = 0b_0000_0001,
    Gathering = 0b_0000_0010,
    Hunt = 0b_0000_0100,
    RandomIncount = 0b_0000_1000,
    Empty = 0b_0001_0000,
    Count = 0b_0010_0000,
}

[Serializable]
public class DungeonRoom
{
    // �� ���� ����
    private bool isCheck;
    private Vector2 pos;
    private DunGeonRoomType roomType;
    
    public int roomIdx;
    public int nextRoomIdx;
    public int beforeRoomIdx;
    public int roadCount;

    public List<DunGeonEvent> eventList = new List<DunGeonEvent>();
    // ���� �̺�ƮŸ�� ����Ʈ�� ���� �����Ǿ��� ������Ʈ�� ����
    public List<EventData> eventObjDataList = new List<EventData>();
    public int gatheringCount;

    public DunGeonRoomType RoomType
    {
        set => roomType = value;
        get => roomType;
    }
    public bool IsCheck
    {
        set => isCheck = value;
        get => isCheck;
    }
    public Vector2 Pos
    {
        set => pos = value;
        get => pos;
    }

    public void SetEvent(DunGeonEvent eventType)
    {
        switch (eventType)
        {
            case DunGeonEvent.Battle:
            case DunGeonEvent.Hunt:
            case DunGeonEvent.RandomIncount:
            case DunGeonEvent.SubStory:
                break;
        }
        eventList.Add(eventType);
    }
    public void UseEvent(DunGeonEvent evnetType)
    {
        var index = eventList.FindIndex(x => x == evnetType);
        if (index != -1)
        {
            eventList.RemoveAt(index);
        }
    }
}

public static class DunGeonRoomSetting
{
    public static void RoomEventSet(DungeonRoom room)
    {
        // �Է¹��� �濡 1~2�� ������ �̺�Ʈ�� �־��ְ�
        // �� �̺�Ʈ�� ���ü� �ִ� �̺�Ʈ Ÿ���� Ȯ�������� 1�� ��� �־��ش�


        //��Ʋ / ä�� / ��� / ������ī��Ʈ / ���
        var tempPercent = new List<int> { 15, 25, 25, 25, 10 };
        if (room.RoomType == DunGeonRoomType.MainRoom)
        {
            room.SetEvent(DunGeonEvent.Gathering);

            var picEvent = EventPic(tempPercent);
            while(picEvent == DunGeonEvent.Gathering)
            {
                picEvent = EventPic(tempPercent);
            }
            room.SetEvent(picEvent);
        }
        else
        {
            var SubEvent = EventPic(tempPercent);
            //while (SubEvent == DunGeonEvent.Battle)
            //{
            //    SubEvent = EventPic(tempPercent);
            //}
            room.SetEvent(SubEvent);
        }
    }

    // �޴� ����Ʈ���� ������� �� �̺�Ʈ Ȯ���� ����ִٰ� ����
    public static DunGeonEvent EventPic(List<int> percentage)
    {
        // �ϴ� �� �̺�Ʈ �߻� Ȯ�� = 20����
        List<int> eventP = new List<int>();
        int sum = 0;
        for (int i = 0; i < percentage.Count; i++)
        {
            sum += percentage[i];
            eventP.Add(sum);
        }
        DunGeonEvent eventType = DunGeonEvent.Empty;

        var rnd = UnityEngine.Random.Range(0, 101);

        // i�� �̺�Ʈenum ��ȸ����, j�� Ȯ�� ����Ʈ �ε�����
        for (int i = 1, j = 0; i != (int)DunGeonEvent.Count;)
        {
            if (j > eventP.Count) break;
            // empty�� ���� �� 20���� Ȯ��
            if (rnd > 100)
            {
                eventType = DunGeonEvent.Empty;
                break;
            }
            if(rnd < eventP[j])
            {
                eventType = (DunGeonEvent)i;
                break;
            }
            i <<= 1;
            j++;
        }
        return eventType;
    }
    // ������� ����
    public static void DungeonLink(DungeonRoom[] dungeonList, List<DungeonRoom> list)
    {
        int curIdx = 100;
        while(dungeonList[curIdx].nextRoomIdx != -1)
        {
            list.Add(dungeonList[curIdx]);
            curIdx = dungeonList[curIdx].nextRoomIdx;
        }
        list.Add(dungeonList[curIdx]);
    }

    // ���۹� �Է¹ޱ�, road���� ī��Ʈ (���ι游!), ������ �������� ������� ����Ʈ�� ���
    public static void DungeonRoadCount(DungeonRoom dungeonRoom, DungeonRoom[] dungeonArray)
    {
        int curIdx = dungeonRoom.roomIdx;

        while(dungeonArray[curIdx].nextRoomIdx != -1)
        {
            int roadCount = 0;
            int tempCurIdx = dungeonArray[curIdx].nextRoomIdx;
            if (dungeonArray[curIdx].RoomType == DunGeonRoomType.MainRoom)
            {
                while (dungeonArray[tempCurIdx].RoomType != DunGeonRoomType.MainRoom)
                {
                    roadCount++;
                    tempCurIdx = dungeonArray[tempCurIdx].nextRoomIdx;
                }
            }
            dungeonArray[curIdx].roadCount = roadCount;
            curIdx = dungeonArray[curIdx].nextRoomIdx;
        }
    }
    // ���۹� �Է¹ޱ�, ���ι��� road������ ����� �̾����ִ� ����� road ���� Set
    public static void DungeonPathRoomCountSet(DungeonRoom dungeonRoom, DungeonRoom[] dungeonArray)
    {
        int roadCount;
        int curIdx = dungeonRoom.roomIdx;

        while (dungeonArray[curIdx].nextRoomIdx != -1)
        {
            if (dungeonArray[curIdx].RoomType == DunGeonRoomType.MainRoom)
            {
                roadCount = dungeonArray[curIdx].roadCount;
                curIdx = dungeonArray[curIdx].nextRoomIdx;
                while (dungeonArray[curIdx].RoomType != DunGeonRoomType.MainRoom)
                {
                    dungeonArray[curIdx].roadCount = roadCount;
                    curIdx = dungeonArray[curIdx].nextRoomIdx;
                }
            }
        }
    }
}
//while(dungeonRoom.nextRoomIdx != -1)
//{
//    if (dungeonRoom.RoomType == DunGeonRoomType.MainRoom)
//    {
//        var tempRoom = dungeonRoom.nextRoom;
//        while(tempRoom.RoomType != DunGeonRoomType.MainRoom)
//        {
//            roadCount++;
//            tempRoom = tempRoom.nextRoom;
//        }
//        dungeonRoom.nextRoadCount = roadCount;
//        roadCount = 0;
//    }
//    list.Add(dungeonRoom);
//    dungeonRoom = dungeonRoom.nextRoom;
//}
//list.Add(dungeonRoom);


//private DunGeonEvent eventCase;

//private int eventCount;
//private int battleSet = 0b_1110_0011;
//private int noBattleSet = 0b_1111_1110;

//public DunGeonEvent EventCase
//{
//    get => eventCase;
//}
//public int EventCount
//{
//    set => eventCount = value;
//    get => eventCount;
//}

//if((eventCase & evnetType) != 0)
//{
//    return true;
//}
//return false;

//for (int i = 0; i < rndEvnetCount; i++)
//{
//    var picEvent = EventPic(tempPercent);
//    if (room.CheckEvent(picEvent))
//    {
//        i--;
//        continue;
//    }
//    room.SetEvent(picEvent);
//}

//for (int i = 0; i < 1; i++)
//{
//    if (room.CheckEvent(SubEvent))
//    {
//        i--;
//        continue;
//    }
//}