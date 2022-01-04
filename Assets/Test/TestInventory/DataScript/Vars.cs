using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vars
{
    private static UserData userData;
    private static List<DataConsumable> consumableItemList = new List<DataConsumable>();
    private static List<DataWeapon> weaponItemList = new List<DataWeapon>();

    public static List<DataConsumable> ConsumableItemList
    {
        get
        {
            if (consumableItemList.Count == 0)
            {
                var consumableTable = DataTableManager.GetTable<ConsumableTable>();

                for (int i = 1; i <= consumableTable.data.Count; i++)
                {
                    var newItem = new DataConsumable();
                    newItem.itemId = i;
                    newItem.dataType = DataType.Consume;
                    var Id = $"CON_000{i}";
                    newItem.itemTableElem = consumableTable.GetData<ConsumableTableElem>(Id);
                    consumableItemList.Add(newItem);
                }
            }
            return consumableItemList;
        }
    }

    public static List<DataWeapon> WeaponItemList
    {
        get
        {
            if (weaponItemList.Count == 0)
            {
                var weaponTable = DataTableManager.GetTable<WeaponTable>();

                for (int i = 1; i <= weaponTable.data.Count; i++)
                {
                    var newWeapon = new DataWeapon();
                    newWeapon.itemId = i;
                    newWeapon.dataType = DataType.Weapon;
                    var Id = $"WEA_000{i}";
                    newWeapon.itemTableElem = weaponTable.GetData<WeaponTableElem>(Id);
                    weaponItemList.Add(newWeapon);
                }
            }
            return weaponItemList;
        }
    }

    public static UserData UserData
    {
        get
        {
            if (userData == null)
            {
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

                userData.consumableItemList.AddRange(consumableItemList);
            }
            return userData;
        }
    }
}
