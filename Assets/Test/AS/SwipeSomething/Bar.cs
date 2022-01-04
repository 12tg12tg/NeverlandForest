using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler,IPointerMoveHandler
{
    public RectTransform inven;
    private Vector3 startPos;
    public float dragDistance;
    public float power;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");

        //var pos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos - MultiTouch.Instance.PrimaryPos);

        //if (pos.y <= dragDistance && !inven.gameObject.activeSelf)
        //{
        //    inven.gameObject.SetActive(true);
        //    var screenPos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
        //    screenPos.y -= inven.rect.height / 4;
        //    inven.position = screenPos;
        //    startPos = inven.position;
        //}
        //inven.position = new Vector3(0f, -pos.y, 0f) * power + startPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");

        if (inven.gameObject.activeSelf)
            inven.localPosition = Vector3.zero;

        startPos = inven.position;
        MultiTouch.Instance.PrimaryStartPos = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Debug.Log("OnPointerMove");

        var pos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos - MultiTouch.Instance.PrimaryPos);

        if (!inven.gameObject.activeSelf)
        {
            inven.gameObject.SetActive(true);
            var screenPos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
            screenPos.y -= inven.rect.height / 4;
            inven.position = screenPos;
            startPos = inven.position;
        }
        inven.position = new Vector3(0f, -pos.y, 0f) * power + startPos;
    }
}
