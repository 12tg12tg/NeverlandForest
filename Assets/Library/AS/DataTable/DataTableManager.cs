using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    public static Dictionary<System.Type, ScriptableObjects> dic;
    private static bool inited = false;

    public static void Init()
    {
        var armor = Resources.Load<CreateArmorScriptableObject>("");
        var consum = Resources.Load<CreateConsumScriptableObject>("");

        dic.Add(typeof(CreateArmorScriptableObject), armor);
    }

    public static T GetTable<T>() where T: ScriptableObjects
    {
        if(!inited)
        {
            Init();
            inited = false;
        }

        if (!dic.ContainsKey(typeof(T)))
            return default(T);
        return (T)dic[typeof(T)];
    }
}