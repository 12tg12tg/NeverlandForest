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
    protected override GameObject OnCreate(TrapTag key)
    {
        var index = (int)key;
        var go = Instantiate(prefabs[index], transform);
        go.SetActive(false);
        return go;
    }

    protected override void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    protected override void OnRelease(GameObject go)
    {
        go.transform.SetParent(transform);
        go.SetActive(false);
    }
}
