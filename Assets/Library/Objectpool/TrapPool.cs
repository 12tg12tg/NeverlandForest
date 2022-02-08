using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapTag
{
    Snare,
    BoobyTrap,
    WoodenTrap,
    ThornTrap,
    Fence,
}

public class TrapPool : CustomObjectPool<TrapTag>
{
    public Transform[] parents;
    protected override GameObject OnCreate(TrapTag key)
    {
        var index = (int)key;
        var go = Instantiate(prefabs[index], transform);
        go.SetActive(false);
        return go;
    }

    protected override void OnGet(GameObject go, TrapTag key)
    {
        var index = (int)key;
        go.SetActive(true);
        go.transform.SetParent(parents[index]);
    }

    protected override void OnRelease(GameObject go)
    {
        go.transform.SetParent(transform);
        go.SetActive(false);
    }
}
