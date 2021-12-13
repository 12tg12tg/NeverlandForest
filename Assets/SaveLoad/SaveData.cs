using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData1 : SaveDataBase
{
    public static readonly SaveData1 DefaultValue = new SaveData1()
    {
        Version = 1,
        intData = 10,
        stringData = "Default",
        boolData = true,
        listData = new List<int> { 0, 0, 0, 0 },
        color = Color.clear
    };

    public int intData { get; set; }
    public string stringData { get; set; }
    public bool boolData { get; set; }
    public List<int> listData { get; set; }
    public Color color { get; set; }

    //public override SaveDataBase VersionUpgrade()
    //{
    //    return new SaveData1();
    //}
}


public class SaveData : SaveDataBase
{
    public static readonly SaveData DefaultValue = new SaveData()
    {
        Version = 0,
        intData = 10,
        stringData = "Default",
        boolData = false,
        listData = new List<int> { 0, 0, 0 }
    };

    public int intData { get; set; }
    public string stringData { get; set; }
    public bool boolData { get; set; }
    public List<int> listData { get; set; }

    public override SaveDataBase VersionUpgrade()
    {
        SaveData1 nextData = new SaveData1();
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

public class SaveDataBase
{
    public int Version { get; set; }
    public virtual SaveDataBase VersionUpgrade()
    {
        return new SaveDataBase();
    }
}

/*==============*/
/*선생님 레퍼런스*/
//public class SaveDataTV2 : SaveDataTV1
//{
//    public float floatValue1 { get; set; } = 1.0f;
//    public SaveDataTV2()
//    {
//        version = 2;
//    }
//    public SaveDataTV2(SaveDataTV1 prevVersion) : this()
//    {
//        strValue1 = 2;
//    }

//}
//public class SaveDataTV1 : SaveDataT
//{
//    public string strValue1 { get; set; } = string.Empty;
//    public SaveDataTV1()
//    {
//        version = 1;
//    }
//    public SaveDataTV1(SaveDataT prevVersion)
//    {

//    }
//    public override SaveDataT VersionUp()
//    {
//        return new SaveDataTV2(this);
//    }
//}

//public class SaveDataT
//{
//    public int version { get; set; }

//    public SaveDataT()
//    {
//        version = 0;
//    }

//    public virtual SaveDataT VersionUp()
//    {

//        return new SaveDataTV1(this);
//    }
//}

/*선생님 레퍼런스*/
/*==============*/