using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Near, Far,
    Count
}

[Serializable]
public class MonsterTableElem : DataTableElemBase
{
    public string iconID;
    public string name;
    public MonsterType type;
    public int hp;
    public int atk;
    public int sheild;
    public int speed;
    public string description;

    private Sprite iconSprite;
    public Sprite IconSprite => iconSprite;
    public MonsterTableElem(Dictionary<string, string> data, List<string> typeNames) : base(data)
    {
        id = data["ID"];
        iconID = data["ICON_ID"];
        name = data["NAME"];
        type = (MonsterType)typeNames.IndexOf(data["TYPE"]);
        hp = int.Parse(data["HP"]);
        atk = int.Parse(data["ATK"]);
        sheild = int.Parse(data["SHEILD"]);
        //speed = int.Parse(data["SPEED"]);
        //description = data["DESC"];
        iconSprite = Resources.Load<Sprite>($"icons/{iconID}");
    }
}


public class MonsterTable : DataTableBase
{
    public MonsterTable() => csvFilePath = "MonsterTable";

    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        var names = Enum.GetNames(typeof(MonsterType)).ToList();
        foreach (var line in list.sc)
        {
            var elem = new MonsterTableElem(line, names);
            data.Add(elem.id, elem);
        }
    }
}
