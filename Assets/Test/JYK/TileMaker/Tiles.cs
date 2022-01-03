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

    //Property
    public Vector3 WolrdPos { get => transform.position; }
    public bool HaveUnit { get => units.Count > 0; }
    public bool CanStand { get => units.Count < 2; }

    //Vars
    private Color tempOriginalColor;
    private bool isHighlightAttack;
    private bool isHighlightConsume;
    private bool isConfirm;
    private Color confirmColor;
    public PlayerType affectedPlayer;

    private void Start()
    {
        tempOriginalColor = ren.material.color;
    }

    //Move
    public bool TryGetFowardTile(out Tiles tile, int distance)
    {
        var dest = index;
        dest.y -= distance;
        var destTile = TileMaker.Instance.GetTile(dest);
        
        if(destTile == null)
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
        isConfirm = false;
        isHighlightAttack = false;
        isHighlightConsume = false;
        ren.material.color = tempOriginalColor;
        affectedPlayer = PlayerType.None;
    }

    public void HighlightSkillRange()
    {
        ren.material.color = Color.blue;
    }

    public void HighlightCanAttackSign()
    {
        isHighlightAttack = true;
        ren.material.color = Color.red;
    }

    public void HighlightCanConsumeSign()
    {
        isHighlightConsume = true;
        ren.material.color = Color.green;
    }

    public void ResetHighlightExceptConfirm()
    {
        
        isHighlightAttack = false;
        isHighlightConsume = false;
        if (isConfirm)
            ren.material.color = confirmColor;
        else
            ren.material.color = tempOriginalColor;
    }

    public void ConfirmTarget(Color confirmColor)
    {
        isConfirm = true;
        this.confirmColor = confirmColor;
    }

    public void SetMiddleState()
    {
        //�巡�� �� �϶��� ȣ��.
        //���̶���Ʈ�� �ʱ�ȭ���� ������, ���콺 �巡���߿� ǥ�õǴ� ��� ����Ʈ�� ���ش�.
        if (isHighlightAttack)
            HighlightCanAttackSign();
        else if (isHighlightConsume)
            HighlightCanConsumeSign();
        else
            ResetHighlightExceptConfirm();
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
        var tileMaker = TileMaker.Instance;
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
                Color color;
                if (button.groupUI.type == PlayerType.Boy)
                {
                    ColorUtility.TryParseHtmlString("#42C0FF", out color);
                    affectedPlayer = PlayerType.Boy;
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#FFCC42", out color);
                    affectedPlayer = PlayerType.Girl;
                }

                ConfirmTarget(color);
            }
        }
        else
        {
            //���� ��ų�̶��
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
