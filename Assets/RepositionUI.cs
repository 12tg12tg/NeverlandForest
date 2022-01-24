using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionUI : MonoBehaviour
{
    public GameObject standardTarget;

    private void Start()
    {
        Resize();
    }

    public void Resize()
    {
        var height = standardTarget.GetComponent<RectTransform>().rect.height;
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, height);
    }
}
