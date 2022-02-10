using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class BottomInfoUI : MonoBehaviour
{
    private enum DescriptionType { Food, Potion, Installation, Burn }

    public Image img;
    public TextMeshProUGUI info_name;
    public TextMeshProUGUI info_description;

    private List<int> desc_colorChange_Index = new List<int>();
    public void Init()
    {
        img.sprite = null;
        img.color = Color.clear;
        info_name.text = "정보 없음";
        info_description.text = "정보 없음";
    }

    public void Init(DataPlayerSkill skill)
    {
        img.sprite = skill.SkillTableElem.IconSprite;
        img.color = Color.white;

        info_name.text = skill.SkillTableElem.name;
        info_description.text = skill.SkillTableElem.description;
    }

    public void Init(DataAllItem item)
    {
        if (item == null)
            return;

        var allItem = item;
        img.sprite = allItem.ItemTableElem.IconSprite;
        img.color = Color.white;

        var elem = item.ItemTableElem;
        if (elem.isBurn)
        {
            info_description.text = DescriptionColoring(DescriptionType.Burn, allItem.ItemTableElem.desc);
        }
        else if (elem.type == "INSTALLATION" || elem.id == "ITEM_22")
        {
            info_description.text = DescriptionColoring(DescriptionType.Installation, allItem.ItemTableElem.desc);
        }
        else if (elem.type == "FOOD")
        {
            info_description.text = DescriptionColoring(DescriptionType.Food, allItem.ItemTableElem.desc);
        }
        else
        {
            info_description.text = allItem.ItemTableElem.desc;
        }
        info_name.text = allItem.ItemTableElem.name;
    }

    private string DescriptionColoring(DescriptionType type, string desc)
    {
        bool isPreDigit = false;
        string newDesc = string.Empty;

        string startToken = string.Empty;
        string endToken = "</color></b>";
        switch (type)
        {
            case DescriptionType.Food:
                startToken = "<b><color=green>";
                break;
            case DescriptionType.Potion:
                startToken = "<b><color=red>";
                break;
            case DescriptionType.Installation:
                startToken = "<b><color=red>";
                break;
            case DescriptionType.Burn:
                startToken = "<b><color=orange>";
                break;
        }

        for (int i = 0; i < desc.Length; i++)
        {
            if (char.IsDigit(desc[i]))
            {
                if (isPreDigit)
                {
                    newDesc += desc[i];
                }
                else
                {
                    newDesc += $"{startToken}{desc[i]}";
                }
                isPreDigit = true;
            }
            else
            {
                if (isPreDigit)
                {
                    newDesc += $"{endToken}{desc[i]}";
                }
                else
                {
                    newDesc += desc[i];
                }
                isPreDigit = false;
            }
        }

        return newDesc;
    }
}
