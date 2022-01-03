using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class BattleSkillInfo : MonoBehaviour
{
    //Instance
    private DataPlayerSkill curSkill;

    //GmaeObject
    public Image skillIcon;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI description;
    private StringBuilder sb = new StringBuilder();

    public void Init(DataPlayerSkill skill)
    {
        curSkill = skill;
        var elem = curSkill.SkillTableElem;
        skillIcon.sprite = elem.IconSprite;
        skillName.text = elem.name;

        sb.Clear();
        var count = elem.count;
        if (count == -1)
            sb.Append($"횟수 : 제한없음.\n");
        else
            sb.Append($"횟수 : {elem.count}\n");
        sb.Append($"{elem.description}");
        description.text = sb.ToString();
    }

    public void CloseInfo()
    {
        gameObject.SetActive(false);
    }

    public void SelectSkill()
    {
        Debug.Log($"Select {curSkill.SkillTableElem.name}");
        //DataPlayerSkill 전달

        // 추가
        BattleManager.Instance.curSelectSkill = curSkill;
    }    
}
