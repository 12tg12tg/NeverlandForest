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

    private DunGeonEvent eventCase;
    private int battleSet = 0b_1110_0011;
    private int noBattleSet = 0b_1111_1110;

    public DungeonRoom nextRoom;
    public int nextRoomIdx;
    public int nextRoadCount = 0;
    //public List<int> nextRoomIndex = new List<int>();

    public DungeonRoom() { }
    public DungeonRoom(DungeonRoom room)
    {
        isCheck = room.isCheck;
        pos = room.pos;
        roomType = room.roomType;
        eventCase = room.eventCase;
        nextRoom = room.nextRoom;
        nextRoomIdx = room.nextRoomIdx;
        nextRoadCount = room.nextRoadCount;
    }

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
                eventCase &= (DunGeonEvent)battleSet;
                break;
            case DunGeonEvent.Hunt:
            case DunGeonEvent.RandomIncount:
            case DunGeonEvent.SubStory:
                eventCase &= (DunGeonEvent)noBattleSet;
                break;
        }

        eventCase |= eventType;
    }
    public DunGeonEvent GetEvent()
    {
        return eventCase;
    }
    public void CheckEvent(DunGeonEvent evnetType)
    {
        if((eventCase & evnetType) != 0)
        {
            Debug.Log("이벤트가 있다");
        }
        else
        {
            Debug.Log("이벤트가 없다");
        }
    }
}

public static class DunGeonRoomSetting
{
    public static void RoomEventSet(DungeonRoom room)
    {
        // 입력받은 방에 1~2사이의 이벤트를 넣어주고
        // 각 이벤트를 나올수 있는 이벤트 타입중 확률적으로 1개 골라서 넣어준다
        var rndEvnetCount = UnityEngine.Random.Range(1, 3);
        var tempPercent = new List<int> { 20, 20, 20, 20, 20 };
        if (room.RoomType == DunGeonRoomType.MainRoom)
        {
            for (int i = 0; i < rndEvnetCount; i++)
            {
                room.SetEvent(eventPic(tempPercent));
            }
        }
        else
        {
            for (int i = 0; i < rndEvnetCount; i++)
            {
                var SubEvent = eventPic(tempPercent);
                while (SubEvent == DunGeonEvent.Battle)
                {
                    SubEvent = eventPic(tempPercent);
                }
                room.SetEvent(SubEvent);
            }
        }
    }

    // 받는 리스트에는 순서대로 각 이벤트 확률이 들어있다고 가정
    public static DunGeonEvent eventPic(List<int> percentage)
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


        var rnd = UnityEngine.Random.Range(0, 130);

        // i는 이벤트enum 순회느낌, j는 확률 리스트 인덱스용
        for (int i = 1, j = 0; i != (int)DunGeonEvent.Count;)
        {
            // 혹시몰라서 무한방지
            if (j > eventP.Count) break;

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

    public static void DungeonLink(DungeonRoom[] dungeonList)
    {
        int count = 0;
        for (int i = 0; i < dungeonList.Length; i++)
        {
            if(dungeonList[i].IsCheck)
            {
                count++;
                var nextIdx = dungeonList[i].nextRoomIdx;
                if (nextIdx != -1 && dungeonList[nextIdx].IsCheck == true)
                {
                    dungeonList[i].nextRoom = dungeonList[nextIdx];
                }
            }
        }
    }
    // 시작방 입력받기, road개수 카운트 메소드
    public static void DungeonLink(DungeonRoom dungeonRoom, List<DungeonRoom> list)
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
