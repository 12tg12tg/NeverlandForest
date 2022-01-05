using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vars
{
    //Skill Info
    public static List<DataPlayerSkill> BoySkillList { get; set; } = new List<DataPlayerSkill>();
    public static List<DataPlayerSkill> GirlSkillList { get; set; } = new List<DataPlayerSkill>();

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
                int tempItemNum = 2;
                for (int i = 0; i < 2; i++)
                {
                    var newItem = new DataAllItem();
                    newItem.itemId = tempItemNum;
                    newItem.LimitCount = Random.Range(5, 6);
                    newItem.OwnCount = 27;
                    var stringId = $"{tempItemNum}";
                    newItem.itemTableElem = allItemTable.GetData<AllItemTableElem>(stringId);
                    userData.HaveAllItemList.Add(newItem);
                    if (!userData.HaveAllItemList2.ContainsKey(newItem.ItemTableElem.name))
                    {
                        userData.HaveAllItemList2.Add(newItem.ItemTableElem.name, newItem);
                    }
                    else
                    {
                        userData.HaveAllItemList2[newItem.ItemTableElem.name].OwnCount++;
                    }
                    tempItemNum += 1;
                }
            }
            return userData;
        }
    }
}
