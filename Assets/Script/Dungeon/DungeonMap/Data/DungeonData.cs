using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    // ���� ����ũ�� ������ ������ġ�� �ϴ� ����..
    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    public DungeonRoom curDungeonRoomData;
    public PlayerDungeonUnitData curPlayerGirlData = new PlayerDungeonUnitData();
    public PlayerDungeonUnitData curPlayerBoyData = new PlayerDungeonUnitData();
}

public class DungeonMapSaveData_0 : SaveDataBase
{
    public List<List<DungeonRoom>> dungeonRoomList;
    public List<PlayerDungeonUnitData> curPlayerGirlData;
    public List<PlayerDungeonUnitData> curPlayerBoyData;
    public List<Vector2> dungeonIndex;
    public Vector2 curDungeonIndex;
    public int curDungeonRoomIndex;

    public int dungeonLastIdx;
    public bool isDungeonReStart;
    public bool isDungeonClear;
    public bool isPlayerDungeonIn;

    public int mainRoomCount;

    public override SaveDataBase VersionUp()
    {
        return new DungeonMapSaveData_0();
    }
}

//public SerializeDictionary<Vector2, DungeonData> CurAllDungeonData;
//public List<DungeonRoom[]> dungeonRoomArray;

//public RoomCtrl curRoomInstanceData;
//public int dungeonStartIdx = -1;
//public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();