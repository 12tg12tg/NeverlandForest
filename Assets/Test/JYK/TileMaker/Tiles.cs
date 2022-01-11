using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public MeshRenderer ren;
    public Vector2 index;
    public bool isObstacle;
    public List<UnitBase> units = new List<UnitBase>();

    //Instance
    private TileMaker tileMaker;
    public MeshRenderer center;
    public MeshRenderer edge;

    //Property
    public Vector3 WorldPos { get => transform.position; }
    public bool HaveUnit { get => units.Count > 0; }
    public bool CanStand { get => units.Count < 2; }

    //Vars
    private bool isHighlightAttack;
    private bool isHighlightConsume;
    private Color confirmColor;
    private Color boyColor;
    private Color girlColor;
    public bool affectedByBoy;
    public bool affectedByGirl;

    private void Start()
    {
        tileMaker = TileMaker.Instance;
        ColorUtility.TryParseHtmlString("#42C0FF", out boyColor);
        ColorUtility.TryParseHtmlString("#FFCC42", out girlColor);
    }

    //Move
    public int MaxMovableRange()
    {
        //���Ͱ� �̵������� �ִ� ������ ��ȯ��. (�ִ� 3 ����)

        int distanceToPlayer = (int)index.y - 0;
        int max = distanceToPlayer - 1;
        max = max > 3 ? 3 : max;
        return max; //�÷��̾� Ÿ�Ͽ��� �ö� �� ����.
    }

    [System.Obsolete("��а� ���� ������")]
    public bool TryGetFowardTile(out Tiles tile, int distance)
    {
        var dest = index;
        dest.y -= distance;
        var destTile = TileMaker.Instance.GetTile(dest);
        
        if(destTile == null || dest.y <= 0)
        {
            Debug.LogWarning($"Cant move foward beacuse there is no {dest} Index Tile!");
            tile = null;
            return false;
        }
        else
        {
            tile = destTile;
            return true;
        }
    }

    //Highlight
    public void Clear()
    {
        //���� �ʱ�ȭ. � ���̶���Ʈ�� ����.
        affectedByBoy = false;
        affectedByGirl = false;
        isHighlightAttack = false;
        isHighlightConsume = false;
        ren.material.color = tileMaker.noneColor;
        center.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
    }

    public void HighlightSkillRange()
    {
        center.material.color = tileMaker.blueColor;
    }

    public void HighlightCanAttackSign()
    {
        isHighlightAttack = true;
        edge.material.color = tileMaker.targetColor;
    }

    public void HighlightCanConsumeSign()
    {
        isHighlightConsume = true;
        edge.material.color = tileMaker.consumeColor;
    }

    public void ResetHighlightExceptConfirm()
    {       
        isHighlightAttack = false;
        isHighlightConsume = false;
        center.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
        if (affectedByBoy || affectedByGirl)
            center.material.color = confirmColor;
    }

    private void ReCalculateAffectedColor()
    {
        Color color = Color.clear;
        if (affectedByBoy)
            color += boyColor;
        if (affectedByGirl)
            color += girlColor;
        this.confirmColor = color;
    }

    public void ConfirmAsTarget(PlayerType who)
    {
        if (who == PlayerType.Boy)
        {
            affectedByBoy = true;
        }
        else
        {
            affectedByGirl = true;
        }

        ReCalculateAffectedColor();
    }
    
    public void CancleConfirmTarget(PlayerType type)
    {
        if(type == PlayerType.Boy)
        {
            affectedByBoy = false;
        }
        else
        {
            affectedByGirl = false;
        }

        ReCalculateAffectedColor();
        ResetHighlightExceptConfirm();
    }

    public void SetMiddleState()
    {
        //�巡�� �� �϶��� ȣ��.
        //���̶���Ʈ�� �ʱ�ȭ���� ������, ���콺 �巡���߿� ǥ�õǴ� ��� ����Ʈ�� ���ش�.
        center.material.color = tileMaker.noneColor;
        if (isHighlightAttack)
            HighlightCanAttackSign();
        if (isHighlightConsume)
            HighlightCanConsumeSign();
        if (affectedByBoy || affectedByGirl)
            ResetHighlightExceptConfirm();
    }

    public void RemoveUnit(UnitBase unit)
    {
        units.Remove(unit);
    }

    //Drag Drop
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {index} Tile! ");
        TileMaker.Instance.LastDropPos = index;
    }

    //Click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BattleManager.Instance.IsWaitingTileSelect)
            return;

        var manager = BattleManager.Instance;
        var button = manager.CurClickedButton;

        var skill = button.Skill;
        var item = button.Item;

        var actionType = button.CurState;

        if (true /*���� Ÿ�� ��ų���� Ȯ��*/)
        {
            //���� Ÿ���̶�� ��ȿ�� �˻�
            if (!BattleManager.Instance.IsVaildTargetTile(this))
            {
                manager.PrintCaution("���� Ÿ�� ��ų�� �ݵ�� ����� �����ؾ� �մϴ�.", 0.7f, 0.5f, null);
                return;
            }
            else
            {
                tileMaker.AffectedTileCancle(button.groupUI.type);
                ConfirmAsTarget(button.groupUI.type);
            }
        }
        else
        {
            //���� ��ų�̶��
            //  0) affectedPlayer�� ���� ���� Ȯ�� ���� �� ��� ���ֱ�.
            //  1) ���� ��� �� Ÿ�� ����Ʈ�� ����.
            //  2) tile.Confirm(color) �Լ� ȣ���ϱ�.
            //  3) affectedPlayer �����ϱ�.
        }

        if (actionType == ActionType.Skill)
        {
            manager.UpdateComand(button.groupUI.type, tileMaker.LastDropPos, skill);
        }
        else
        {
            manager.UpdateComand(button.groupUI.type, tileMaker.LastDropPos, item);
        }

        tileMaker.SetAllTileSoftClear();
        button.groupUI.EnableGroup();
        button.groupUI.Close();
        manager.EndTileClick();
    }


}
