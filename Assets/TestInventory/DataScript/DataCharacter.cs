using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCharacter
{
    public CharacterTableElem tableElem;
    //public LevelTable levelTable;

    public int level = 1;
    public int exp;

    // Stats
    public float Hp
    {
        get
        {
            var hp = tableElem.hp;
            return hp;
        }
    }
    public float Mp
    {
        get
        {
            var mp = tableElem.mp;
            return mp;
        }
    }

    public float Ad
    {
        get
        {
            var ad = tableElem.ad;
            ad += GetWeaponStat(WeaponStat.Ad);
            return ad;
        }
    }
    public float Ap
    {
        get
        {
            var ap = tableElem.ad;
            return ap;
        }
    }
    public float Df
    {
        get
        {
            var df = tableElem.ad;
            df += GetArmorStat(ArmorStat.Df);
            return df;
        }
    }
    public int StatStr
    {
        get
        {
            var statStr = tableElem.statStr;
            statStr += GetWeaponStat(WeaponStat.StatStr);
            statStr += GetArmorStat(ArmorStat.StatStr);
            return statStr;
        }
    }
    public int StatDex
    {
        get
        {
            var statDex = tableElem.statDex;
            return statDex;
        }
    }
    public int StatInt
    {
        get
        {
            var statInt = tableElem.statInt;


            statInt += GetWeaponStat(WeaponStat.StatInt);
            statInt += GetArmorStat(ArmorStat.StatInt);
            return statInt;
        }
    }
    public int StatLuk
    {
        get
        {
            var statLuk = tableElem.statLuk;

            statLuk += GetWeaponStat(WeaponStat.StatLuk);
            statLuk += GetArmorStat(ArmorStat.StatLuk);
            return statLuk;
        }
    }

    // Weapon
    public DataWeapon dataWeapon;

    // Armor
    public List<ArmorTableElem> listDataArmor = new List<ArmorTableElem>();


    public DataCharacter(string charId, string charName)
    {
        var charTable = DataTableManager.GetTable<CharacterTable>();
        tableElem = charTable.GetData<CharacterTableElem>(charId);
        //levelTable = DataTableMgr.GetLevelTable(charName);
    }
    public DataCharacter(CharacterTableElem tableElem)
    {
        this.tableElem = tableElem;
    }

    public int GetWeaponStat(WeaponStat statType)
    {
        int stat = 0;
        if (dataWeapon != null)
        {
            switch (statType)
            {
                case WeaponStat.Ad:
                    stat = dataWeapon.ItemTableElem.damage;
                    break;
                case WeaponStat.StatStr:
                    stat = dataWeapon.ItemTableElem.str;
                    break;
                case WeaponStat.StatInt:
                    stat = dataWeapon.ItemTableElem.intellet;
                    break;
                case WeaponStat.StatLuk:
                    stat = dataWeapon.ItemTableElem.luck;
                    break;
            }
        }
        return stat;
    }

    public int GetArmorStat(ArmorStat statType)
    {
        int stat = 0;
        switch (statType)
        {
            case ArmorStat.Df:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.def;
                }
                break;
            case ArmorStat.StatStr:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.str;
                }
                break;
            case ArmorStat.StatInt:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.intellet;
                }
                break;
            case ArmorStat.StatLuk:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.luck;
                }
                break;
        }
        return stat;
    }
}
