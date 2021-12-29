using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SwipeSomething : MonoBehaviour
{
    public TMP_Text tmp;
    public GameObject inventory;
    private RectTransform inven;


    private bool isChange;

    [Header("Swipe")]
    public float swipeDistance;
    public float tiemr;
    private readonly float minlocation = 0.25f;
    private readonly float maxlocation = 0.75f;
    private Coroutine coMove;

    [Header("Other")]
    public float otherDistance;
    public float power;
    private Vector3 startPos;
    private float startY;

    private void Awake()
    {
        inven = inventory.transform.GetChild(0) as RectTransform;
        startPos = inven.localPosition;
        startY = startPos.y;
    }
    private void Update()
    {
        if (isChange)
        {
            Swipe();
        }
        else
        {
            Other();
        }
    }

    public void OnChange() => isChange = !isChange;
    private void Swipe()
    {
        var pos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos);

        if (pos.x >= minlocation && pos.x <= maxlocation &&
            pos.y >= minlocation && pos.y <= maxlocation)
        {
            var swipe = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.Swipe);

            if (swipe.y >= swipeDistance && !inven.gameObject.activeSelf)
            {
                inven.gameObject.SetActive(true);
                if (coMove == null)
                {
                    var startPos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
                    startPos.y -= inven.rect.height / 4;
                    inven.position = startPos;
                    tmp.text = "Swipe";
                }
                coMove ??= StartCoroutine(Utility.CoTranslate(inven, inven.localPosition, Vector3.zero, tiemr, () => coMove = null));

                MultiTouch.Instance.PrimaryStartPos = Vector2.zero; // 일단 임시로 해결방안 찾으면 바꿀 예정
            }
        }
    }
    private void Other()
    {
        var startPos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos);
        if (MultiTouch.Instance.TouchCount > 0)
        {
            if (startPos.x >= minlocation && startPos.x <= maxlocation &&
                startPos.y >= minlocation && startPos.y <= maxlocation)
            {
                var pos = Camera.main.ScreenToViewportPoint(MultiTouch.Instance.PrimaryStartPos - MultiTouch.Instance.PrimaryPos);

                if (pos.y <= otherDistance && !inven.gameObject.activeSelf)
                {
                    inven.gameObject.SetActive(true);
                    var screenPos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
                    screenPos.y -= inven.rect.height / 4;
                    inven.position = screenPos;
                    this.startPos = inven.position;
                    tmp.text = "Other";
                }
                inven.position = new Vector3(0f, -pos.y, 0f) * power + this.startPos;
            }
        }
        else
        {
            if (inven.gameObject.activeSelf)
                inven.localPosition = Vector3.zero;

            this.startPos = inven.position;
            MultiTouch.Instance.PrimaryStartPos = Vector2.zero; // 일단 임시로 해결방안 찾으면 바꿀 예정
        }
    }
}