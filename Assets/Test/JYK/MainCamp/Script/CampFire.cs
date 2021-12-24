using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public Light fireLight;
    private float range;
    private float timer;
    private float time;
    private float originalRange;
    private bool isIncrese;

    private void Awake()
    {
        originalRange = fireLight.range;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        var ratio = timer / time;
        if(!isIncrese)
        {
            var lerp = Mathf.Lerp(1f, 0.7f, ratio);
            fireLight.range = originalRange * lerp;
        }
        else
        {
            var lerp = Mathf.Lerp(0.7f, 1f, ratio);
            fireLight.range = originalRange * lerp;
        }

        if(timer > time)
        {
            timer = 0f;
            time = Random.Range(0.1f, 0.6f);
            isIncrese = !isIncrese;
        }
    }
}
