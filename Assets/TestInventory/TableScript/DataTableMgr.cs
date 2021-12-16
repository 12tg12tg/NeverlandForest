using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DataTableMgr
{
    public static Dictionary<System.Type, DataTableBase> tables =
        new Dictionary<System.Type, DataTableBase>();


    //public static Dictionary<string, LevelTable> levelTables =
    //    new Dictionary<string, LevelTable>();

    private static bool inited = false;

    public static void Init()
    {
        var itemTable = new ConsumableTableInho();
        itemTable.Load();
        tables.Add(typeof(ConsumableTableInho), itemTable);


        var weaponTable = new WeaponTable();
        weaponTable.Load();
        tables.Add(typeof(WeaponTable), weaponTable);

        var armorTable = new ArmorTable();
        armorTable.Load();
        tables.Add(typeof(ArmorTable), armorTable);

        var charTable = new CharacterTable();
        charTable.Load();
        tables.Add(typeof(CharacterTable), charTable);

        inited = true;
    }

    public static T GetTable<T>() where T : DataTableBase
    {
        if (!inited)
            Init();

        var t = typeof(T);
        if (!tables.ContainsKey(t))
            return null;
        return tables[t] as T;
    }
}

