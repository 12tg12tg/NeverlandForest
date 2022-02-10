using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Production : MonoBehaviour
{
    public RectTransform img1;
    public RectTransform img2;
    public RectTransform bg;
    public Canvas uiCanvas;
    public void FadeIn(UnityAction action = null)
    {
        img1.gameObject.SetActive(true);
        StartCoroutine(Utility.FadeIn(img1, bg, uiCanvas, () => { action?.Invoke(); img1.gameObject.SetActive(false); }));
    }
    public void FadeOut(UnityAction action = null)
    {
        bg.gameObject.SetActive(true);

        img2.gameObject.SetActive(true);
        StartCoroutine(Utility.FadeOut(img2, bg, uiCanvas, () => { action?.Invoke(); img2.gameObject.SetActive(false); }));
    }
}
