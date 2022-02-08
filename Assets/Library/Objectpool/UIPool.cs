using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPoolTag
{
    MonsterUI, ProgressToken, DamageTxt
}

public class UIPool : CustomObjectPool<UIPoolTag>
{
    public Transform[] parents;

    protected override GameObject OnCreate(UIPoolTag key)
    {
        var index = (int)key;
        var go = Instantiate(prefabs[index], parents[index]);
        go.SetActive(false);
        return go;
    }

    protected override void OnGet(GameObject go, UIPoolTag key)
    {
        var index = (int)key;
        go.SetActive(true);
        go.transform.SetParent(parents[index]);
    }

    protected override void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
}