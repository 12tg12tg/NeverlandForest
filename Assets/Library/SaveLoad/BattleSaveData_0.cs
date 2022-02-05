using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSaveData_0 : SaveDataBase
{
    public ArrowType arrowType { get; set; }

    public override SaveDataBase VersionUp()
    {
        return null;
    }
}
