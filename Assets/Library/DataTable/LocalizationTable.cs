using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationTableElem : DataTableElemBase
{
    public string kor;
    public string eng;


    public LocalizationTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        kor = data["KOR"];
        eng = data["ENG"];
    }
}

public class LocalizationTable : DataTableBase
{
    public LocalizationTable() => csvFilePath = "LocalizationTable";

    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in list.sc)
        {
            var elem = new LocalizationTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}
