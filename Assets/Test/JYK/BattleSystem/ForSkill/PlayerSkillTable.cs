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
    public PlayerType player;
    public string aniTrigger;
    public int cost;
    private int min_Damage;
    private int max_Damage;
    private bool addLantern;
    public int hitCount;
    public SkillRangeType range;
    public string description;
    public string attackSound;
    public int Damage
    {
        get
        {
            int damage;
            if (min_Damage == max_Damage)
                damage = min_Damage;
            else
                damage = UnityEngine.Random.Range(min_Damage, max_Damage + 1);

            if (addLantern)
            {
                damage += (int)Vars.UserData.uData.lanternState;
            }

            if (player == PlayerType.Boy && Vars.UserData.arrowType == ArrowType.Iron
                && !BattleManager.Instance.costLink.skillIDs_NearAttack.Contains(id))
            {
                damage += BattleManager.Instance.costLink.ironArrowElem.damage;
            }

            return damage;
        }
    }

    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public PlayerSkillTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];

        var enums = Enum.GetNames(typeof(PlayerType)).ToList();
        player = (PlayerType)enums.IndexOf(data["PLAYER"]);

        aniTrigger = data["ANI TRIGGER"];

        cost = int.Parse(data["COST"]);
        min_Damage = int.Parse(data["MIN DAMAGE"]);
        max_Damage = int.Parse(data["MAX DAMAGE"]);

        addLantern = data["ADD LANTERN"].Equals("O") ? true : false;

        hitCount = int.Parse(data["HIT COUNT"]);

        var enums2 = Enum.GetNames(typeof(SkillRangeType)).ToList();
        range = (SkillRangeType)enums2.IndexOf(data["RANGE"]);

        description = data["DESC"];

        iconID = data["ICON_ID"];
        iconSprite = Resources.Load<Sprite>($"SkillSprites/{iconID}");

        attackSound = data["SOUND"];
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
