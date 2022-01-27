using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestTextUI : MonoBehaviour
{
    public RectTransform guiTarget;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 100), "Co UI"))
        {
            StartCoroutine(CoScaleChange(guiTarget, new Vector3(10, 10, 10), 5f, null));
        }
    }


    public static IEnumerator CoScaleChange(RectTransform rt, Vector3 destScale, float time, UnityAction action = null)
    {
        float startTime = Time.realtimeSinceStartup;
        float destTime = Time.realtimeSinceStartup + time;

        Vector3 startScale = rt.localScale;
        while (Time.realtimeSinceStartup < destTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            var lerp = Vector3.Lerp(startScale, destScale, ratio);
            lerp.z = 0f;
            rt.localScale = lerp;
            yield return null;
        }
        rt.localScale = destScale;
        action?.Invoke();
    }
}
