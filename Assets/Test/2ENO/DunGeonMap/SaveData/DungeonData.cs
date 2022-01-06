using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public int dungeonStartIdx = -1;
    public DungeonRoom[] dungeonRoomArray = null;
    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();

    public DungeonRoom curDungeonData;
    public RoomCtrl curRoomData;
    public PlayerDungeonUnitData curPlayerData = new PlayerDungeonUnitData();
}