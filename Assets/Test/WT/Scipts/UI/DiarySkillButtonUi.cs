using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiarySkillButtonUi : MonoBehaviour
{
    public DataPlayerSkill skill;
    [SerializeField] private Image skillImg;

    public void Init(DataPlayerSkill skill)
    {
        this.skill = skill;
        skillImg.sprite = skill.SkillTableElem.IconSprite;
    }
    public void IntoSkillStage() // ¹öÆ°
    {
        DiarySkill.Instance.info.Init(skill);
    }
}
