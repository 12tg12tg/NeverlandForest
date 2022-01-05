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
        miniMap.sizeDelta = new Vector2(size, size);
    }
}
