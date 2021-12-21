using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class DataTableManager
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

        var weaponTable = new WeaponTable();
        weaponTable.Load();
        tables.Add(typeof(WeaponTable), weaponTable);

        var materialTable = new AllItemDataTable();
        materialTable.Load();
        tables.Add(typeof(AllItemDataTable), materialTable);

        var recipeTable = new RecipeDataTable();
        recipeTable.Load();
        tables.Add(typeof(RecipeDataTable), recipeTable);

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