using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vars
{
    //Skill Info
    public static List<DataPlayerSkill> BoySkillList { get; set; } = new List<DataPlayerSkill>();
    public static List<DataPlayerSkill> GirlSkillList { get; set; } = new List<DataPlayerSkill>();

    public static int maxIngameHour = 24;
    public static int maxIngameMinute = 60;
    public static int maxStamina = 100; // 회색 스태미나 절대로 변하지 않는 스태미너칸
    public static float hunterMaxHp = 100;
    public static float herbalistMaxHp = 100;
    public static int LanternMaxCount = 18;
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
                //테스트용 리스트
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

                //테스트용 리스트
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

                // 인벤토리에서 테스트로 사용할 아이템 리스트
                int tempItemNum = 3;
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
}
//if (!userData.HaveAllItemList2.ContainsKey(newItem.ItemTableElem.name))
//{
//    userData.HaveAllItemList2.Add(newItem.ItemTableElem.name, newItem);
//}
//else
//{
//    userData.HaveAllItemList2[newItem.ItemTableElem.name].OwnCount++;
//}