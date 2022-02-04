using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomSkillButtonUI : MonoBehaviour
{
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

    // Property
    private Vector2 OpenOffset
    {
        get
        {
            if(openOffset == Vector2.zero)
            {
                CalculateOffset();
            }
            return openOffset;
        } 
    }

    public void Init(DataPlayerSkill skill)
    {
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
        BottomUIManager.Instance.info.Init(skill);

        if(GameManager.Manager.State == GameState.Battle && BattleManager.Instance.FSM.curState == BattleState.Player)
        {
            cover.interactable = false;
            BottomUIManager.Instance.IntoSkillState(this);
            BattleManager.Instance.directLink.StartSkillSelect();
            StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, coverRt.anchoredPosition + OpenOffset, 0.3f,
                () => { below.interactable = true; }));
        }
    }

    public void Cancle() // ��ư
    {
        BattleManager.Instance.directLink.EndSkillSelect();
        below.interactable = false;
        BottomUIManager.Instance.ExitSkillState();
        StartCoroutine(Utility.CoTranslate(coverRt, coverRt.anchoredPosition, coverRt.anchoredPosition - OpenOffset, 0.3f,
            () => { cover.interactable = true; }));
    }
}
