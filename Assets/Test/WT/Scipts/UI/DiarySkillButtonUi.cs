using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiarySkillButtonUi : MonoBehaviour
{

    public DataPlayerSkill skill;                       // 담고있는 스킬정보
    [SerializeField] private Image skillImg;
    [SerializeField] private TextMeshProUGUI costItemCount;

    public Image SkillImg
    {
        get => skillImg;
    }

    public void Init(DataPlayerSkill skill) // 스킬을 지닌 버튼 초기화
    {
        this.skill = skill;
        skillImg.sprite = skill.SkillTableElem.IconSprite;
    }
    public void IntoSkillStage() // 버튼
    {
        DiarySkill.Instance.info.Init(skill);
    }
}
