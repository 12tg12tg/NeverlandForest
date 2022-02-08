using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomObjectPool<T> : Singleton<CustomObjectPool<T>> where T : System.Enum
{
    private const int poolCount = 10;
    protected static bool isInit;
    [SerializeField] protected List<GameObject> prefabs;

    public Dictionary<T, Queue<GameObject>> pool = new Dictionary<T, Queue<GameObject>>();

    public void Init()
    {
        isInit = true;

        var enums = System.Enum.GetValues(typeof(T));

        foreach (T e in enums)
        {
            var queue = new Queue<GameObject>();
            pool.Add(e, queue);

            for (int i = 0; i < poolCount; i++)
            {
                queue.Enqueue(OnCreate(e));
            }
        }
    }

    protected abstract GameObject OnCreate(T key);
    protected abstract void OnGet(GameObject go, T tag);
    protected abstract void OnRelease(GameObject go);
    public GameObject GetObject(T key)
    {
        if (!isInit)
        {
            Init();
            isInit = true;
        }

        var queue = pool[key];
        if (queue.Count > 0)
        {
            var obj = queue.Dequeue();
            OnGet(obj, key);
            return obj;
        }
        else
        {
            var newObj = OnCreate(key);
            OnGet(newObj, key);
            return newObj;
        }
    }
    public void ReturnObject(T key, GameObject go)
    {
        var queue = pool[key];
        OnRelease(go);
        queue.Enqueue(go);
        go.SetActive(false);
    }
}
