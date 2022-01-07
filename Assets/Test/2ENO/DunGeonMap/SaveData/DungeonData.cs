using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public int dungeonStartIdx = -1;
    public DungeonRoom[] dungeonRoomArray = null;
    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();
    public List<DungeonRoom> roomList = new List<DungeonRoom>();

    public DungeonRoom curDungeonData;
    public RoomCtrl curRoomData;
    public PlayerDungeonUnitData curPlayerData = new PlayerDungeonUnitData();
}

public class DungeonMapSaveData_0 : SaveDataBase
{
    //public SerializeDictionary<Vector2, DungeonData> CurAllDungeonData;
    public List<DungeonRoom[]> dungeonRoomArray;
    public List<DungeonRoom> curDungeonData;
    public List<PlayerDungeonUnitData> curPlayerData;
    public List<Vector2> dungeonIndex;
    public Vector2 curDungeonIndex;
    public List<DungeonRoom> dungeonRoomList;

    public override SaveDataBase VersionUp()
    {
        return new DungeonMapSaveData_0();
    }
}