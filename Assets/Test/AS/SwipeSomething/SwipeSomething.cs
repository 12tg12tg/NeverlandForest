using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeSomething : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject inventory;
    public GameObject panel;
    public GameObject bar;
    private RectTransform inven;

    [Header("BarDrag")]
    public float dragDistance;
    public float openDistance;
    public float power;
    private Vector3 startPos;

    private void Awake()
    {
        var rt = bar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(canvas.rect.width * 0.2f, canvas.rect.height * 0.015f);
        
        inven = inventory.transform.GetChild(0) as RectTransform;
        startPos = inven.localPosition;
    }
    private void Update()
    {
        Bar();
    }
    
    public void Bar()
    {
        var startPos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos);
        //Debug.Log(startPos);
        if (MultiTouch.Instance.TouchCount > 0)
        {
            if (startPos.x >= 0.4f && startPos.x <= 0.6f &&
                startPos.y <= 0.04f)
            {
                var pos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos - MultiTouch.Instance.PrimaryPos);

                if (pos.y <= -dragDistance && !inven.gameObject.activeInHierarchy)
                {
                    bar.SetActive(false);
                    inventory.SetActive(true);
                    panel.SetActive(true);
                    inven.gameObject.SetActive(true);
                    var screenPos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
                    screenPos.y -= inven.rect.height / 4;
                    inven.position = screenPos;
                    this.startPos = inven.position;
                }
                inven.position = new Vector3(0f, -pos.y, 0f) * power + this.startPos;
            }
        }
        else if(inven.gameObject.activeInHierarchy)
        {
            if (inven.localPosition.y > -openDistance)
                inven.localPosition = Vector3.zero;
            else
            {
                inven.gameObject.SetActive(false);
                inventory.SetActive(false);
                panel.SetActive(false);
                bar.SetActive(true);
            }

            this.startPos = inven.position;
            MultiTouch.Instance.PrimaryStartPos = Vector2.zero;
        }
    }
}