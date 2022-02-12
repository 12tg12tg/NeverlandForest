using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SkillButtonType
{
    None, Swap, Info,
}

public class BottomSkillButtonUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BattleManager bm;
    private BottomUIManager bottomUiManager;

    [Header("자신의 버튼 연결")]
    public Button ownButton;


    [Header("스킬 및 재료 정보 UI 연결")]
    public DataPlayerSkill skill;                       // 담고있는 스킬정보
    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costItemCount;

    [Header("덮개 및 하단 버튼 연결")]
    [SerializeField] private Button cover;
    [SerializeField] private Button below;              // 활성화/비활성화할 버튼

    [Header("덮개 크기 계산을 위한 연결")]
    [SerializeField] private RectTransform coverRt;
    private CanvasScaler cs;
    private float height;
    private Vector2 openOffset;                         // Open/Close용 크기 계산
    private bool isCalculateOffset;
    public SkillButtonType buttonType;

    // Property
    private Vector2 CoverOrigianlPos { get; set; }
    private Vector2 CoverOpenPos { get; set; }

    public void Init() // 제일 앞 스킬 버튼 (부여된 스킬 없는) 초기화
    {
        bm ??= BattleManager.Instance;
        bottomUiManager ??= BottomUIManager.Instance;
        cover.interactable = true;
        below.interactable = false;

        if (bm == null)
            return;

        if(buttonType == SkillButtonType.Swap)
        {
            var arrow = bm.costLink.GetCurrentArrowElem();
            skillImg.sprite = arrow.IconSprite;
            costItemCount.text = bm.costLink.NumberOfArrows().ToString();
        }
        else if(buttonType == SkillButtonType.Info)
        {
            var oil = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_19");
            skillImg.sprite = oil.IconSprite;
            costItemCount.text = bm.costLink.NumberOfOil().ToString();
        }
    }

    public void UpdateCostInfo()
    {
        if (buttonType == SkillButtonType.Swap)
        {
            var arrow = bm.costLink.GetCurrentArrowElem();
            skillImg.sprite = arrow.IconSprite;
            costItemCount.text = bm.costLink.NumberOfArrows().ToString();
        }
        else if (buttonType == SkillButtonType.Info)
        {
            costItemCount.text = bm.costLink.NumberOfOil().ToString();
        }
    }

    public void Init(DataPlayerSkill skill) // 스킬을 지닌 버튼 초기화
    {
        bm ??= BattleManager.Instance;
        bottomUiManager ??= BottomUIManager.Instance;

        cover.interactable = true;
        below.interactable = false;

        this.skill = skill;
        skillImg.sprite = skill.SkillTableElem.IconSprite;
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
        if (buttonType == SkillButtonType.Info)
            return;

        ownButton.interactable = false;
        skillImg.color = new Color(0.4f, 0.4f, 0.4f);
    }

    public void MakeClickable()
    {
        ownButton.interactable = true;
        skillImg.color = Color.white;
    }

    public void SwapArrow()
    {
        var otherArrow = bm.costLink.GetOtherArrowElem();

        bool haveOne = false;

        if ((Vars.UserData.arrowType == ArrowType.Iron && bm.costLink.HaveArrowNow())
            || (Vars.UserData.arrowType == ArrowType.Normal && bm.costLink.HaveIronArrowNow()))
        {
            haveOne = true;
        }

        // 다른 화살이 있다면
        if(haveOne)
        {
            skillImg.sprite = otherArrow.IconSprite;
            if (Vars.UserData.arrowType == ArrowType.Normal)
                Vars.UserData.arrowType = ArrowType.Iron;
            else
                Vars.UserData.arrowType = ArrowType.Normal;
            UpdateCostInfo();
        }
    }

    public void IntoSkillStage() // 버튼
    {
        // 튜토리얼
        if (bm.isTutorial && bm.tutorial.lockSkillButtonClick)
            return;

        // 스킬버튼이 아닌 경우 예외처리.
        if (buttonType == SkillButtonType.Swap)
        {
            SwapArrow();
            return;
        }
        else if(buttonType == SkillButtonType.Info)
            return;

        // 튜토리얼
        if (bm.isTutorial && !bm.tutorial.tu_07_BoySkill1)
            bm.tutorial.tu_07_BoySkill1 = true;
        else if (bm.isTutorial && !bm.tutorial.tu_10_GirlSkill1)
            bm.tutorial.tu_10_GirlSkill1 = true;
        else if (bm.isTutorial && !bm.tutorial.tu_12_GirlSkill1)
            bm.tutorial.tu_12_GirlSkill1 = true;

        // 턴 종료 버튼 비활성화
        bm.uiLink.turnSkipButton.interactable = false;

        // 계산 안되있으면 첫 계산.
        if (!isCalculateOffset)
            CalculateOffset();

        // 스킬 정보 갱신
        bottomUiManager.info.Init(skill);
        if (!bottomUiManager.IsSkillLock)
        {
            // 스킬에 대한 소모값이 부족한 경우.
            if (!bm.costLink.CheckCanUseSkill(skill, out string cautionMessage))
            {
                var offset = new Vector2(0f, height * 1 / 8);
                var alittleOpenPos = CoverOrigianlPos + offset;

                bm.uiLink.PrintCaution(cautionMessage, 0.75f, 0.3f, null);

                // 깔짝
                StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, alittleOpenPos, 0.15f,
                    () => { StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOrigianlPos, 0.15f, null)); }));
            }
            else
            {
                cover.interactable = false;
                bottomUiManager.IntoSkillState(this);
                StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOpenPos, 0.3f,
                    () => { below.interactable = true; }));

            }
        }
    }

    public void Cancle() // 버튼
    {
        if (bm.isTutorial && bm.tutorial.lockSkillButtonClick)
            return;
        // 턴 종료 버튼 다시 활성화
        bm.uiLink.turnSkipButton.interactable = true;
        below.interactable = false;
        bm.uiLink.turnSkipButton.interactable = true;
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

        if (!ownButton.interactable)
            return;

        if (buttonType != SkillButtonType.None)
            return;

        if (bm.isTutorial && bm.tutorial.lockSkillButtonDrag)
            return;

        bottomUiManager.info.Init(skill);
        if (!bottomUiManager.IsSkillLock)
        {
            // 스킬에 대한 소모값이 없는 경우.
            if (!bm.costLink.CheckCanUseSkill(skill, out string cautionMessage))
            {
                var offset = new Vector2(0f, height * 1 / 8);
                var alittleOpenPos = CoverOrigianlPos + offset;

                bm.uiLink.PrintCaution(cautionMessage, 0.75f, 0.3f, null);

                StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, alittleOpenPos, 0.15f,
                    () => { StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOrigianlPos, 0.15f,
                                () => bm.dragLink.Release())); }));
            }
            else
            {
                bm.dragLink.Init(this);

                cover.interactable = false;
                bottomUiManager.IntoSkillState(this);
                StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOpenPos, 0.3f,
                    () => { below.interactable = true; }));
            }
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
        //Debug.Log($"마지막 드랍 타일 : {bm.dragLink.lastDrapTile.index}");
        //Debug.Log($"마지막 드랍 타일의 어느쪽? : {bm.dragLink.lastDrapTile.WhichPartOfTile(bm.dragLink.lastDragWorldPos)}");

        if (bm.dragLink.lastDrapTile == null)
        {
            bottomUiManager.curSkillButton?.Cancle();
        }
        else if (bm.dragLink.lastDrapTile.isHighlight || bm.dragLink.lastDrapTile.isHighlightConsume)
        {
            bm.DoCommand(skill.SkillTableElem.player, bm.dragLink.lastDrapTile.index, skill, true);

            bottomUiManager.curSkillButton.Cancle_UseSkill();
            bottomUiManager.InteractiveSkillButton(skill.SkillTableElem.player, false);

            var progressIcon = skill.SkillTableElem.player == PlayerType.Boy ? BattleUI.ProgressIcon.Boy : BattleUI.ProgressIcon.Girl;
            bm.uiLink.UpdateProgress(progressIcon);
        }
        else if (!bm.dragLink.lastDrapTile.isHighlight && !bm.dragLink.lastDrapTile.isHighlightConsume)
        {
            if(bottomUiManager.curSkillButton != null)
                bottomUiManager.curSkillButton.Cancle();
        }
        bm.dragLink.Release();
    }
}
