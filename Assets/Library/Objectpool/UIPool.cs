using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPoolTag
{
    MonsterUI, ProgressToken, DamageTxt
}

public class UIPool : CustomObjectPool<UIPoolTag>
{
    protected override GameObject OnCreate(UIPoolTag key)
    {
        var index = (int)key;
        var go = Instantiate(prefabs[index], transform);
        go.SetActive(false);
        return go;
    }

    protected override void OnGet(GameObject go)
    {
        go.SetActive(true);
        go.transform.SetParent(null);
    }

    protected override void OnRelease(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform);
    }
}