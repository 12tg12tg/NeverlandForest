using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    private SkillSelectUI groupUI;
    private BattleManager manager;
    public Image mainIcon;
    private DataPlayerSkill skill;
    private bool isButtonDown;

    public void Init(SkillSelectUI group, BattleManager manager, DataPlayerSkill skill)
    {
        this.groupUI = group;
        this.manager = manager;
        this.skill = skill;
        mainIcon.sprite = skill.SkillTableElem.IconSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(MultiTouch.Instance.PrimaryPos != Vector2.zero)
            manager.OpenSkillInfo(skill, MultiTouch.Instance.PrimaryPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        groupUI.DisableGroupDuringDrag(this);
        manager.CreateTempSkillUiForDrag(skill);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        groupUI.EnableGroup();
        manager.EndTempSkillUiForDrag();
    }


}
