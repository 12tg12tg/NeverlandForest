using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vars
{
    private static UserData userData;
    private static List<DataCunsumable> consumableItemList =
        new List<DataCunsumable>();
    private static List<DataWeapon> weaponItemList =
        new List<DataWeapon>();

    public static List<DataCunsumable> ConsumableItemList
    {
        get
        {
            if (consumableItemList.Count == 0)
            {
                var consumableTable = DataTableManager.GetTable<ConsumableTable>();

                for (int i = 1; i <= consumableTable.data.Count; i++)
                {
                    var newItem = new DataCunsumable();
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

                userData = new UserData();
                userData.id = 111;
                userData.nickname = "KANG";

                for (var i = 0; i < 20; ++i)
                {
                    var newWeapon = new DataWeapon();
                    newWeapon.itemId = i;
                    newWeapon.dataType = DataType.Weapon;
                    var randId = $"WEA_000{Random.Range(1, 5)}";
                    newWeapon.itemTableElem = weaponTable.GetData<WeaponTableElem>(randId);
                    userData.weaponItemList.Add(newWeapon);
                }

                for (var i = 0; i < 50; ++i)
                {
                    var newItem = new DataCunsumable();
                    newItem.itemId = i;
                    newItem.dataType = DataType.Consume;
                    var randId = $"CON_000{Random.Range(1, 8)}";
                    newItem.itemTableElem = consumalbeTable.GetData<ConsumableTableElem>(randId);
                    userData.consumableItemList.Add(newItem);
                }

                userData.characterList.Add(new DataCharacter("CHAR_0001", "Diluc"));
                userData.characterList.Add(new DataCharacter("CHAR_0002", "Jean"));

                //userData.characterList[0].dataWeapon = weaponTable.GetData<WeaponTableElem>("WEA_0001");
                //userData.characterList[0].listDataArmor.Add(armorTable.GetData<ArmorTableElem>("DEF_0012"));
                //userData.characterList[0].listDataArmor.Add(armorTable.GetData<ArmorTableElem>("DEF_0002"));
                //userData.characterList[0].listDataArmor.Add(armorTable.GetData<ArmorTableElem>("DEF_0001"));
            }
            return userData;
        }
    }
}
