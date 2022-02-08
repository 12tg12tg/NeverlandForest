using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour
{
    public RectTransform img1;
    public RectTransform img2;
    public RectTransform bg;
    public Canvas uiCanvas;

    public void FadeIn()
    {
        img1.gameObject.SetActive(true);
        StartCoroutine(Utility.FadeIn(img1, bg, uiCanvas, () => img1.gameObject.SetActive(false)));
    }
    public void FadeOut()
    {
        bg.gameObject.SetActive(true);

        img2.gameObject.SetActive(true);
        StartCoroutine(Utility.FadeOut(img2, bg, uiCanvas, () => img2.gameObject.SetActive(false)));
    }
}
