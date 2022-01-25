using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AllItemTableElem : DataTableElemBase
{
    public string name;
    public string desc;
    public string type;
    public int limitCount;
    public bool isEat;
    public bool isBurn;
    public int stat_Hp;
    public ObstacleType obstacleType;
    public int obstacleHp;
    public int trapDamage;
    public int duration;
    public int InstallationOfNumber;
    
    private string iconID;
    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    private string prefabID;
    private GameObject prefab;
    public GameObject Prefab
    {
        get { return prefab; }
    }
    public AllItemTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        desc = data["DESC"];
        type = data["TYPE"];
        limitCount = int.Parse(data["LIMIT"]);
        isEat = Convert.ToBoolean(int.Parse(data["EAT"]));
        isBurn = Convert.ToBoolean(int.Parse(data["BURN"]));
        stat_Hp = int.Parse(data["STAT_HP"]);
        obstacleType = (ObstacleType)int.Parse(data["OBSTACLE_TYPE"]);
        obstacleHp = int.Parse(data["OBSTACLE_HP"]);
        trapDamage = int.Parse(data["TRAP_DAMAGE"]);
        duration = int.Parse(data["DURATION"]);
        InstallationOfNumber = int.Parse(data["INSTALLATION_NUMBER"]);

        iconID = data["ICON_ID"];
        iconSprite = Resources.Load<Sprite>($"Icons/{iconID}");

        prefabID = data["PREFAB_ID"];
        // TODO : юс╫ц
        //prefab = Resources.Load<GameObject>($"Icons/{iconID}");
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
