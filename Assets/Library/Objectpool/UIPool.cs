using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPoolTag
{
    MonsterUI,
}

public class UIPool : CustomObjectPool<UIPoolTag>
{
    public Transform parent;

    protected override GameObject OnCreate(UIPoolTag key)
    {
        var index = (int)key;
        var go = Instantiate(prefabs[index], parent);
        go.SetActive(false);
        return go;
    }

    protected override void OnGet(GameObject go)
    {
        go.SetActive(true);
        go.transform.SetParent(parent);
    }

    protected override void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
}