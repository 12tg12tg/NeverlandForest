using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUiInCanvas : MonoBehaviour
{
    private Camera uiCamera;
    private Canvas canvas;
    private RectTransform rectParent;
    [SerializeField] private RectTransform rectHp;

    [HideInInspector] public Vector3 offset = Vector3.zero;
    [HideInInspector] public Transform targetTr;

    public Image rangeColor;
    public Image hpBarImg;
    public Image sheildImg;
    public TextMeshProUGUI nextMoveDistance;
    public Image iconImage;
    private bool isInit;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
    }

    public void Init(Transform target)
    {
        targetTr = target;
        isInit = true;
    }

    public void Release()
    {
        targetTr = null;
        isInit = false;
    }

    private void LateUpdate()
    {
        if (isInit)
        {
            var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);

            if (screenPos.z < 0.0f)
            {
                screenPos *= -1.0f;
            }

            var localPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

            rectHp.localPosition = localPos;
        }
    }
}
