using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataTableBase
{
    protected string csvFilePath = string.Empty;
    protected Dictionary<string, DataTableElemBase> data = new Dictionary<string, DataTableElemBase>();

    public Dictionary<string, DataTableElemBase> Data
    {
        get
        {
            return data;
        }
    }
    
    public abstract void Load();

    public T GetData<T>(string id) where T : DataTableElemBase
    {
        if (!data.ContainsKey(id))
            return null;
        return data[id] as T;
    }
}