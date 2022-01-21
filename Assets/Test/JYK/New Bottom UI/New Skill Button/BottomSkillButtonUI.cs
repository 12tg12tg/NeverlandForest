using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomSkillButtonUI : MonoBehaviour
{
    public DataPlayerSkill skill;                       // 담고있는 스킬정보
    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costTxt;   //스킬 정보를 바탕으로 구성

    [SerializeField] private Button cover;
    [SerializeField] private Button below;              // 활성화/비활성화할 버튼

    [SerializeField] private RectTransform coverRt;
    [SerializeField] private CanvasScaler cs;
    private float height;
    private Vector3 openOffset;                         // Open/Close용 크기 계산

    [SerializeField] private RectTransform costRt;      // 왼쪽 위 코스트 정보 크기계산

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

    public void IntoSkillStage() // 버튼
    {
        BottomUIManager.Instance.info.Init(skill);

        if(BattleManager.Instance.IsMiddleOfBattle)
        {
            cover.interactable = false;
            StartCoroutine(Utility.CoTranslate(coverRt, coverRt.position, coverRt.position + OpenOffset, 0.3f,
                () => { below.interactable = true; }));
        }
    }

    public void Cancle() // 버튼
    {
        below.interactable = false;
        StartCoroutine(Utility.CoTranslate(coverRt, coverRt.position, coverRt.position - OpenOffset, 0.3f,
            () => { cover.interactable = true; }));
    }
}
