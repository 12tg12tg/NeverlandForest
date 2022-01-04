using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public DungeonRoom[] dungeonRoomArray = null;
    public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();
    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();

    public DungeonRoom curDungeonData;
    public RoomCtrl curRoomData;
    public int curRoadCount;
    public List<DungeonRoom> curIncludeRoomList = new List<DungeonRoom>();
    public PlayerDungeonUnitData curPlayerData = new PlayerDungeonUnitData();
    public List<EventObjectData> curEventObjList = new List<EventObjectData>();
}