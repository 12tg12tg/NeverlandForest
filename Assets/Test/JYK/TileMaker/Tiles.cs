using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public enum HalfTile { Front, Behind }
public class AffectedInfo
{
    public bool isAffected;
    public PlayerType player; // 누구의 스킬이 영향을 주는가 -> 색 결정
    public SkillRangeType skillRange; // 어떤 범위의 스킬인가 -> Half 인지 Full 인지 결정
    public HalfTile half; // 앞/뒤 -> 어느 타일을 칠해야하는가
}


public class Tiles : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public MeshRenderer ren;
    public Vector2 index;
    public bool isObstacle;
    [SerializeField] private List<UnitBase> units = new List<UnitBase>();
    public Obstacle obstacle = null;

    //Instance
    private TileMaker tileMaker;
    private BattleManager bm;
    [Space(15)] // 장애물 이랑 구별 하려고 추가
    public MeshRenderer center;
    public MeshRenderer edge;
    public MeshRenderer frontEdge;
    public MeshRenderer front;
    public MeshRenderer behindEdge;
    public MeshRenderer behind;

    //Property
    public Vector3 CenterPos { get => transform.position; }
    public bool HaveUnit { get => Units_UnitCount() > 0; }
    public bool CanStand { get => Units_UnitCount() < 2; }
    public Vector3 FrontPos { get; private set; }
    public Vector3 BehindPos { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public MonsterUnit FrontMonster { get => units[0] as MonsterUnit; }
    public MonsterUnit BehindMonster { get => units[1] as MonsterUnit; }
    public IEnumerable<UnitBase> Units { get => from n in units where n != null select n; }

    //Vars
    public bool isHighlight;
    //private bool isHighlightConsume;
    private Color confirmColor;
    private Color boyColor;
    private Color girlColor;
    public AffectedInfo affectedByBoy = new AffectedInfo();
    public AffectedInfo affectedByGirl = new AffectedInfo();

    private void Awake()
    {
        while (units.Count < 2)
        {
            units.Add(null);
        }
    }

    private void Start()
    {
        bm = BattleManager.Instance;
        tileMaker = TileMaker.Instance;
        ColorUtility.TryParseHtmlString("#42C0FF", out boyColor);
        ColorUtility.TryParseHtmlString("#FFCC42", out girlColor);
        var bound = GetComponent<MeshRenderer>().bounds;
        Width = bound.size.x;
        Height = bound.size.z;
        var leftTop = CenterPos + new Vector3(-Width/2, 0f, Height/2);
        var rightTop = CenterPos + new Vector3(Width/2, 0f, Height/2);
        var leftBottom = CenterPos + new Vector3(-Width / 2, 0f, -Height/2);
        var rightBottom = CenterPos + new Vector3(Width / 2, 0f, -Height/2);
        FrontPos = (leftTop + rightTop + leftBottom) / 3;
        BehindPos = (leftBottom + rightTop + rightBottom) / 3;

        affectedByBoy.player = PlayerType.Boy;
        affectedByGirl.player = PlayerType.Girl;
    }

    //Move
    public int MaxMovableRange()
    {
        //몬스터가 이동가능한 최대 범위를 반환함. (최대 3 제한)

        int distanceToPlayer = (int)index.y - 0;
        int max = distanceToPlayer - 1;
        max = max > 3 ? 3 : max;
        return max; //플레이어 타일에는 올라설 수 없다.
    }

    [System.Obsolete("당분간 쓸일 없을듯")]
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

    //Half Tile
    public HalfTile WhichPartOfTile(Vector3 touchPos)
    {
        float x = touchPos.x;
        float touchY = touchPos.z;

        float slopeY = (x - CenterPos.x) * Height / Width + CenterPos.z;

        return (touchY > slopeY) ? HalfTile.Front : HalfTile.Behind;
    }

    //Highlight
    public void Clear()
    {
        //완전 초기화. 어떤 하이라이트도 없음.
        affectedByBoy.isAffected = false;
        affectedByGirl.isAffected = false;
        isHighlight = false;
        //isHighlightConsume = false;
        ren.material.color = tileMaker.noneColor;
        center.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
    }

    public void HighlightSkillRange()
    {
        // 드래그 지점 타일에서의 범위 스킬로 표시되는 경우. 수동적인 표시.
        center.material.color = tileMaker.dragColor;
    }

    public void HighlightSkillRange(SkillRangeType range, Vector3 dragWorldPos) // 드래그중인 스킬 범위 내부표시
    {
        // For Drag
        switch (range)
        {
            case SkillRangeType.One:
                if (Units_UnitCount() == 1)
                    center.material.color = tileMaker.dragColor;
                else
                {
                    var whichPart = WhichPartOfTile(dragWorldPos);
                    if (whichPart == HalfTile.Front)
                        front.material.color = tileMaker.dragColor;
                    else
                        behind.material.color = tileMaker.dragColor;
                }
                break;
            case SkillRangeType.Tile:
                center.material.color = tileMaker.dragColor;
                break;

            case SkillRangeType.Line:
            case SkillRangeType.Lantern:
                var rangedTiles = tileMaker.GetSkillRangedTiles(index, range);
                foreach (var tile in rangedTiles)
                    tile.HighlightSkillRange();
                break;
        }
    }

    public void HighlightCanAttackSign(SkillRangeType range) // 빨간 테두리 표시
    {
        // Both drag and click
        isHighlight = true;

        if (range == SkillRangeType.One)
        {
            if (Units_UnitCount() == 1)
                edge.material.color = tileMaker.targetColor;
            else
            {
                frontEdge.material.color = tileMaker.targetColor;
                behindEdge.material.color = tileMaker.targetColor;
            }
        }
        else
        {
            edge.material.color = tileMaker.targetColor;
        }
    }

    //public void HighlightCanConsumeSign()
    //{
    //    // Both drag and click
    //    isHighlightConsume = true;
    //    edge.material.color = tileMaker.consumeColor;
    //}

    public void ResetHighlightExceptConfirm() // 스킬 사용 이후 호출.
    {
        isHighlight = false;
        //isHighlightConsume = false;

        center.material.color = tileMaker.noneColor;
        front.material.color = tileMaker.noneColor;
        behind.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
        frontEdge.material.color = tileMaker.noneColor;
        behindEdge.material.color = tileMaker.noneColor;

        if (!affectedByBoy.isAffected && !affectedByGirl.isAffected)
            return; // 선택되지 않은 타일이라면 종료.

        AffectedInfo curInfo = null;
        Color selectedColor = Color.clear;
        if (affectedByBoy.isAffected)
        {
            curInfo = affectedByBoy;
            selectedColor = boyColor;
        }
        if (affectedByGirl.isAffected)
        {
            curInfo = affectedByGirl;
            selectedColor = girlColor;
        }

        // 둘 중 하나라도 선택되어있다면, 새롭게 색칠.
        bool isOneTargetSkill = curInfo.skillRange == SkillRangeType.One;
        if (isOneTargetSkill && Units_UnitCount() == 2)
        {
            if (curInfo.half == HalfTile.Front)
            {
                front.material.color = selectedColor;
            }
            else
            {
                behind.material.color = selectedColor;
            }
        }
        else
        {
            center.material.color = selectedColor;
        }
    }

    public void ConfirmAsTarget(PlayerType player, Vector3 touchPos, SkillRangeType skillType)
    {
        // 넘어온 정보를 바탕으로 AffectedInfo 재구성.
        AffectedInfo info;
        if (player == PlayerType.Boy)
            info = affectedByBoy;
        else
            info = affectedByGirl;       

        info.isAffected = true;
        info.half = WhichPartOfTile(touchPos);
        info.skillRange = skillType;
    }
    
    public void CancleConfirmTarget(PlayerType type)
    {
        if(type == PlayerType.Boy)
        {
            affectedByBoy.isAffected = false;
        }
        else
        {
            affectedByGirl.isAffected = false;
        }

        ResetHighlightExceptConfirm();
    }

    public void SetMiddleState(SkillRangeType skillType)
    {
        //드래그 중 일때만 호출.
        //하이라이트를 초기화하진 않으나, 마우스 드래그중에 표시되는 블루 프린트를 없앤다.
        center.material.color = tileMaker.noneColor;
        front.material.color = tileMaker.noneColor;
        behind.material.color = tileMaker.noneColor;

        if (isHighlight)
            HighlightCanAttackSign(skillType);
        //if (isHighlightConsume)
        //    HighlightCanConsumeSign();
        //if (affectedByBoy.isAffected || affectedByGirl.isAffected)
        //    ResetHighlightExceptConfirm();
    }

    public void RemoveUnit(UnitBase unit)
    {
        Units_MonsterRemove(unit as MonsterUnit);
    }

    //Drag Drop
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log($"Pointer is drop here to {eventData.pointerCurrentRaycast.worldPosition} Tile! ");
        TileMaker.Instance.LastDropPos = index;
        TileMaker.Instance.LastHalfTile = WhichPartOfTile(eventData.pointerCurrentRaycast.worldPosition);
    }

    //Click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isHighlight)
            return;

        if (!bm.IsWaitingTileSelect && !tileMaker.IsWaitingToSelectTrapTile)
            return;

        // 1) 설치 단계에서 타일 클릭
        if(bm.FSM.curState == BattleState.Start)
        {
            tileMaker.LastSelectedTile = this;
            tileMaker.IsWaitingToSelectTrapTile = false;
        }
        // 2) 스킬사용중에 타일 클릭
        else
        {
            var skill = BottomUIManager.Instance.curSkillButton.skill;
            //var item = BottomUIManager.Instance.curSkillButton.item;

            var actionType = BottomUIManager.Instance.buttonState;

            if (actionType == BottomUIManager.ButtonState.Skill)
            {
                bm.DoCommand(skill.SkillTableElem.player, index, skill);
            }
            else
            {
                //manager.DoCommand(item as DataConsumable);
            }

            BottomUIManager.Instance.curSkillButton.Cancle_UseSkill();
            BottomUIManager.Instance.InteractiveSkillButton(skill.SkillTableElem.player, false);
            bm.tileLink.EndTileClick();
            bm.uiLink.UpdateProgress(); 
        }
    }

    public void Units_UnitAdd(UnitBase unit)
    {
        if(units[0] == null)
        {
            units[0] = unit;
        }
        else if(units[1] == null)
        {
            units[1] = unit;
        }
    }

    public int Units_UnitCount()
    {
        int count = 0;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] != null)
                ++count;
        }
        return count;
    }

    public void Units_MonsterRemove(MonsterUnit unit)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == unit)
            {
                units[i] = null;
                return;
            }
        }
    }
}
