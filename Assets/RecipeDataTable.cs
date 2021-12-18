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
  
    public RecipeTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        result_ID = data["RESULTID"];
        FireExist = data["FIRE"];
        MSG = data["MSG"];
        Material = data["MATERIAL"];
    }
}

public class RecipeDataTable : DataTableBase
{
    public string[] tableTitle;
    public RecipeDataTable()
    {
        csvFilePath = "Tables/RecipeDataTable";
    }

    public Dictionary<int, string> CombineDictionary =
        new Dictionary<int, string>();
    public Dictionary<string, string[]> CombineListDictionary =
        new Dictionary<string, string[]>();
   
    public override void Load()
    {
        if (data != null)
            data.Clear();
        else
            data = new SerializeDictionary<string, DataTableElemBase>();
        var list = CSVReader.Read(csvFilePath); // 생성자에서 해도 된다 어차피 무조껀 해야하는 것 이기 때문에
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
        }
    }

    public bool ISCombine(string fireexist, string msg, string material,out string result)
    {
        var combinekey = new RecipeCombine();
        combinekey.fire = byte.Parse(fireexist);
        combinekey.msg = byte.Parse(msg);
        combinekey.material = byte.Parse(material);
        return CombineDictionary.TryGetValue(combinekey.fullkey, out result);
    }

    public string[] GetCombination(string key)
    {
        return CombineListDictionary[key];
    }


    public override void Save(DataTableBase dataTableBase)
    {
        CSVWriter.Writer(csvFilePath, dataTableBase);
    }
}
