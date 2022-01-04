using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public SkillSelectUI groupUI;
    private BattleManager manager;
    public Image mainIcon;
    private DataPlayerSkill skill;
    private DataCunsumable item;
    private bool isButtonDown;
    private bool isDragOnTile;
    private ActionType curState;
    public ActionType CurState { get => curState; }
    public DataPlayerSkill Skill { get => skill; }
    public DataCunsumable Item { get => item; }
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
                manager.OpenSkillInfo(this, skill, MultiTouch.Instance.PrimaryPos);
            else
                manager.OpenItemInfo(this, item, MultiTouch.Instance.PrimaryPos);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        groupUI.DisableGroupDuringSelect(this);

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
            TileMaker.Instance.SetAllTileMiddleState();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        groupUI.EnableGroup();
        manager.EndTempUiForDrag();

        if (isDragOnTile)
        {
            var tileMaker = TileMaker.Instance;
            if (true /*단일 타겟 스킬인지 확인*/)
            {
                //단일 타겟이라면 유효성 검사
                if (!manager.IsVaildTargetTile(tileMaker.LastDropTile))
                {
                    manager.PrintCaution("단일 타겟 스킬은 반드시 대상을 지정해야 합니다.", 0.7f, 0.5f, null);
                    tileMaker.SetAllTileSoftClear();
                    return;
                }
                else
                {
                    Color color;
                    if (groupUI.type == PlayerType.Boy)
                    {
                        ColorUtility.TryParseHtmlString("#42C0FF", out color);
                        tileMaker.LastDropTile.affectedPlayer = PlayerType.Boy;
                    }
                    else
                    {
                        ColorUtility.TryParseHtmlString("#FFCC42", out color);
                        tileMaker.LastDropTile.affectedPlayer = PlayerType.Girl;
                    }

                    tileMaker.LastDropTile.ConfirmTarget(color);
                }
            }
            else
            {
                //범위 스킬이라면
                //  1) 범위 계산 후 타일 리스트를 만들어서.
                //  2) tile.Confirm(color) 함수 호출하기.
                //  3) affectedPlayer 설정하기.
            }

            if (curState == ActionType.Skill)
            {
                manager.UpdateComand(groupUI.type, tileMaker.LastDropPos, skill);
            }
            else
            {
                manager.UpdateComand(groupUI.type, tileMaker.LastDropPos, item);
            }

            tileMaker.SetAllTileSoftClear();
            groupUI.Close();
        }
    }


}
