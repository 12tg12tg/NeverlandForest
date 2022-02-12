using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Localization { Korean, English }

public static class Vars
{
    // Skill Info
    public static List<DataPlayerSkill> boySkillList;
    public static List<DataPlayerSkill> girlSkillList;
    public static List<DataPlayerSkill> BoySkillList
    {
        get
        {
            if (boySkillList == null)
            {
                MakeSkillList();
            }
            return boySkillList;
        }
    }
    public static List<DataPlayerSkill> GirlSkillList
    {
        get
        {
            if (girlSkillList == null)
            {
                MakeSkillList();
            }
            return girlSkillList;
        }
    }

    // Vars
    public static readonly int maxIngameHour = 24;
    public static readonly int maxIngameMinute = 60;
    public static readonly int maxStamina = 100; // 회색 스태미나 절대로 변하지 않는 스태미너칸
    public static /*readonly*/ float maxHp = 30f;
    public static readonly float lanternMaxCount = 18;

    // Localization
    public static Localization localization = Localization.Korean;

    


    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!  게임에서 사용되는 공통 변수들  !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    private static UserData userData;
    public static UserData UserData
    {
        get
        {
            if (userData == null)
            {
                userData = new UserData();
                userData.UserItemInit();
            }
            return userData;
        }
    }
    public static void MakeSkillList()
    {
        /*플레이어 스킬 목록 구성*/
        var boy = Vars.boySkillList = new List<DataPlayerSkill>();
        var girl = Vars.girlSkillList = new List<DataPlayerSkill>(); ;

        var skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("0");
        var data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("1");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("2");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("3");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("4");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("5");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("6");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("7");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("8");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("9");
        data = new DataPlayerSkill(skill);
        girl.Add(data);
    }
}