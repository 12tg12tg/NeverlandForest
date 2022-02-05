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
    public void Init()
    {
        img.sprite = null;
        info_name.text = "���� ����";
        info_description.text = "���� ����";
    }
    public void Init(DataPlayerSkill skill)
    {
        img.sprite = skill.SkillTableElem.IconSprite;
        info_name.text = skill.SkillTableElem.name;
        info_description.text = skill.SkillTableElem.description;
    }
}
