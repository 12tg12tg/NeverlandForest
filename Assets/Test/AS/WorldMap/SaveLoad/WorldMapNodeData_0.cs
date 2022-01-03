using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapNodeStruct_0
{
    public Difficulty difficulty;
    public Vector2 index;
    public int level;
    public List<Vector2> children;
    public List<Vector2> parent;
    
}

public class WorldMapNodeData_0 : SaveDataBase
{
    public List<MapNodeStruct_0> MapNodeStruct { get; set; } = new List<MapNodeStruct_0>();
}