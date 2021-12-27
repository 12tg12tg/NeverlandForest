using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]

public class AllItemTableElem : DataTableElemBase
{
    public string name;
    public string type;
    private string Desc;
    private string iconID;
    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }
    public AllItemTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        Desc = data["DESC"];
        type = data["TYPE"];
        iconID = data["ICON_ID"];
        iconSprite = Resources.Load<Sprite>($"icons/{iconID}");
    }
}

public class AllItemDataTable : DataTableBase
{
    public AllItemDataTable()
    {
        csvFilePath = "AllItemDataTable";
    }
    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in list.sc)
        {
            var elem = new AllItemTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}
