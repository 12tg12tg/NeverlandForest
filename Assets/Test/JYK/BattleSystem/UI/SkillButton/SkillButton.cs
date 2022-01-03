using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private SkillSelectUI groupUI;
    private BattleManager manager;
    public Image mainIcon;
    private DataPlayerSkill skill;
    private DataCunsumable item;
    private bool isButtonDown;
    private bool isDragOnTile;
    private ActionType curState;
    public void Init(SkillSelectUI group, BattleManager manager, DataPlayerSkill skill)
    {
        this.groupUI ??= group;
        this.manager ??= manager;
        this.skill = skill;
        curState = ActionType.Skill;
        mainIcon.sprite = skill.SkillTableElem.IconSprite;
    }
    public void Init(SkillSelectUI group, BattleManager manager, DataCunsumable item)
    {
        this.groupUI ??= group;
        this.manager ??= manager;
        this.item = item;
        curState = ActionType.Item;
        mainIcon.sprite = item.ItemTableElem.IconSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (MultiTouch.Instance.PrimaryPos != Vector2.zero)
        {
            if (curState == ActionType.Skill)
                manager.OpenSkillInfo(skill, MultiTouch.Instance.PrimaryPos);
            else
                manager.OpenItemInfo(item, MultiTouch.Instance.PrimaryPos);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        groupUI.DisableGroupDuringDrag(this);

        if (curState == ActionType.Skill)
        {
            manager.DisplayMonsterTile();
            manager.CreateTempSkillUiForDrag(skill);
        }
        else
        {
            manager.DisplayPlayerTile();
            manager.CreateTempItemUiForDrag(item);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (curState == ActionType.Skill)
        {
            Debug.Log("스킬 드래그 중");
        }
        else
        {
            Debug.Log("아이템 드래그 중");
        }

        var ray = Camera.main.ScreenPointToRay(MultiTouch.Instance.TouchPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask("Tile")))
        {
            var tile = hit.transform.GetComponent<Tiles>();
            TileMaker.Instance.SetAllTileMiddleState();
            tile.HighlightSkillRange();
            isDragOnTile = true;
        }
        else
        {
            isDragOnTile = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        manager.DisplayTileClear();

        groupUI.EnableGroup();
        manager.EndTempUiForDrag();

        if (isDragOnTile)
        {
            if (curState == ActionType.Skill)
            {
                manager.UpdateComand(groupUI.type, TileMaker.Instance.LastDropPos, skill);
            }
            else
            {
                manager.UpdateComand(groupUI.type, TileMaker.Instance.LastDropPos, item);
            }
        }
    }


}
