using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionTest : MonoBehaviour
{
    public RectTransform img1;
    public RectTransform img2;
    public RectTransform bg;
    public Canvas uiCanvas;

    public void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "페이드 아웃"))
        {
            img1.gameObject.SetActive(true);
            StartCoroutine(Utility.FadeIn(img1, bg, uiCanvas));
        }
        if (GUI.Button(new Rect(Screen.width - 105, 300, 100, 100), "페이드 인"))
        {
            img1.gameObject.SetActive(false);
            StartCoroutine(Utility.FadeOut(img2, bg, uiCanvas));
        }
    }
}
