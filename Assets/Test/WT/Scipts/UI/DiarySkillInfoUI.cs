using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DiarySkillInfoUI : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI info_name;
    public TextMeshProUGUI info_description;

   [SerializeField] private List<DiarySkillButtonUi> diarySkillButtonList = new List<DiarySkillButtonUi>();

    public void Init()
    {
        img.sprite = diarySkillButtonList[2].SkillImg.sprite;
        info_name.text = diarySkillButtonList[2].skill.SkillTableElem.name;
        info_description.text = diarySkillButtonList[2].skill.SkillTableElem.description;
    }
    public void Init(DataPlayerSkill skill)
    {
        img.sprite = skill.SkillTableElem.IconSprite;
        info_name.text = skill.SkillTableElem.name;
        info_description.text = skill.SkillTableElem.description;
    }
}
