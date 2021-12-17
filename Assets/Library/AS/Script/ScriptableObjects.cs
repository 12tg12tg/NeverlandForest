using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjects : ScriptableObject
{
    private SerializeDictionary<string, DataTableElemBase> sc = new SerializeDictionary<string, DataTableElemBase>();

    private Dictionary<string, DataTableElemBase> dic;
    public Dictionary<string, DataTableElemBase> Dic { get
        {
            if (dic == null)
            {
                dic = new Dictionary<string, DataTableElemBase>();
                foreach (var elem in sc)
                {
                    dic.Add(elem.Key, elem.Value);
                }
            }
            return dic;
        }
    }

    public T GetData<T>(string id) where T: DataTableElemBase
    {
        if (!Dic.ContainsKey(id))
            return null;
        return Dic[id] as T;
    }
}
