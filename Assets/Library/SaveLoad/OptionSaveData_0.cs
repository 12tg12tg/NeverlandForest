using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSaveData_0 : SaveDataBase
{
    public static readonly OptionSaveData_0 DefaultValue = new OptionSaveData_0()
    {
        Version = 0,
        volume = 0f,
        isMute = false,
        isVibrate = false,
    };

    public float volume { get; set; }
    public bool isMute { get; set; }
    public bool isVibrate { get; set; }

    public override SaveDataBase VersionUp()
    {
        return new OptionSaveData_0();
    }
}
