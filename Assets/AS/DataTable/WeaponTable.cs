using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WeaponStat
{
    Damage,
    Critical,
    CriticalDamage,
    Stat_str,
    Stat_con,
    Stat_int,
    Stat_luk,
}

// 멤버들
[Serializable]
public class WeaponTableElem : DataTableElemBase
{
    public string name;
    public string description;
    public string iconID;
    public string prefabsID;
    public string type;

    public int cost;
    public int damage;
    public int critical;
    public int criticalDamage;
    public int weight;
    public int stat_str;
    public int stat_con;
    public int stat_int;
    public int stat_luk;
   
    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public WeaponTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        description = data["DESC"];
        iconID = data["ICON_ID"];
        prefabsID = data["PREFAB_ID"];
        type = data["TYPE"];

        cost = int.Parse(data["COST"]);
        damage = int.Parse(data["DAMAGE"]);
        critical = int.Parse(data["CRIT_RATE"]);
        criticalDamage = int.Parse(data["CIRT_DAMAGE"]);
        weight = int.Parse(data["WEIGHT"]);
        stat_str = int.Parse(data["STR"]);
        stat_con = int.Parse(data["CON"]);
        stat_int = int.Parse(data["INT"]);
        stat_luk = int.Parse(data["LUK"]);

        iconSprite = Resources.Load<Sprite>($"icons/{iconID}");
    }
}

[Serializable]
public class WeaponTable : DataTableBase
{
    public string[] tableTitle;
    public WeaponTable()
    {
        csvFilePath = "Tables/WeaponDataTable"; // csv파일 이름
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
            var elem = new WeaponTableElem(line);
            data.Add(elem.id, elem);
        }
    }

    public override void Save(DataTableBase dataTableBase)
    {
        CSVWriter.Writer(csvFilePath, dataTableBase);
    }
}