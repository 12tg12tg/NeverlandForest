using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSaveData_0 : SaveDataBase
{
    public static readonly OptionSaveData_0 DefaultValue = new OptionSaveData_0()
    {
        Version = 0,
        bgmVolume = 0f,
        sfVolume = 0f,
        isBgmMute = false,
        isSfMute=false,
        isVibrate = false,
    };

    public float bgmVolume { get; set; }
    public float sfVolume { get; set; }
    public bool isBgmMute { get; set; }
    public bool isSfMute { get; set; }

    public bool isVibrate { get; set; }

    public override SaveDataBase VersionUp()
    {
        return new OptionSaveData_0();
    }
}
