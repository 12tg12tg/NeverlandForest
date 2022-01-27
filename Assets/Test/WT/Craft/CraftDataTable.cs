using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Explicit)]
struct CraftCombine
{
    [FieldOffset(0)]
    public int fullkey;
    [FieldOffset(0)]
    public byte material0;
    [FieldOffset(1)]
    public byte material1;
    [FieldOffset(2)]
    public byte material2;

}
public enum CraftType
{
    MATERIAL,
    TOOL,
    INSTALLATION,
    CONSUMABLE,
    FOOD,
    TRAP,
}

[Serializable]
public class CraftTableElem : DataTableElemBase
{
    public string name;
    public int type;
    public string result_ID;
    public string material0;
    public string material1;
    public string material2;
    public float duration_minute;

    public CraftTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        type = int.Parse(data["TYPE"]);
        result_ID = data["RESULTID"];
        material0 = data["Mat1"];
        material1 = data["Mat2"];
        material2 = data["Mat3"];
        duration_minute = float.Parse(data["DURATION"]);
    }
}
public class CraftDataTable : DataTableBase
{
    public List<Dictionary<string, string>> list;
    public CraftDataTable()
    {
        csvFilePath = "CraftDataTable";
    }
    public Dictionary<int, string> CombineDictionary =
        new Dictionary<int, string>(); // 조합식이랑 결과값을 가지고있는 diction
    public Dictionary<string, string[]> CombineListDictionary =
        new Dictionary<string, string[]>(); // 재료들의 번호를 가지고있는 diction

    public Dictionary<string, string> MakingTimeDictionary = new Dictionary<string, string>();
    //아이템 번호(Result)랑 시간에 대한 값들을 가지고 있다. 
    public override void Load()
    {
        if (data != null)
            data.Clear();
        else
            data = new SerializeDictionary<string, DataTableElemBase>();
        var alist = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in alist.sc)
        {
            var elem = new CraftTableElem(line);
            data.Add(elem.id, elem);

            var id1 = byte.Parse(elem.material0);
            var id2 = byte.Parse(elem.material1);
            var id3 = byte.Parse(elem.material2);

            var combinekey = new CraftCombine();
            combinekey.material0 = id1;
            combinekey.material1 = id2;
            combinekey.material2 = id3;
            CombineDictionary.Add(combinekey.fullkey, elem.result_ID);

            string[] crafts = new string[] { elem.material0, elem.material1, elem.material2 };

            CombineListDictionary.Add(elem.result_ID, crafts);

            string craftTime = elem.duration_minute.ToString();
            MakingTimeDictionary.Add(elem.result_ID, craftTime);
        }
    }
    public bool IsCombine(string material0, string material1, out string result, string material2 = "0")
    {
        var combinekey = new CraftCombine();
        combinekey.material0 = byte.Parse(material0);
        combinekey.material1 = byte.Parse(material1);
        combinekey.material2 = byte.Parse(material2);
        return CombineDictionary.TryGetValue(combinekey.fullkey, out result);
    }
    public string IsMakingTime(string key)
    {
        return MakingTimeDictionary[key];
    }
    public string[] GetCombination(string key)
    {
        return CombineListDictionary[key];
    }
    public string GetCraftId(string result)
    {
        var alist = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in alist.sc)
        {
            if (line["RESULTID"].Equals(result))
            {
                return line["ID"];
            }
        }
        return string.Empty;
    }
}
