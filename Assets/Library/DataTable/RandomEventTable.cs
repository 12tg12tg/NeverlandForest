using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RandomEventCondition
{
    None,
    Always,
    Conditional,
}

public enum RandomEventFrequency
{
    None,
    Usually,
    Often,
    SomeTime,
    Rarely,
}

public enum EventFeedBackType
{
    NoLose,
    GetNote,
    Stamina,
    Hp,
    Item,
    LanternGage,
    TurnConsume,
    Battle,
    MostItemLose,
    RandomMaterial,
    AnotherEvent,
    None,
}

public class RandomEventTableElem : DataTableElemBase
{
    public string name;
    public string eventDesc;
    public RandomEventCondition eventCondition;
    public RandomEventFrequency eventFrequency;
    public int eventFrequency2;
    public string select1Name;
    public int sucess1Chance;
    public List<EventFeedBackType> sucess1Type = new();
    public List<int> sucess1FeedBackID = new();
    public List<int> sucess1Val = new();
    public string sucess1Desc;
    public List<EventFeedBackType> fail1Type = new();
    public List<int> fail1FeedBackID = new();
    public List<int> fail1Val = new();
    public string fail1Desc;
    public string select2Name;
    public int sucess2Chance;
    public List<EventFeedBackType> sucess2Type = new();
    public List<int> sucess2FeedBackID = new();
    public List<int> sucess2Val = new();
    public string sucess2Desc;
    public List<EventFeedBackType> fail2Type = new();
    public List<int> fail2FeedBackID = new();
    public List<int> fail2Val = new();
    public string fail2Desc;
    public string select3Name;
    public int sucess3Chance;
    public List<EventFeedBackType> sucess3Type = new();
    public List<int> sucess3FeedBackID = new();
    public List<int> sucess3Val = new();
    public string sucess3Desc;
    public List<EventFeedBackType> fail3Type = new();
    public List<int> fail3FeedBackID = new();
    public List<int> fail3Val = new();
    public string fail3Desc;

    public RandomEventTableElem(Dictionary<string, string> data) : base(data)
    {
        id = data["ID"];
        name = data["NAME"]; ;
        eventDesc = data["EVENTDESC"];
        eventCondition = (RandomEventCondition)int.Parse(data["EVENTCONDITION"]);
        eventFrequency = (RandomEventFrequency)int.Parse(data["EVENTFREQUENCY"]);
        eventFrequency2 = int.Parse(data["EVENTFREQUENCY2"]);
        select1Name = data["SELECT1NAME"];
        sucess1Chance = int.Parse(data["SUCESS1CHANCE"]);
        var types1 = data["SUCESS1TYPE"].Split("/");
        foreach(var dt in types1)
        {
            sucess1Type.Add((EventFeedBackType)int.Parse(dt));
        }
        var ids = data["SUCESS1ID"].Split("/");
        foreach (var dt in ids)
        {
            sucess1FeedBackID.Add(int.Parse(dt));
        }
        var vals = data["SUCESS1VAL"].Split("/");
        foreach (var dt in vals)
        {
            if(string.IsNullOrEmpty(dt))
                sucess1Val.Add(int.MinValue);
            else
                sucess1Val.Add(int.Parse(dt));
        }
        sucess1Desc = data["SUCESS1DESC"];
        ////////////////
        var types2 = data["FAIL1TYPE"].Split("/");
        foreach (var dt in types2)
        {
            fail1Type.Add((EventFeedBackType)int.Parse(dt));
        }
        var ids2 = data["FAIL1ID"].Split("/");
        foreach (var dt in ids2)
        {
            fail1FeedBackID.Add(int.Parse(dt));
        }
        var vals2 = data["FAIL1VAL"].Split("/");
        foreach (var dt in vals2)
        {
            if (string.IsNullOrEmpty(dt))
                fail1Val.Add(int.MinValue);
            else
                fail1Val.Add(int.Parse(dt));
        }
        fail1Desc = data["FAIL1DESC"];
        ///////////////////
        select2Name = data["SELECT2NAME"];
        sucess2Chance = int.Parse(data["SUCESS2CHANCE"]);
        var types3 = data["SUCESS2TYPE"].Split("/");
        foreach (var dt in types3)
        {
            sucess2Type.Add((EventFeedBackType)int.Parse(dt));
        }
        var ids3 = data["SUCESS2ID"].Split("/");
        foreach (var dt in ids3)
        {
            sucess2FeedBackID.Add(int.Parse(dt));
        }
        var vals3 = data["SUCESS2VAL"].Split("/");
        foreach (var dt in vals3)
        {
            if (string.IsNullOrEmpty(dt))
                sucess2Val.Add(int.MinValue);
            else
                sucess2Val.Add(int.Parse(dt));
        }
        sucess2Desc = data["SUCESS2DESC"];
        ////////////////////////////////
        var types4 = data["FAIL2TYPE"].Split("/");
        foreach (var dt in types4)
        {
            fail2Type.Add((EventFeedBackType)int.Parse(dt));
        }
        var ids4 = data["FAIL2ID"].Split("/");
        foreach (var dt in ids4)
        {
            fail2FeedBackID.Add(int.Parse(dt));
        }
        var vals4 = data["FAIL2VAL"].Split("/");
        foreach (var dt in vals4)
        {
            if (string.IsNullOrEmpty(dt))
                fail2Val.Add(int.MinValue);
            else
                fail2Val.Add(int.Parse(dt));
        }
        fail2Desc = data["FAIL2DESC"];
        /////////////////////////////////
        select3Name = data["SELECT3NAME"];
        sucess3Chance = int.Parse(data["SUCESS3CHANCE"]);
        var types5 = data["SUCESS3TYPE"].Split("/");
        foreach (var dt in types5)
        {
            sucess3Type.Add((EventFeedBackType)int.Parse(dt));
        }
        var ids5 = data["SUCESS3ID"].Split("/");
        foreach (var dt in ids5)
        {
            sucess3FeedBackID.Add(int.Parse(dt));
        }
        var vals5 = data["SUCESS3VAL"].Split("/");
        foreach (var dt in vals5)
        {
            if (string.IsNullOrEmpty(dt))
                sucess3Val.Add(int.MinValue);
            else
                sucess3Val.Add(int.Parse(dt));
        }
        sucess3Desc = data["SUCESS3DESC"];
        //////////////////////////////////
        var types6 = data["FAIL3TYPE"].Split("/");
        foreach (var dt in types6)
        {
            fail3Type.Add((EventFeedBackType)int.Parse(dt));
        }
        var ids6 = data["FAIL3ID"].Split("/");
        foreach (var dt in ids6)
        {
            fail3FeedBackID.Add(int.Parse(dt));
        }
        var vals6 = data["FAIL3VAL"].Split("/");
        foreach (var dt in vals6)
        {
            if (string.IsNullOrEmpty(dt))
                fail3Val.Add(int.MinValue);
            else
                fail3Val.Add(int.Parse(dt));
        }
        fail3Desc = data["FAIL3DESC"];
    }
}

public class RandomEventTable : DataTableBase
{
    public RandomEventTable() => csvFilePath = "RandomEventTable";
    public override void Load()
    {
        data.Clear();
        var list = Resources.Load<ScriptableObjectDataBase>(csvFilePath);
        foreach(var line in list.sc)
        {
            var elem = new RandomEventTableElem(line);
            data.Add(elem.id, elem);
        }
    }
}
