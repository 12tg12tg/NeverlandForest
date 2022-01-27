using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeMiniMap : MonoBehaviour
{
    public RectTransform miniMap;
    public float defaultPercentage;

    private void Awake()
    {
        Resize(defaultPercentage);
    }

    public void Resize(float percentage)
    {
        var toggle = miniMap.GetComponent<Toggle>();
        var rt = GetComponent<RectTransform>();
        var size = toggle.isOn ? rt.rect.height * percentage : rt.rect.width * defaultPercentage;
        //var rect = miniMap.rect;
        var rect = GetComponent<RectTransform>().rect;

        miniMap.anchoredPosition = toggle.isOn ? new Vector2(rect.width * 0.5f, -rect.height * 0.5f) : miniMap.anchoredPosition;
        Debug.Log($"{Screen.width} , {Screen.height}");
        miniMap.sizeDelta = new Vector2(size, size);
    }
}
