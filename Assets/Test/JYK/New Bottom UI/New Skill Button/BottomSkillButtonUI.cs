using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomSkillButtonUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BattleManager bm;
    private BottomUIManager bottomUiManager;
    public Button ownButton;

    public DataPlayerSkill skill;                       // 담고있는 스킬정보

    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costTxt;   // 스킬 정보를 바탕으로 구성

    [SerializeField] private Button cover;
    [SerializeField] private Button below;              // 활성화/비활성화할 버튼

    [SerializeField] private RectTransform coverRt;
    private CanvasScaler cs;
    private float height;
    private Vector2 openOffset;                         // Open/Close용 크기 계산
    private bool isCalculateOffset;

    // Property
    private Vector2 CoverOrigianlPos { get; set; }
    private Vector2 CoverOpenPos { get; set; }

    public void Init(DataPlayerSkill skill)
    {
        bm ??= BattleManager.Instance;
        bottomUiManager ??= BottomUIManager.Instance;
        cover.interactable = true;
        below.interactable = false;

        this.skill = skill;
        skillImg.sprite = skill.SkillTableElem.IconSprite;
        costTxt.text = skill.SkillTableElem.cost.ToString();
    }

    public void CalculateOffset()
    {
        isCalculateOffset = true;

        cs = GetComponentInParent<CanvasScaler>();
        var size = Utility.RelativeRectSize(cs, coverRt);
        height = size.y;
        openOffset = new Vector2(0f, height * 3 / 8);
        CoverOrigianlPos = coverRt.anchoredPosition;
        CoverOpenPos = CoverOrigianlPos + openOffset;
    }

    public void MakeUnclickable()
    {
        ownButton.interactable = false;
        skillImg.color = new Color(0.4f, 0.4f, 0.4f);
    }

    public void MakeClickable()
    {
        ownButton.interactable = true;
        skillImg.color = Color.white;
    }

    public void IntoSkillStage() // 버튼
    {
        if(!isCalculateOffset)
            CalculateOffset();

        bottomUiManager.info.Init(skill);

        if(!bottomUiManager.IsSkillLock)
        {
            cover.interactable = false;
            bottomUiManager.IntoSkillState(this);
            StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOpenPos, 0.3f,
                () => { below.interactable = true; }));
        }
    }

    public void Cancle() // 버튼
    {
        below.interactable = false;
        bottomUiManager.ExitSkillState(true);
        StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOrigianlPos, 0.3f,
            () => { cover.interactable = true; }));
    }

    public void Cancle_UseSkill()
    {
        below.interactable = false;
        bottomUiManager.ExitSkillState(false);
        StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOrigianlPos, 0.3f, null));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isCalculateOffset)
            CalculateOffset();

        if (!bottomUiManager.IsSkillLock)
        {
            bm.dragLink.Init(this);
            bottomUiManager.info.Init(skill);

            cover.interactable = false;
            bottomUiManager.IntoSkillState(this);
            StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOpenPos, 0.3f,
                () => { below.interactable = true; }));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (bm.dragLink.isDrag)
        {
            bm.dragLink.UpdatePos(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (bm.dragLink.lastDrapTile == null)
        {
            bottomUiManager.curSkillButton?.Cancle();
        }
        else if (bm.dragLink.lastDrapTile.isHighlight)
        {
            bm.DoCommand(skill.SkillTableElem.player, bm.dragLink.lastDrapTile.index, skill);

            bottomUiManager.curSkillButton.Cancle_UseSkill();
            bottomUiManager.InteractiveSkillButton(skill.SkillTableElem.player, false);

            bm.uiLink.UpdateProgress();
        }
        else if (!bm.dragLink.lastDrapTile.isHighlight)
        {
            bottomUiManager.curSkillButton.Cancle();
        }
        bm.dragLink.Release();
    }
}
