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
    Empty = 0b_0000_0000,
    Battle = 0b_0000_0001,
    Gathering = 0b_0000_0010,
    Hunt = 0b_0000_0100,
    RandomIncount = 0b_0000_1000,
    SubStory = 0b_0001_0000,
    Count = 0b_0010_0000,
}
[Serializable]
public class DungeonRoom
{
    private bool isCheck;
    private Vector2 pos;
    private DunGeonRoomType roomType;

    public List<DunGeonEvent> eventList = new List<DunGeonEvent>();
    
    public DungeonRoom nextRoom;
    // ���� �̻��
    public DungeonRoom beforeRoom;

    public int roomIdx;
    public int nextRoomIdx;
    public int nextRoadCount = 0;
    // ���� �̻��
    public int beforeRoomIdx;

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
                if (eventList.FindIndex(x => x == DunGeonEvent.Hunt) != -1
                    || eventList.FindIndex(x => x == DunGeonEvent.RandomIncount) != -1
                    || eventList.FindIndex(x => x == DunGeonEvent.SubStory) != -1)
                    return;
                break;
            case DunGeonEvent.Hunt:
            case DunGeonEvent.RandomIncount:
            case DunGeonEvent.SubStory:
                if (eventList.FindIndex(x => x == DunGeonEvent.Battle) != -1)
                    return;
                break;
        }
        eventList.Add(eventType);

    }

    public bool CheckEvent(DunGeonEvent evnetType)
    {

        if (eventList.FindIndex(x => x == evnetType) != -1)
            return true;

        return false;
    }
}

public static class DunGeonRoomSetting
{
    public static void RoomEventSet(DungeonRoom room)
    {
        // �Է¹��� �濡 1~2�� ������ �̺�Ʈ�� �־��ְ�
        // �� �̺�Ʈ�� ���ü� �ִ� �̺�Ʈ Ÿ���� Ȯ�������� 1�� ��� �־��ش�
        var rndEvnetCount = UnityEngine.Random.Range(1, 3);
        var tempPercent = new List<int> { 20, 20, 20, 20, 20 };
        if (room.RoomType == DunGeonRoomType.MainRoom)
        {
            for (int i = 0; i < rndEvnetCount; i++)
            {
                var picEvent = EventPic(tempPercent);
                if (room.CheckEvent(picEvent))
                {
                    i--;
                    continue;
                }
                room.SetEvent(picEvent);
            }
        }
        else
        {
            for (int i = 0; i < rndEvnetCount; i++)
            {
                var SubEvent = EventPic(tempPercent);
                while (SubEvent == DunGeonEvent.Battle)
                {
                    SubEvent = EventPic(tempPercent);
                }
                if (room.CheckEvent(SubEvent))
                {
                    i--;
                    continue;
                }
                room.SetEvent(SubEvent);
            }
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


        var rnd = UnityEngine.Random.Range(0, 130);

        // i�� �̺�Ʈenum ��ȸ����, j�� Ȯ�� ����Ʈ �ε�����
        for (int i = 1, j = 0; i != (int)DunGeonEvent.Count;)
        {
            if (j > eventP.Count) break;
            // empty�� ���� �� 30���� Ȯ��
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
    public static void DungeonLink(DungeonRoom[] dungeonList)
    {
        for (int i = 0; i < dungeonList.Length; i++)
        {
            if(dungeonList[i].IsCheck)
            {
                var nextIdx = dungeonList[i].nextRoomIdx;
                if (nextIdx != -1 && dungeonList[nextIdx].IsCheck == true)
                {
                    dungeonList[i].nextRoom = dungeonList[nextIdx];
                }
            }
        }
    }
    // ������� ����� ������ ����Ʈ�� ����, �������� ������� �����ϱ� ( ���� �̻�� )
    public static void DungeonReverseLink(List<DungeonRoom> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                return;
            list[i].beforeRoom = list[i - 1];
            list[i].beforeRoomIdx = list[i - 1].roomIdx;
        }
    }
    // ���۹� �Է¹ޱ�, road���� ī��Ʈ, ������ �������� ������� ����Ʈ�� ���
    public static void DungeonRoadCount(DungeonRoom dungeonRoom, List<DungeonRoom> list)
    {
        int roadCount = 0;
        while(dungeonRoom.nextRoomIdx != -1)
        {
            if (dungeonRoom.RoomType == DunGeonRoomType.MainRoom)
            {
                var tempRoom = dungeonRoom.nextRoom;
                while(tempRoom.RoomType != DunGeonRoomType.MainRoom)
                {
                    roadCount++;
                    tempRoom = tempRoom.nextRoom;
                }
                dungeonRoom.nextRoadCount = roadCount;
                roadCount = 0;
            }
            list.Add(dungeonRoom);
            dungeonRoom = dungeonRoom.nextRoom;
        }
        list.Add(dungeonRoom);
    }
}


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