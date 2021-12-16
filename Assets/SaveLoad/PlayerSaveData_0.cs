using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData_1 : SaveDataBase
{
    public static readonly PlayerSaveData_1 DefaultValue = new PlayerSaveData_1()
    {
        Version = 1,
        intData = 0,
        stringData = "Default",
        boolData = true,
        listData = new List<int> { 0, 0, 0, 0 },
        color = Color.clear
    };

    public int intData { get; set; }
    public string stringData { get; set; }
    public bool boolData { get; set; }
    public List<int> listData { get; set; } = new List<int>();
    public Color color { get; set; }

    public override SaveDataBase VersionUp()
    {
        return new PlayerSaveData_1();
    }
}

public class PlayerSaveData_0 : SaveDataBase
{
    public static readonly PlayerSaveData_0 DefaultValue = new PlayerSaveData_0()
    {
        Version = 0,
        intData = 0,
        stringData = "Default",
        boolData = false,
        listData = new List<int> { 0, 0, 0 }
    };

    public int intData { get; set; }
    public string stringData { get; set; }
    public bool boolData { get; set; }
    public List<int> listData { get; set; } = new List<int>();

    public override SaveDataBase VersionUp()
    {
        PlayerSaveData_1 nextData = new PlayerSaveData_1();
        nextData.Version = 1;
        nextData.intData = this.intData;
        nextData.stringData = this.stringData;
        nextData.boolData = true;           //무조건 true
        nextData.listData = this.listData;
        nextData.listData.Add(0);           //길이를 4로.
        nextData.color = Color.cyan;        //색 추가.
        return nextData;
    }
}

