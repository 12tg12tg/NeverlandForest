using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSaveData_0 : SaveDataBase
{
    public ArrowType arrowType { get; set; }
    public List<Vector2> trapPos { get; set; }
    public List<TrapTag> trapType { get; set; }
    public bool isBluemoonDone { get; set; }
    public override SaveDataBase VersionUp()
    {
        return null;
    }
}
