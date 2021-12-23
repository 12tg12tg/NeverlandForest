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
    public string FireExist;
    public string MSG;
    public string Material;

    public string Hour;
    public string Mininute;
    public string Second;

    public RecipeTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        result_ID = data["RESULTID"];
        FireExist = data["FIRE"];
        MSG = data["MSG"];
        Material = data["MATERIAL"];
        Hour = data["HOUR"];
        Mininute = data["MININUTE"];
        Second = data["SECOND"];
    }
}

public class RecipeDataTable : DataTableBase
{
    public string[] tableTitle;
    public List<Dictionary<string, string>> list;

    public RecipeDataTable()
    {
        csvFilePath = "Tables/RecipeDataTable";
    }

    public Dictionary<int, string> CombineDictionary =
        new Dictionary<int, string>(); // 조합식이랑 결과값을 가지고있는 diction
    public Dictionary<string, string[]> CombineListDictionary =
        new Dictionary<string, string[]>(); // 재료들의 번호를 가지고있는 diction

    public Dictionary<string, string[]> MakingTimeDictionary = new Dictionary<string, string[]>();
    //아이템 번호(Result)랑 시간에 대한 값들을 가지고 있다. 

    public override void Load()
    {
        if (data != null)
            data.Clear();
        else
            data = new SerializeDictionary<string, DataTableElemBase>();
        list = CSVReader.Read(csvFilePath); // 생성자에서 해도 된다 어차피 무조껀 해야하는 것 이기 때문에
        tableTitle = list.First().Keys.ToArray();
        foreach (var line in list)
        {
            var elem = new RecipeTableElem(line);
            data.Add(elem.id, elem);

            var id1 =byte.Parse(elem.FireExist);
            var id2 = byte.Parse(elem.MSG);
            var id3 = byte.Parse(elem.Material);

            var combinekey = new RecipeCombine();
            combinekey.fire = id1;
            combinekey.msg = id2;
            combinekey.material = id3;
            CombineDictionary.Add(combinekey.fullkey, elem.result_ID);

            string[] recipe = new string[] { elem.FireExist ,elem.MSG, elem.Material};

            CombineListDictionary.Add(elem.result_ID, recipe);

            string[] TimeRecipe = new string[] { elem.Hour, elem.Mininute, elem.Second };
            MakingTimeDictionary.Add(elem.result_ID, TimeRecipe);
        }
    }

    public bool ISCombine(string msg,string material,out string result,string fireexist= "0")
    {
        var combinekey = new RecipeCombine();
        combinekey.fire = byte.Parse(fireexist);
        combinekey.msg = byte.Parse(msg);
        combinekey.material = byte.Parse(material);
        return CombineDictionary.TryGetValue(combinekey.fullkey, out result);
    }

    public string[] IsMakingTime(string key)
    {
        return MakingTimeDictionary[key];
    }



    public string[] GetCombination(string key)
    {
        return CombineListDictionary[key];
    }
    public string GetRecipeId(string result)
    {
        foreach (var line in list)
        {
            if (line["RESULTID"].Equals(result))
            {
                return line["ID"];
            }
        }
        return string.Empty;
    }
   
    public override void Save(DataTableBase dataTableBase)
    {
        CSVWriter.Writer(csvFilePath, dataTableBase);
    }
}
