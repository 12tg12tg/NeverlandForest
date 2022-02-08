using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterPoolTag
{
    Mushbae, Mushbro,
    Seed_foot, Flower_pong,
    Mini_web, Spikeder,
    Halloween_bat, Bloody_bat,
    Bay, Gust, Wrath,
    Peekabae, Peekaboo, Pride,
    Fear, Envy, Nightmare,
}

public class MonsterPool : CustomObjectPool<MonsterPoolTag>
{
    public Transform[] parents;
    protected override GameObject OnCreate(MonsterPoolTag key)
    {
        var index = (int)key;
        var go = Instantiate(prefabs[index], transform);
        go.SetActive(false);
        return go;
    }

    protected override void OnGet(GameObject go, MonsterPoolTag key)
    {
        var index = (int)key;
        go.SetActive(true);
        go.transform.SetParent(parents[index]);
        var col = go.GetComponent<Collider>();
        col.enabled = false;
        var anim = go.GetComponentInChildren<Animator>();
        anim.Rebind();
    }

    protected override void OnRelease(GameObject go)
    {
        go.transform.SetParent(transform);
        go.SetActive(false);
    }
}