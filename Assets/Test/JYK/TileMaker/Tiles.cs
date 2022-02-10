using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public enum HalfTile { Front, Behind }
public class AffectedInfo
{
    public bool isAffected;
    public PlayerType player; // ������ ��ų�� ������ �ִ°� -> �� ����
    public SkillRangeType skillRange; // � ������ ��ų�ΰ� -> Half ���� Full ���� ����
    public HalfTile half; // ��/�� -> ��� Ÿ���� ĥ�ؾ��ϴ°�
}


public class Tiles : MonoBehaviour, IPointerClickHandler
{
    public MeshRenderer ren;
    public Vector2 index;
    public bool isObstacle;
    [SerializeField] private List<UnitBase> units = new List<UnitBase>();
    public Obstacle obstacle = null;

    //Instance
    private TileMaker tileMaker;
    private BattleManager bm;
    [Space(15)] // ��ֹ� �̶� ���� �Ϸ��� �߰�
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
    public bool isHighlightConsume;
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
        //���� �ʱ�ȭ. � ���̶���Ʈ�� ����.
        affectedByBoy.isAffected = false;
        affectedByGirl.isAffected = false;
        isHighlight = false;
        isHighlightConsume = false;
        ren.material.color = tileMaker.noneColor;
        center.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
    }

    public void HighlightSkillRange()
    {
        // �巡�� ���� Ÿ�Ͽ����� ���� ��ų�� ǥ�õǴ� ���. �������� ǥ��.
        center.material.color = tileMaker.dragColor;
    }

    public void HighlightSkillRange(SkillRangeType range, Vector3 dragWorldPos) // �巡������ ��ų ���� ����ǥ��
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

    public void HighlightCanAttackSign(SkillRangeType range) // ���� �׵θ� ǥ��
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

    public void HighlightCanConsumeSign()
    {
        // Both drag and click
        isHighlightConsume = true;
        edge.material.color = tileMaker.consumeColor;
    }

    public void ResetHighlightExceptConfirm() // ��ų ��� ���� ȣ��.
    {
        isHighlight = false;
        isHighlightConsume = false;

        center.material.color = tileMaker.noneColor;
        front.material.color = tileMaker.noneColor;
        behind.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
        frontEdge.material.color = tileMaker.noneColor;
        behindEdge.material.color = tileMaker.noneColor;

        if (!affectedByBoy.isAffected && !affectedByGirl.isAffected)
            return; // ���õ��� ���� Ÿ���̶�� ����.

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

        // �� �� �ϳ��� ���õǾ��ִٸ�, ���Ӱ� ��ĥ.
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
        // �Ѿ�� ������ �������� AffectedInfo �籸��.
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
        //�巡�� �� �϶��� ȣ��.
        //���̶���Ʈ�� �ʱ�ȭ���� ������, ���콺 �巡���߿� ǥ�õǴ� ��� ����Ʈ�� ���ش�.
        center.material.color = tileMaker.noneColor;
        front.material.color = tileMaker.noneColor;
        behind.material.color = tileMaker.noneColor;

        if (isHighlight)
            HighlightCanAttackSign(skillType);
        if (isHighlightConsume)
            HighlightCanConsumeSign();
        //if (affectedByBoy.isAffected || affectedByGirl.isAffected)
        //    ResetHighlightExceptConfirm();
    }

    public void RemoveUnit(UnitBase unit)
    {
        Units_MonsterRemove(unit as MonsterUnit);
    }

    public void ClearUnit()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i] = null;
        }
    }

    //Click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (bm.isTutorial && bm.tutorial.lockTileClick)
            return;

        if (!isHighlight && !isHighlightConsume)
            return;

        if (!bm.IsWaitingTileSelect && !tileMaker.IsWaitingToSelectTrapTile)
            return;

        if (bm.isTutorial && !bm.tutorial.tu_04_TileClick && index == new Vector2(1, 5))
            bm.tutorial.tu_04_TileClick = true;
        else if (bm.isTutorial && !bm.tutorial.tu_04_TileClick && index != new Vector2(1, 5))
            return;

        if (bm.isTutorial && !bm.tutorial.tu_07_BoySkill2 && index == new Vector2(1, 6))
            bm.tutorial.tu_07_BoySkill2 = true;
        else if (bm.isTutorial && !bm.tutorial.tu_10_GirlSkill2 && index == new Vector2(1, 6))
            bm.tutorial.tu_10_GirlSkill2 = true;
        else if (bm.isTutorial && !bm.tutorial.tu_12_GirlSkill2 && index == new Vector2(0, 0))
            bm.tutorial.tu_12_GirlSkill2 = true;

        // 1) ��ġ �ܰ迡�� Ÿ�� Ŭ��
        if (bm.FSM.curState == BattleState.Start)
        {
            tileMaker.LastSelectedTile = this;
            tileMaker.IsWaitingToSelectTrapTile = false;
        }
        // 2) ��ų����߿� Ÿ�� Ŭ��
        else
        {
            // �����ɽ�Ʈ ����
            var ray = Camera.main.ScreenPointToRay(eventData.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask("Tile")))
            {
                tileMaker.LastClickPos = hit.point;
            }

            var skill = BottomUIManager.Instance.curSkillButton.skill;

            var actionType = BottomUIManager.Instance.buttonState;

            if (actionType == BottomUIManager.ButtonState.Skill)
            {
                bm.DoCommand(skill.SkillTableElem.player, index, skill, false);
            }

            BottomUIManager.Instance.curSkillButton.Cancle_UseSkill();
            BottomUIManager.Instance.InteractiveSkillButton(skill.SkillTableElem.player, false);

            bm.tileLink.EndTileClick();

            var progressIcon = skill.SkillTableElem.player == PlayerType.Boy ? BattleUI.ProgressIcon.Boy : BattleUI.ProgressIcon.Girl;
            bm.uiLink.UpdateProgress(progressIcon); 
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
