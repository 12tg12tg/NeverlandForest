using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Obsolete("SkillRangeType을 사용하세요.")]
public enum SkillRangeShape
{
    One, Six, Eight, Nine, Cross, X
}

public enum SkillRangeType { One, Tile, Line, Lantern }

[Serializable]
public class PlayerSkillTableElem : DataTableElemBase
{
    private string iconID;
    public string name;
    public int count;
    public int damage;
    public bool isSideEffect;
    [Obsolete("rangeType을 사용하세요.")] public SkillRangeShape rangeShape;
    public SkillRangeType rangeType;
    public int additionalRange;
    public string description;

    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public PlayerSkillTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        count = int.Parse(data["COUNT"]);
        damage = int.Parse(data["DAMAGE"]);
        isSideEffect = data["SIDE EFFECT"].Equals("O") ? true : false;

        var enumNames = Enum.GetNames(typeof(SkillRangeShape)).ToList();
        rangeShape = (SkillRangeShape)enumNames.IndexOf(data["RANGE SHAPE"]);

        additionalRange = int.Parse(data["RANGE"]);

        description = string.Format(data["DESC"], damage);

        iconID = data["ICON_ID"];
        iconSprite = Resources.Load<Sprite>($"SkillSprites/{iconID}");
    }
}

public class PlayerSkillTable : DataTableBase
{
    public PlayerSkillTable()
    {
        csvFilePath = "PlayerSkillTable";
    }
    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in list.sc)
        {
            var elem = new PlayerSkillTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}
