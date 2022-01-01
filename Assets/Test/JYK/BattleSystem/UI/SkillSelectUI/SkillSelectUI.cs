using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillSelectUI : MonoBehaviour
{
    //prefab
    public GameObject skillPrefab;

    //Component
    public CanvasGroup group;

    //Instance
    private BattleManager manager;
    public Transform skillContent;
    private CanvasGroup exceptGoDuringDrag;

    //Own List
    private List<DataPlayerSkill> skillList;
    private List<SkillButton> skillGoList = new List<SkillButton>();

    //Vars
    private int defaultCount = 5;

    public void Init(BattleManager manager, List<DataPlayerSkill> skills)
    {
        skillList = skills;
        for (int i = 0; i < defaultCount; i++)
        {
            var go = Instantiate(skillPrefab, skillContent);
            go.name = $"Skill {i}";
            go.SetActive(false);
            skillGoList.Add(go.GetComponent<SkillButton>());
        }

        for (int i = 0; i < skillList.Count; i++)
        {
            skillGoList[i].gameObject.SetActive(true);
            skillGoList[i].Init(this, manager, skillList[i]);
        }
    }
    public void DisableGroupDuringDrag(SkillButton except)
    {
        exceptGoDuringDrag = except.GetComponent<CanvasGroup>();
        exceptGoDuringDrag.ignoreParentGroups = true;

        group.interactable = false;
        group.alpha = 0.5f;
    }

    public void EnableGroup()
    {
        if(exceptGoDuringDrag != null)
            exceptGoDuringDrag.ignoreParentGroups = false;
        group.interactable = true;
        group.alpha = 1f;
    }
}
