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
        //완전 초기화. 어떤 하이라이트도 없음.
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
        //드래그 중 일때만 호출.
        //하이라이트를 초기화하진 않으나, 마우스 드래그중에 표시되는 블루 프린트를 없앤다.
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

        if (true /*단일 타겟 스킬인지 확인*/)
        {
            //단일 타겟이라면 유효성 검사
            if (!BattleManager.Instance.IsVaildTargetTile(this))
            {
                manager.PrintCaution("단일 타겟 스킬은 반드시 대상을 지정해야 합니다.", 0.7f, 0.5f, null);
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
            //범위 스킬이라면
            //  1) 범위 계산 후 타일 리스트를 만들어서.
            //  2) tile.Confirm(color) 함수 호출하기.
            //  3) affectedPlayer 설정하기.
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
