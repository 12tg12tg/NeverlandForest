using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomInfoUI : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI info_name;
    public TextMeshProUGUI info_description;


    public void Init()
    {
        img.sprite = null;
        info_name.text = string.Empty;
        info_description.text = string.Empty;
    }

    public void Init(DataPlayerSkill skill)
    {
        img.sprite = skill.SkillTableElem.IconSprite;
        info_name.text = skill.SkillTableElem.name;
        info_description.text = skill.SkillTableElem.description;
    }

    public void Init(DataItem item)
    {
        if (item == null)
            return;
        switch (item.dataType)
        {
            case DataType.Consume:
                break;
            case DataType.AllItem:
                var allItem = item as DataAllItem;
                img.sprite = allItem.ItemTableElem.IconSprite;
                info_name.text = allItem.ItemTableElem.name;
                info_description.text = ".....";
                break;
            case DataType.Material:
                break;
        }
    }
}
