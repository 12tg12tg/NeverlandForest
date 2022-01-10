using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public int dungeonStartIdx = -1;
    // ���� ����ũ�� ������ ������ġ�� �ϴ� ����..
    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();

    public DungeonRoom curDungeonData;
    public RoomCtrl curRoomData;
    public PlayerDungeonUnitData curPlayerData = new PlayerDungeonUnitData();
}

public class DungeonMapSaveData_0 : SaveDataBase
{
    public List<List<DungeonRoom>> dungeonRoomList;
    //public List<DungeonRoom> curDungeonData;
    public List<PlayerDungeonUnitData> curPlayerData;
    public List<Vector2> dungeonIndex;
    public Vector2 curDungeonIndex;
    
    public override SaveDataBase VersionUp()
    {
        
        return new DungeonMapSaveData_0();
    }
}

//public SerializeDictionary<Vector2, DungeonData> CurAllDungeonData;
//public List<DungeonRoom[]> dungeonRoomArray;