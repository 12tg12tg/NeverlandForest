using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DataTableManager
{
    public static Dictionary<System.Type, DataTableBase> tables = 
        new Dictionary<System.Type, DataTableBase>();
    private static bool inited = false;

    public static void Init()
    {
        //var itemTable = new ItemTable();
        //itemTable.Load();
        //tables.Add(typeof(ItemTable), itemTable);
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