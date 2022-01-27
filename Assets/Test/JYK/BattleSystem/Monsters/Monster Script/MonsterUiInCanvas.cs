using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUiInCanvas : MonoBehaviour
{
    private Camera uiCamera;
    private Canvas canvas;
    private RectTransform rectParent;
    [SerializeField] private RectTransform ui;

    [HideInInspector] public Vector3 offset = Vector3.zero;
    [HideInInspector] public Transform targetTr;

    public Image rangeColor;
    public Image hpBarImg;
    public Image sheildImg;
    public TextMeshProUGUI nextMoveDistance;
    public Image iconImage;
    private bool isInit;

    private bool moveUi;
    public bool UpdateUi { set => moveUi = value; }

    public void Init(Transform target)
    {
        canvas ??= GetComponentInParent<Canvas>();
        uiCamera ??= canvas.worldCamera;
        rectParent ??= canvas.GetComponent<RectTransform>();

        targetTr = target;
        isInit = true;
        SetNaturalState();
    }

    public void SetNaturalState()
    {
        // 트랜스폼의 스케일, 회전값, 이미지 색상, 알파값 조정.
        nextMoveDistance.alpha = 1f;
        iconImage.color = Color.yellow;
        iconImage.transform.rotation = Quaternion.identity;
        iconImage.transform.localScale.Set(1f, 1f, 1f);
        nextMoveDistance.transform.localScale.Set(1f, 1f, 1f);
    }

    public void Release()
    {
        targetTr = null;
        isInit = false;

    }

    public void RepositionUi()
    {
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);

        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out Vector2 localPos);

        ui.localPosition = localPos;
    }

    public void MoveUi()
    {
        RepositionUi();
        RePositionAllUi();
    }

    public void RePositionAllUi()
    {
        var monsters = BattleManager.Instance.monsters;
        var list = from n in monsters
                   select n.uiLinker.linkedUi;
        foreach (var ui in list)
        {
            ui.RepositionUi();
        }
    }

    private void LateUpdate()
    {
        if (isInit)
        {
            if (moveUi)
            {
                MoveUi();
            }
        }

    }
}
