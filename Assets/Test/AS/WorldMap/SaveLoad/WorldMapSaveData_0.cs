using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapNodeStruct_0
{
    public Vector2 index;
    public int level;
    public List<Vector2> children;
    public List<Vector2> parent;
}

public class WorldMapSaveData_0 : SaveDataBase
{
    public List<MapNodeStruct_0> MapNodeStruct { get; set; } = new List<MapNodeStruct_0>();
    public Vector2 PlayerCurrentIndex { get; set; }
}