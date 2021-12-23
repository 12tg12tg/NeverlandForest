using System;
using System.Collections;
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
    public AllItemTableElem(Dictionary<string, string> data) :base(data)
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
    public string[] tableTitle;
    public AllItemDataTable()
    {
        csvFilePath = "Tables/AllItemDataTable";
    }
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
            var elem = new AllItemTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}
