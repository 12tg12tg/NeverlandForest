using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomSkillButtonUI : MonoBehaviour
{
    private BottomUIManager bottomUiManager;
    public Button ownButton;

    public DataPlayerSkill skill;                       // ����ִ� ��ų����

    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costTxt;   // ��ų ������ �������� ����

    [SerializeField] private Button cover;
    [SerializeField] private Button below;              // Ȱ��ȭ/��Ȱ��ȭ�� ��ư

    [SerializeField] private RectTransform coverRt;
    private CanvasScaler cs;
    private float height;
    private Vector2 openOffset;                         // Open/Close�� ũ�� ���
    private bool isCalculateOffset;

    // Property
    private Vector2 CoverOrigianlPos { get; set; }
    private Vector2 CoverOpenPos { get; set; }

    public void Init(DataPlayerSkill skill)
    {
        bottomUiManager ??= BottomUIManager.Instance;
        cover.interactable = true;
        below.interactable = false;

        this.skill = skill;
        skillImg.sprite = skill.SkillTableElem.IconSprite;
        costTxt.text = skill.SkillTableElem.cost.ToString();
    }

    public void CalculateOffset()
    {
        cs = GetComponentInParent<CanvasScaler>();
        var size = Utility.RelativeRectSize(cs, coverRt);
        height = size.y;
        openOffset = new Vector2(0f, height * 3 / 8);
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

    public void IntoSkillStage() // ��ư
    {
        if(!isCalculateOffset)
        {
            isCalculateOffset = true;
            CalculateOffset();
            CoverOrigianlPos = coverRt.anchoredPosition;
            CoverOpenPos = CoverOrigianlPos + openOffset;
        }

        bottomUiManager.info.Init(skill);

        if(!bottomUiManager.IsSkillLock)
        {
            cover.interactable = false;
            bottomUiManager.IntoSkillState(this);
            StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, CoverOpenPos, 0.3f,
                () => { below.interactable = true; }));
        }
    }

    public void Cancle() // ��ư
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
}
