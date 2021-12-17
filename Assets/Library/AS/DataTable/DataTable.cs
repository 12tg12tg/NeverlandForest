using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class DataTable
{
    public static Dictionary<System.Type, DataTableBase> tables = 
        new Dictionary<System.Type, DataTableBase>();
    private static bool inited = false;

    public static void Init()
    {
        tables.Clear();

        var itemTable = new ConsumableTable();
        itemTable.Load();
        tables.Add(typeof(ConsumableTable), itemTable);

        var armorTable = new ArmorTable();
        armorTable.Load();
        tables.Add(typeof(ArmorTable), armorTable);
    }

    public static T GetTable<T>() where T : DataTableBase
    {
        if (!inited)
        {
            Init();
            inited = true;
        }

        if (!tables.ContainsKey(typeof(T)))
            return null;
        return (T)tables[typeof(T)];
    }
}