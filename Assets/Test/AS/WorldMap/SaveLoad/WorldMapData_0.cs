using System.Collections.Generic;
using UnityEngine;

public struct WorldMapNodeStruct
{
    public Vector2 index;
    public int level;
    public List<Vector2> children;
    public List<Vector2> parent;
}

public struct WorldMapTreeInfo
{
    public Vector3 treePos;
    public int type;
}

public class WorldMapData_0 : SaveDataBase
{
    public List<WorldMapNodeStruct> WorldMapNodeStruct { get; set; } = new List<WorldMapNodeStruct>();
    public List<WorldMapTreeInfo> WorldMapTree { get; set; } = new List<WorldMapTreeInfo>();
}