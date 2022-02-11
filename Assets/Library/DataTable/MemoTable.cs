using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoTableElem : DataTableElemBase
{
    public int count;
    public string date;
    public string desc;

    public MemoTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        count = int.Parse(data["COUNT"]);
        date = data["DATE"];
        desc = data["DESCRIPTION"];
    }
}
public class MemoTable : DataTableBase
{
    public MemoTable() => csvFilePath = "MemoDataTable";

    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in list.sc)
        {
            var elem = new MemoTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}
