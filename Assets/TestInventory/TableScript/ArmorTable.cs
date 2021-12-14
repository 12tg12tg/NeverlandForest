using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ArmorStat
{
    Df,
    StatStr,
    StatInt,
    StatLuk,
}

public class ArmorTableElem : DataTableElemBase
{
    public string iconId;
    public string prefabId;
    public string name;
    public string description;
    public int def;
    public int cost;
    public int weight;
    public int str;
    public int con;
    public int intellet;
    public int luck;
    public int evade;
    public int block;

    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public ArmorTableElem(Dictionary<string, string> data)
    {
        id = data["ID"];
        iconId = data["ICON_ID"];
        prefabId = data["PREFAB_ID"];
        name = data["NAME"];
        description = data["DESC"];
        type = data["TYPE"];
        def = int.Parse(data["DEF"]);
        cost = int.Parse(data["COST"]);
        weight = int.Parse(data["WEIGHT"]);
        str = int.Parse(data["STR"]);
        con = int.Parse(data["CON"]);
        intellet = int.Parse(data["INT"]);
        luck = int.Parse(data["LUK"]);
        evade = int.Parse(data["EVADE"]);
        block = int.Parse(data["BLOCK"]);
        iconSprite = Resources.Load<Sprite>($"Sprites/Icons/{iconId}");
    }
}

public class ArmorTable : DataTableBase
{
    public ArmorTable()
    {
        csvFilePath = @"Tables\DefDataTable";
    }

    public override void Load()
    {
        data.Clear();
        var list = CSVReader.Read(csvFilePath);
        foreach (var line in list)
        {
            var elem = new ArmorTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}