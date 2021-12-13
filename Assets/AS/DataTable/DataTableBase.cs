using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataTableBase
{
    protected string csvFilePath = string.Empty;
    public Dictionary<string, DataTableElemBase> data = new Dictionary<string, DataTableElemBase>();
    public abstract void Load();
    public int GetSizeCount() => data.Count; // ���̺��� ����� �������� ���� �뵵
    public virtual T GetData<T>(string id) where T : DataTableElemBase
    {
        if (!data.ContainsKey(id))
            return null;
        return data[id] as T;
    }
}