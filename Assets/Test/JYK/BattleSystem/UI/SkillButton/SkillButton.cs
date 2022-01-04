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
    private DataConsumable item;
    private bool isButtonDown;
    private bool isDragOnTile;
    private ActionType curState;
    public ActionType CurState { get => curState; }
    public DataPlayerSkill Skill { get => skill; }
    public DataConsumable Item { get => item; }
    public void Init(SkillSelectUI group, BattleManager manager, DataPlayerSkill skill)
    {
        this.groupUI ??= group;
        this.manager ??= manager;
        this.skill = skill;
        curState = ActionType.Skill;
        mainIcon.sprite = skill.SkillTableElem.IconSprite;
    }
    public void Init(SkillSelectUI group, BattleManager manager, DataConsumable item)
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
            Debug.Log("��ų �巡�� ��");
        }
        else
        {
            Debug.Log("������ �巡�� ��");
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
            if (true /*���� Ÿ�� ��ų���� Ȯ��*/)
            {
                //���� Ÿ���̶�� ��ȿ�� �˻�
                if (!manager.IsVaildTargetTile(tileMaker.LastDropTile))
                {
                    manager.PrintCaution("���� Ÿ�� ��ų�� �ݵ�� ����� �����ؾ� �մϴ�.", 0.7f, 0.5f, null);
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
                //���� ��ų�̶��
                //  1) ���� ��� �� Ÿ�� ����Ʈ�� ����.
                //  2) tile.Confirm(color) �Լ� ȣ���ϱ�.
                //  3) affectedPlayer �����ϱ�.
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
