using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
struct RecipeCombine
{
    [FieldOffset(0)]
    public int fullkey;
    [FieldOffset(0)]
    public byte fire;
    [FieldOffset(1)]
    public byte msg;
    [FieldOffset(2)]
    public byte material;

}
[Serializable]
public class RecipeTableElem : DataTableElemBase
{
    public string result_ID;
    public string name;
    public int type;
    public string material1;
    public string material2;
    public string material3;

    public float time;
  

    public RecipeTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        type = int.Parse(data["TYPE"]);
        result_ID = data["RESULTID"];
        material1 = data["Mat1"];
        material2 = data["Mat2"];
        material3 = data["Mat3"];
        time = float.Parse(data["DURATION"]);
    }
}

public class RecipeDataTable : DataTableBase
{
    public string[] tableTitle;
    public List<Dictionary<string, string>> list;

    public RecipeDataTable()
    {
        csvFilePath = "RecipeDataTable";
    }

    public Dictionary<int, string> CombineDictionary =
        new Dictionary<int, string>(); // 조합식이랑 결과값을 가지고있는 diction
    public Dictionary<string, string[]> CombineListDictionary =
        new Dictionary<string, string[]>(); // 재료들의 번호를 가지고있는 diction

    public Dictionary<string, string> MakingTimeDictionary = new Dictionary<string, string>();

    public Dictionary<string, string> allRecipeDictionary = new Dictionary<string, string>();
    public List<string> allRecipeIdList = new List<string>();
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
            var elem = new RecipeTableElem(line);
            data.Add(elem.id, elem);
            allRecipeIdList.Add(elem.id);

            var id1 = byte.Parse(elem.material1);
            var id2 = byte.Parse(elem.material2);
            var id3 = byte.Parse(elem.material3);

            var combinekey = new RecipeCombine();
            combinekey.fire = id1;
            combinekey.msg = id2;
            combinekey.material = id3;
            CombineDictionary.Add(combinekey.fullkey, elem.result_ID);

            string[] recipe = new string[] { elem.material1, elem.material2, elem.material3 };

            CombineListDictionary.Add(elem.result_ID, recipe);

            allRecipeDictionary.Add(elem.id, elem.result_ID);

            string Time = elem.time.ToString();
            MakingTimeDictionary.Add(elem.result_ID, Time);
        }
    }

    public bool IsCombine(string material1, string material2,out string result,string material3 = "0")
    {
        var combinekey = new RecipeCombine();
        combinekey.fire = byte.Parse(material1);
        combinekey.msg = byte.Parse(material2);
        combinekey.material = byte.Parse(material3);
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
    public string GetRecipeId(string result)
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
