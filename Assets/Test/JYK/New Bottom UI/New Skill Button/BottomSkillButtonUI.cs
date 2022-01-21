using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomSkillButtonUI : MonoBehaviour
{
    public DataPlayerSkill skill;                       // ����ִ� ��ų����
    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costTxt;   //��ų ������ �������� ����

    [SerializeField] private Button cover;
    [SerializeField] private Button below;              // Ȱ��ȭ/��Ȱ��ȭ�� ��ư

    [SerializeField] private RectTransform coverRt;
    [SerializeField] private CanvasScaler cs;
    private float height;
    private Vector3 openOffset;                         // Open/Close�� ũ�� ���

    [SerializeField] private RectTransform costRt;      // ���� �� �ڽ�Ʈ ���� ũ����

    private Vector3 OpenOffset
    {
        get
        {
            if(openOffset == Vector3.zero)
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
        var size = Utility.RelativeRectSize(cs, coverRt);
        height = size.y;
        openOffset = new Vector3(0f, height * 3 / 8, 0f);
        Debug.Log($"{height}, {coverRt.sizeDelta.y}");
    }

    public void IntoSkillStage() // ��ư
    {
        BottomUIManager.Instance.info.Init(skill);

        if(BattleManager.Instance.IsMiddleOfBattle)
        {
            cover.interactable = false;
            StartCoroutine(Utility.CoTranslate(coverRt, coverRt.position, coverRt.position + OpenOffset, 0.3f,
                () => { below.interactable = true; }));
        }
    }

    public void Cancle() // ��ư
    {
        below.interactable = false;
        StartCoroutine(Utility.CoTranslate(coverRt, coverRt.position, coverRt.position - OpenOffset, 0.3f,
            () => { cover.interactable = true; }));
    }
}
