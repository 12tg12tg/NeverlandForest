using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Production : MonoBehaviour
{
    public RectTransform img1;
    public RectTransform img2;
    public RectTransform bg;
    public GameObject black;
    public Canvas uiCanvas;

    public void FadeIn(UnityAction action = null)
    {
        img1.gameObject.SetActive(true);

        StartCoroutine(Utility.FadeIn(img1, bg, uiCanvas, () => { 
            img1.gameObject.SetActive(false); 
            black.SetActive(true);
            action?.Invoke();
            Debug.Log("페이드 인 끝");
        }));
    }
    public void FadeOut(UnityAction action = null)
    {
        black.SetActive(false);
        bg.gameObject.SetActive(true);
        img2.gameObject.SetActive(true);

        StartCoroutine(Utility.FadeOut(img2, bg, uiCanvas, () => { 
            action?.Invoke(); 
            img2.gameObject.SetActive(false);
            Debug.Log("페이드 아웃 끝");
        }));
    }
}
