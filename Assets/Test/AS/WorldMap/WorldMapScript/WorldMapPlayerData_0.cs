using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class WorldMapPlayerData
{
    public Vector2 currentIndex;
    public Vector2 goalIndex;

    public Vector3 startPos;
    public Vector3 goalPos;
    public Vector3 currentPos;

    public bool isClear;
}

public class WorldMapPlayerData_0 : SaveDataBase
{
    public WorldMapPlayerData WorldMapPlayerData { get; set; }
}