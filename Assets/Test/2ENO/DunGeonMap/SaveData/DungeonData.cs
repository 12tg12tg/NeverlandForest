using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public int dungeonStartIdx = -1;
    // 현재 던전크기 지정은 고정수치로 하는 느낌..
    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();

    public DungeonRoom curDungeonRoomData;
    //public RoomCtrl curRoomInstanceData;
    public PlayerDungeonUnitData curPlayerData = new PlayerDungeonUnitData();
}

public class DungeonMapSaveData_0 : SaveDataBase
{
    public List<List<DungeonRoom>> dungeonRoomList;
    public List<PlayerDungeonUnitData> curPlayerData;
    public List<Vector2> dungeonIndex;
    public Vector2 curDungeonIndex;
    public int curDungeonRoomIndex;
    
    public override SaveDataBase VersionUp()
    {
        return new DungeonMapSaveData_0();
    }
}

//public SerializeDictionary<Vector2, DungeonData> CurAllDungeonData;
//public List<DungeonRoom[]> dungeonRoomArray;