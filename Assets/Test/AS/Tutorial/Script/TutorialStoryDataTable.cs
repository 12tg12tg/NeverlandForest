using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialStoryDataTableElem : DataTableElemBase
{
    public string character;
    public string description;
    public string color;

    public int index;

    public bool option;
    public bool typing;
    public bool fade;

    public TutorialStoryDataTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];

        character = data["CHAR"];
        description = data["DESC"];
        color = data["COLOR"];

        index = Convert.ToInt32(data["INDEX"]);

        option = Convert.ToBoolean(data["OPTION"]);
        typing = Convert.ToBoolean(data["TYPING"]);
        fade = Convert.ToBoolean(data["FADE"]);
    }
}

public class TutorialStoryDataTable : DataTableBase
{
    public TutorialStoryDataTable() => csvFilePath = "TutorialStoryDataTable";
    public List<string> ids = new List<string>();
    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach (var line in list.sc)
        {
            var elem = new TutorialStoryDataTableElem(line);
            data.Add(elem.id, elem);
            ids.Add(elem.id);
        }
    }
}
