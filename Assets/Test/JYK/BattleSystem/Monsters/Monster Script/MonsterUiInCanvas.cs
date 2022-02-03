using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUiInCanvas : MonoBehaviour
{
    [Header("최 상단 부모")]
    private Camera uiCamera;
    private Canvas canvas;
    private RectTransform rectParent;
    [SerializeField] private RectTransform ui;

    [HideInInspector] public Vector3 offset = Vector3.zero;
    [HideInInspector] public Transform targetTr;

    [Header("변경할 이미지, 색, 텍스트")]
    public Image rangeColor;
    public TextMeshProUGUI nextMoveDistance;
    public Image iconImage;

    [Header("프로그래스 바 토큰")]
    public HorizontalLayoutGroup shieldLayoutGroup;
    public HorizontalLayoutGroup hpLayoutGroup;
    private List<MonsterProgressToken> shields = new List<MonsterProgressToken>();
    private List<MonsterProgressToken> hps = new List<MonsterProgressToken>();

    [Header("디버프 표시 정보")]
    public List<Image> debuffUIs;

    // Vars
    private bool isInit;
    private bool moveUi;
    public bool UpdateUi { set => moveUi = value; }
    public int ShieldToken
    {
        set
        {
            shieldLayoutGroup.enabled = false;
            for (int i = 0; i < shields.Count; ++i)
            {
                if(i < value)
                    shields[i].TokenOn();
                else
                    shields[i].TokenOff();
            }
        }
    }
    public int HpToken
    {
        set
        {
            hpLayoutGroup.enabled = false;
            for (int i = 0; i < hps.Count; ++i)
            {
                if (i < value)
                    hps[i].TokenOn();
                else
                    hps[i].TokenOff();
            }
        }
    }


    public void Init(Transform target, int maxSheildGaugage, int maxHpGaugage)
    {
        canvas ??= GetComponentInParent<Canvas>();
        uiCamera ??= canvas.worldCamera;
        rectParent ??= canvas.GetComponent<RectTransform>();

        targetTr = target;
        isInit = true;
        SetOriginalDisplay();
        SetProgress(maxSheildGaugage, maxHpGaugage);
        debuffUIs.ForEach(n => n.enabled = false);
    }

    public void SetOriginalDisplay()
    {
        // 트랜스폼의 스케일, 회전값, 이미지 색상, 알파값 조정.
        nextMoveDistance.alpha = 1f;
        iconImage.color = Color.white;
        iconImage.transform.rotation = Quaternion.identity;
        iconImage.transform.localScale.Set(1f, 1f, 1f);
        nextMoveDistance.transform.localScale.Set(1f, 1f, 1f);
    }

    public void SetProgress(int maxSheildGaugage, int maxHpGaugage)
    {
        shields.Clear();
        for (int i = 0; i < maxSheildGaugage; i++)
        {
            var sheildToken = UIPool.Instance.GetObject(UIPoolTag.ProgressToken);
            var tokenScript = sheildToken.GetComponent<MonsterProgressToken>();
            sheildToken.transform.SetParent(shieldLayoutGroup.transform);
            tokenScript.image.color = Color.yellow;
            tokenScript.transform.localScale.Set(1f, 1f, 1f);
            shields.Add(tokenScript);
        }

        hps.Clear();
        for (int i = 0; i < maxHpGaugage; i++)
        {
            var hpToken = UIPool.Instance.GetObject(UIPoolTag.ProgressToken);
            var tokenScript = hpToken.GetComponent<MonsterProgressToken>();
            hpToken.transform.SetParent(hpLayoutGroup.transform);
            tokenScript.image.color = Color.red;
            tokenScript.transform.localScale.Set(1f, 1f, 1f);
            hps.Add(tokenScript);
        }
    }

    public void Release()
    {
        targetTr = null;
        isInit = false;
        shieldLayoutGroup.enabled = true;
        hpLayoutGroup.enabled = true;
        for (int i = 0; i < hps.Count; i++)
        {
            hps[i].TokenOn();
            UIPool.Instance.ReturnObject(UIPoolTag.ProgressToken, hps[i].gameObject);
        }
        for (int i = 0; i < shields.Count; i++)
        {
            shields[i].TokenOn();
            UIPool.Instance.ReturnObject(UIPoolTag.ProgressToken, shields[i].gameObject);
        }
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
                   where n.State != MonsterState.Dead
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
