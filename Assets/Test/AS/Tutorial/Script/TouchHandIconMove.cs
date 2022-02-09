using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchHandIconMove : MonoBehaviour
{
    public GameObject circle;
    private Coroutine coMove;
    private Quaternion startQuat = Quaternion.Euler(0f, 0f, -20f);
    private readonly float time = 0.5f;

    private void OnEnable()
    {
        coMove = null;
    }

    private void Update()
    {
        var co = Utility.CoRotateLoop(transform, startQuat, Quaternion.identity, time, () => StartCoroutine(CoScale(time)), () => coMove = null);
        coMove ??= StartCoroutine(co);
    }


    private IEnumerator CoScale(float time)
    {
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + time;
        var startScale = Vector3.one * 0.5f;
        var endScale = Vector3.one;
        var image = circle.GetComponent<Image>();
        var color = Color.white;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            circle.transform.localScale = Vector3.Lerp(startScale, endScale, ratio);
            color.a = Mathf.Lerp(1, 0, ratio); ;
            image.color = color;
            yield return null;
        }
        circle.transform.localScale = endScale;
    }
}
