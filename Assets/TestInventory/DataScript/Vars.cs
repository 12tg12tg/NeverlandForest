using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vars
{
    private static UserData userData;

    public static UserData UserData
    {
        get
        {
            if (userData == null)
            {
                var weaponTable = DataTableMgr.GetTable<WeaponTable>();
                var consumalbeTable = DataTableMgr.GetTable<ItemTable>();

                userData = new UserData();
                userData.id = 111;
                userData.nickname = "KANG";

                for (var i = 0; i < 20; ++i)
                {
                    var newWeapon = new DataWeapon();
                    newWeapon.itemId = i;
                    var randId = $"WEA_000{Random.Range(1, 5)}";
                    newWeapon.itemTableElem = weaponTable.GetData<WeaponTableElem>(randId);
                    userData.weaponItemList.Add(newWeapon);
                }

                for (var i = 0; i < 80; ++i)
                {
                    var newItem = new DataCunsumable();
                    newItem.itemId = i;
                    var randId = $"CON_000{Random.Range(1, 8)}";
                    newItem.itemTableElem = consumalbeTable.GetData<ItemTableElem>(randId);
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
