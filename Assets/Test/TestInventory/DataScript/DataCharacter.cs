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
            ad += GetWeaponStat(WeaponStat.Damage);
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
    public float Defence
    {
        get
        {
            var Defence = tableElem.ad;
            Defence += GetArmorStat(ArmorStat.Defence);
            return Defence;
        }
    }
    public int Stat_str
    {
        get
        {
            var Stat_str = tableElem.statStr;
            Stat_str += GetWeaponStat(WeaponStat.Stat_str);
            Stat_str += GetArmorStat(ArmorStat.Stat_str);
            return Stat_str;
        }
    }
    public int Stat_dex
    {
        get
        {
            var statDex = tableElem.statDex;
            return statDex;
        }
    }
    public int Stat_int
    {
        get
        {
            var Stat_int = tableElem.statInt;


            Stat_int += GetWeaponStat(WeaponStat.Stat_int);
            Stat_int += GetArmorStat(ArmorStat.Stat_int);
            return Stat_int;
        }
    }
    public int Stat_luk
    {
        get
        {
            var Stat_luk = tableElem.statLuk;

            Stat_luk += GetWeaponStat(WeaponStat.Stat_luk);
            Stat_luk += GetArmorStat(ArmorStat.Stat_luk);
            return Stat_luk;
        }
    }

    // Weapon
    public DataWeapon dataWeapon;

    // Armor
    public List<ArmorTableElem> listDataArmor = new List<ArmorTableElem>();


    public DataCharacter(string charId, string charName)
    {
        var charTable = DataTable.GetTable<CharacterTable>();
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
                case WeaponStat.Damage:
                    stat = dataWeapon.ItemTableElem.damage;
                    break;
                case WeaponStat.Stat_str:
                    stat = dataWeapon.ItemTableElem.stat_str;
                    break;
                case WeaponStat.Stat_int:
                    stat = dataWeapon.ItemTableElem.stat_int;
                    break;
                case WeaponStat.Stat_luk:
                    stat = dataWeapon.ItemTableElem.stat_luk;
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
            case ArmorStat.Defence:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.defence;
                }
                break;
            case ArmorStat.Stat_str:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.stat_str;
                }
                break;
            case ArmorStat.Stat_int:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.stat_int;
                }
                break;
            case ArmorStat.Stat_luk:
                foreach (var armor in listDataArmor)
                {
                    stat += armor.stat_luk;
                }
                break;
        }
        return stat;
    }
}
