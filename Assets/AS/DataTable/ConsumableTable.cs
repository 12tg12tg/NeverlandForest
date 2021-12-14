using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;

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
    protected ConsumableTableElem(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }

    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}

[Serializable]
public class ConsumableTable : DataTableBase
{
    public ConsumableTable()
    {
        csvFilePath = "Tables/ConsumDataTable"; // csv���� �̸�
    }

    public override void Load()
    {
        if (data != null)
            data.Clear();
        else
            data = new SerializeDictionary<string, DataTableElemBase>();
        var list = CSVReader.Read(csvFilePath); // �����ڿ��� �ص� �ȴ� ������ ������ �ؾ��ϴ� �� �̱� ������
        foreach (var line in list)
        {
            var elem = new ConsumableTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}