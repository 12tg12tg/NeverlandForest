using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;
using System.Linq;

[Serializable]
public class ConsumableTableElem : DataTableElemBase // ��� ID �뵵
{
    public string iconID;
    public string prefabsID;
    public string name;
    public string description;
    public int cost;
    public int hp;
    public int mp;
    public int statStr;
    public float duration;

    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }
    public ConsumableTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        iconID = data["ICON_ID"];
        prefabsID = data["PREFAB_ID"];
        name = data["NAME"];
        description = data["DESC"];

        cost = int.Parse(data["COST"]);
        hp = int.Parse(data["STAT_HP"]);
        mp = int.Parse(data["STAT_MP"]);
        statStr = int.Parse(data["STAT_STR"]);
        duration = float.Parse(data["DURATION"]);
        iconSprite = Resources.Load<Sprite>($"icons/{iconID}");
    }
}

public class ConsumableTable : DataTableBase
{
    public ConsumableTable()
    {
        csvFilePath = "ConsumDataTable"; // csv���� �̸�
    }

    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in list.sc)
        {
            var elem = new ConsumableTableElem(line);

            data.Add(elem.id, elem);
        }
    }
}