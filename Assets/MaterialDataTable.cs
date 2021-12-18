using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]

public class MaterialTableElem : DataTableElemBase
{
    public string name;
    private string type;

    private Sprite iconSprite;
    public Sprite IconSprite
    {
        get { return iconSprite; }
    }
    public MaterialTableElem(Dictionary<string, string> data) :base(data)
    {
        id = data["ID"];
        name = data["NAME"];
        type = data["TYPE"];
    }
}

public class MaterialDataTable : DataTableBase
{
    public string[] tableTitle;
    public MaterialDataTable()
    {
        csvFilePath = "Tables/MaterialDataTable";
    }
    public override void Load()
    {

        if (data != null)
            data.Clear();
        else
            data = new SerializeDictionary<string, DataTableElemBase>();
        var list = CSVReader.Read(csvFilePath); // �����ڿ��� �ص� �ȴ� ������ ������ �ؾ��ϴ� �� �̱� ������
        tableTitle = list.First().Keys.ToArray();
        foreach (var line in list)
        {
            var elem = new MaterialTableElem(line);
            data.Add(elem.id, elem);
        }
    }

    public override void Save(DataTableBase dataTableBase)
    {
        CSVWriter.Writer(csvFilePath, dataTableBase);
    }
}
