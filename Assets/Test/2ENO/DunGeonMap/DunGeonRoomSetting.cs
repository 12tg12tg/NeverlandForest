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
    // 방 생성 관련
    private bool isCheck;
    private Vector2 pos;
    private DunGeonRoomType roomType;
    
    public int roomIdx;
    public int nextRoomIdx;
    public int beforeRoomIdx;
    public int roadCount;


    // < 이벤트 생성 관련 > - 이벤트 생성 매니저를 따로 만들어서 그곳에 타입, 이벤트 id 입력하면 그것에 맞는 이벤트 반환?
    public List<DunGeonEvent> eventList = new List<DunGeonEvent>();

    // 현재 이벤트타입 리스트에 따라 생성되어질 오브젝트들 정보
    public int gatheringCount;
    public List<EventData> eventObjDataList = new List<EventData>();

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
        // 입력받은 방에 1~2개 사이의 이벤트를 넣어주고
        // 각 이벤트를 나올수 있는 이벤트 타입중 확률적으로 1개 골라서 넣어준다

        var tempPercent = new List<int> { 30, 40, 30, 5, 5 };
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
            while (SubEvent == DunGeonEvent.Battle)
            {
                SubEvent = EventPic(tempPercent);
            }
            room.SetEvent(SubEvent);
        }
    }

    // 받는 리스트에는 순서대로 각 이벤트 확률이 들어있다고 가정
    public static DunGeonEvent EventPic(List<int> percentage)
    {
        // 일단 각 이벤트 발생 확률 = 20프로
        List<int> eventP = new List<int>();
        int sum = 0;
        for (int i = 0; i < percentage.Count; i++)
        {
            sum += percentage[i];
            eventP.Add(sum);
        }
        DunGeonEvent eventType = DunGeonEvent.Empty;


        var rnd = UnityEngine.Random.Range(0, 110);

        // i는 이벤트enum 순회느낌, j는 확률 리스트 인덱스용
        for (int i = 1, j = 0; i != (int)DunGeonEvent.Count;)
        {
            if (j > eventP.Count) break;
            // empty는 현재 약 20프로 확률
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
    // 다음방과 연결
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

    // 시작방 입력받기, road개수 카운트, 생성된 던전맵을 순서대로 리스트에 담기
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
    // 시작방 입력받기, 메인방의 road개수를 참고로 이어져있는 서브방 road 개수 Set
    public static void DungeonPathRoomCountSet(DungeonRoom dungeonRoom, DungeonRoom[] dungeonArray)
    {
        int roadCount = 0;
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