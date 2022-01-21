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
            if(boySkillList == null)
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
    public static readonly int maxStamina = 100; // ȸ�� ���¹̳� ����� ������ �ʴ� ���¹̳�ĭ
    public static readonly float hunterMaxHp = 100;
    public static readonly float herbalistMaxHp = 100;
    public static readonly float lanternMaxCount = 18;

    // Localization
    public static Localization localization = Localization.Korean;

    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!  ���ӿ��� ���Ǵ� ���� ������  !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    private static UserData userData;
    public static UserData UserData
    {
        get
        {
            if (userData == null)
            {
                //�׽�Ʈ�� ����Ʈ
                var weaponTable = DataTableManager.GetTable<WeaponTable>();
                var consumalbeTable = DataTableManager.GetTable<ConsumableTable>();
                var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
                userData = new UserData();

                for (int i = 0; i < 16; i++)
                {
                    var newItem = new DataMaterial();
                    newItem.itemId = i;
                    var randId = $"{i}";
                    newItem.itemTableElem = allItemTable.GetData<AllItemTableElem>(randId);
                    userData.HaveMaterialList.Add(newItem);
                }

                //�׽�Ʈ�� ����Ʈ
                var consumableTable = DataTableManager.GetTable<ConsumableTable>();

                for (int i = 1; i <= consumableTable.data.Count; i++)
                {
                    var newItem = new DataConsumable();
                    newItem.itemId = i;
                    newItem.dataType = DataType.Consume;
                    var Id = $"CON_000{i}";
                    newItem.itemTableElem = consumableTable.GetData<ConsumableTableElem>(Id);
                    userData.ConsumableItemList.Add(newItem);
                }

                // �κ��丮���� �׽�Ʈ�� ����� ������ ����Ʈ
                int tempItemNum = 0;
                for (int i = 0; i < 11; i++)
                {
                    var newItem = new DataAllItem();
                    newItem.itemId = tempItemNum;
                    newItem.LimitCount = Random.Range(5, 6);
                    newItem.OwnCount = Random.Range(1, 7);
                    newItem.dataType = DataType.AllItem;
                    var stringId = $"{tempItemNum}";
                    newItem.itemTableElem = allItemTable.GetData<AllItemTableElem>(stringId);
                    tempItemNum += 1;

                    userData.AddItemData(newItem);
                }
            }
            return userData;
        }
    }
    public static void MakeSkillList()
    {
        /*�÷��̾� ��ų ��� ����*/
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