using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType { Near, Far, Count }
public enum MonsterGrade { Normal, Epic, Elite }

[Serializable]
public class MonsterTableElem : DataTableElemBase
{
    public string iconID;
    private string localID;
    public MonsterGrade grade;
    public MonsterType type;
    public int group;
    public int hp;
    public int atk;
    public int sheild;
    private int minSpeed;
    private int maxSpeed;
    public string description;
    private Sprite iconSprite;

    public string Name
    { 
        get
        {
            var elem = DataTableManager.GetTable<LocalizationTable>().GetData<LocalizationTableElem>(localID);
            switch (Vars.localization)
            {
                case Localization.Korean:
                    return elem.kor;
                case Localization.English:
                    return elem.eng;
                default:
                    return string.Empty;
            }
        }
    }
    public Sprite IconSprite => iconSprite;
    public int Speed => UnityEngine.Random.Range(minSpeed, maxSpeed + 1);
    public MonsterTableElem(Dictionary<string, string> data, List<string> typeNames, List<string> gradeNames) : base(data)
    {
        id = data["ID"];
        iconID = data["ICON_ID"];
        localID = data["LOCAL_ID"];
        grade = (MonsterGrade)gradeNames.IndexOf(data["GRADE"]);
        group = int.Parse(data["GROUP"]);
        type = (MonsterType)typeNames.IndexOf(data["TYPE"]);
        atk = int.Parse(data["ATK"]);
        sheild = int.Parse(data["SHEILD"]);
        hp = int.Parse(data["HP"]);
        minSpeed = int.Parse(data["SPEED_MIN"]);
        maxSpeed = int.Parse(data["SPEED_MAX"]);
        iconSprite = Resources.Load<Sprite>($"icons/{iconID}");
    }
}


public class MonsterTable : DataTableBase
{
    public MonsterTable() => csvFilePath = "MonsterTable";
    public List<SerializeDictionary<string, string>> allMonster;
    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        var typeNames = Enum.GetNames(typeof(MonsterType)).ToList();
        var gradeNames = Enum.GetNames(typeof(MonsterGrade)).ToList();
        foreach (var line in list.sc)
        {
            var elem = new MonsterTableElem(line, typeNames, gradeNames);
            data.Add(elem.id, elem);
        }
    }
}
