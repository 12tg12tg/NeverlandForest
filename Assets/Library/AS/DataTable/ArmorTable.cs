using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;

public enum ArmorStat
{
    Defence,
    Stat_str,
    Stat_con,
    Stat_int,
    Stat_luk,
    Evade,
    Block,
}

[Serializable]
public class ArmorTableElem : DataTableElemBase
{
    public string name;
    public string description;
    public string iconID;
    public string prefabsID;
    public string type;

    public int cost;
    public int defence;
    public int weight;
    public int stat_str;
    public int stat_con;
    public int stat_int;
    public int stat_luk;
    public int evade;
    public int block;

    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public ArmorTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        description = data["DESC"];
        iconID = data["ICON_ID"];
        prefabsID = data["PREFAB_ID"];
        type = data["TYPE"];

        cost = int.Parse(data["COST"]);
        defence = int.Parse(data["DEF"]);
        weight = int.Parse(data["WEIGHT"]);
        stat_str = int.Parse(data["STR"]);
        stat_con = int.Parse(data["CON"]);
        stat_int = int.Parse(data["INT"]);
        stat_luk = int.Parse(data["LUK"]);

        evade = int.Parse(data["EVADE"]);
        block = int.Parse(data["BLOCK"]);

        iconSprite = Resources.Load<Sprite>($"icons/{iconID}");
    }
}

public class ArmorTable : DataTableBase
{
    public string[] tableTitle;
    public ArmorTable()
    {
        csvFilePath = "Tables/DefDataTable"; // csv파일 이름
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
            var elem = new ArmorTableElem(line);
            data.Add(elem.id, elem);
        }
    }

    public override void Save(DataTableBase dataTableBase)
    {
        CSVWriter.Writer(csvFilePath, dataTableBase);
    }
}
