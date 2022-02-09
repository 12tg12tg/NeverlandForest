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

    [Header("�ڽ��� ��ư ����")]
    public Button ownButton;


    [Header("��ų �� ��� ���� UI ����")]
    public DataPlayerSkill skill;                       // ����ִ� ��ų����
    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costItemCount;

    [Header("���� �� �ϴ� ��ư ����")]
    [SerializeField] private Button cover;
    [SerializeField] private Button below;              // Ȱ��ȭ/��Ȱ��ȭ�� ��ư

    [Header("���� ũ�� ����� ���� ����")]
    [SerializeField] private RectTransform coverRt;
    private CanvasScaler cs;
    private float height;
    private Vector2 openOffset;                         // Open/Close�� ũ�� ���
    private bool isCalculateOffset;
    public SkillButtonType buttonType;

    // Property
    private Vector2 CoverOrigianlPos { get; set; }
    private Vector2 CoverOpenPos { get; set; }

    public void Init() // ���� �� ��ų ��ư (�ο��� ��ų ����) �ʱ�ȭ
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

    public void Init(DataPlayerSkill skill) // ��ų�� ���� ��ư �ʱ�ȭ
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

        // �ٸ� ȭ���� �ִٸ�
        if(haveOne)
        {
            skillImg.sprite = otherArrow.IconSprite;
            if (Vars.UserData.arrowType == ArrowType.Normal)
                Vars.UserData.arrowType = ArrowType.Iron;
            else
                Vars.UserData.arrowType = ArrowType.Normal;
            UpdateCostInfo();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
        }
    }

    public void IntoSkillStage() // ��ư
    {
        // Ʃ�丮��
        if (bm.isTutorial && bm.tutorial.lockSkillButtonClick)
            return;

        // ��ų��ư�� �ƴ� ��� ����ó��.
        if (buttonType == SkillButtonType.Swap)
        {
            SwapArrow();
            return;
        }
        else if(buttonType == SkillButtonType.Info)
            return;

        // Ʃ�丮��
        if (bm.isTutorial && !bm.tutorial.tu_07_BoySkill1)
            bm.tutorial.tu_07_BoySkill1 = true;
        else if (bm.isTutorial && !bm.tutorial.tu_10_GirlSkill1)
            bm.tutorial.tu_10_GirlSkill1 = true;
        else if (bm.isTutorial && !bm.tutorial.tu_12_GirlSkill1)
            bm.tutorial.tu_12_GirlSkill1 = true;

        // ��� �ȵ������� ù ���.
        if (!isCalculateOffset)
            CalculateOffset();

        // ��ų ���� ����
        bottomUiManager.info.Init(skill);
        if (!bottomUiManager.IsSkillLock)
        {
            // ��ų�� ���� �Ҹ��� ������ ���.
            if (!bm.costLink.CheckCanUseSkill(skill, out string cautionMessage))
            {
                var offset = new Vector2(0f, height * 1 / 8);
                var alittleOpenPos = CoverOrigianlPos + offset;

                bm.uiLink.PrintCaution(cautionMessage, 0.75f, 0.3f, null);

                // ��¦
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

    public void Cancle() // ��ư
    {
        if (bm.isTutorial && bm.tutorial.lockSkillButtonClick)
            return;
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

        if (!ownButton.interactable)
            return;

        if (buttonType != SkillButtonType.None)
            return;

        if (bm.isTutorial && bm.tutorial.lockSkillButtonDrag)
            return;

        bottomUiManager.info.Init(skill);
        if (!bottomUiManager.IsSkillLock)
        {
            // ��ų�� ���� �Ҹ��� ���� ���.
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
        //Debug.Log($"������ ��� Ÿ�� : {bm.dragLink.lastDrapTile.index}");
        //Debug.Log($"������ ��� Ÿ���� �����? : {bm.dragLink.lastDrapTile.WhichPartOfTile(bm.dragLink.lastDragWorldPos)}");

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
