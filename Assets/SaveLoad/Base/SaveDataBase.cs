using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataBase
{
    public int Version { get; set; }
    public virtual SaveDataBase VersionUp()
    {
        return new SaveDataBase();
    }
}