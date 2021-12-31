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
    public List<DungeonRoom> curSubRoomList = new List<DungeonRoom>();
    public Vector3 curPlayerPosition;
    public List<EventObject> curEventObjList = new List<EventObject>();
}