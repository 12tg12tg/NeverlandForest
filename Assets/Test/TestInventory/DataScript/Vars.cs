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
                var materialTable = DataTableManager.GetTable<AllItemDataTable>();
                userData = new UserData();

                for (int i = 0; i < 16; i++)
                {
                    var newItem = new DataMaterial();
                    newItem.itemId = i;
                    var randId = $"{i}";
                    newItem.itemTableElem = materialTable.GetData<AllItemTableElem>(randId);
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
            }
            return userData;
        }
    }
}
