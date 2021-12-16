using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableTableElemInho : DataTableElemBase
{
    public string iconId;
    public string prefabsId;
    public string name;
    public string description;
    public int weight;
    public int cost;
    public int hp;
    public int mp;
    public int str;
    public float duration;

    private Sprite iconSprite;

    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public ConsumableTableElemInho(Dictionary<string, string> data)
    {
        id = data["ID"];
        iconId = data["ICON_ID"];
        prefabsId = data["PREFAB_ID"];
        name = data["NAME"];
        description = data["DESC"];
        type = data["TYPE"];
        weight = int.Parse(data["WEIGHT"]);
        cost = int.Parse(data["COST"]);
        hp = int.Parse(data["STAT_HP"]);
        mp = int.Parse(data["STAT_MP"]);
        str = int.Parse(data["STAT_STR"]);
        duration = float.Parse(data["DURATION"]);
        iconSprite = Resources.Load<Sprite>($"Sprites/Icons/{iconId}");
    }
}

public class ConsumableTableInho : DataTableBase
{
    public ConsumableTableInho()
    {
        csvFilePath = @"Tables\ConsumDataTable";
    }

    public override void Load()
    {
        data.Clear();
        var list = CSVReader.Read(csvFilePath);
        foreach (var line in list)
        {
            var elem = new ConsumableTableElemInho(line);
            data.Add(elem.id, elem);
        }

    }
}

