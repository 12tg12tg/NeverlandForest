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


public class Tiles : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public MeshRenderer ren;
    public Vector2 index;
    public bool isObstacle;
    [SerializeField]private List<UnitBase> units = new List<UnitBase>();

    //Instance
    private TileMaker tileMaker;
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
    private bool isHighlightAttack;
    private bool isHighlightConsume;
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
    private void MoveUnitCenter(UnitBase unit)
    {
        StartCoroutine(Utility.CoTranslate(unit.transform, unit.transform.position, CenterPos, 0.3f));
    }
    private void MoveUnitFront(UnitBase unit)
    {
        StartCoroutine(Utility.CoTranslate(unit.transform, unit.transform.position, FrontPos, 0.3f));
    }

    private void MoveUnitBehind(UnitBase unit)
    {
        StartCoroutine(Utility.CoTranslate(unit.transform, unit.transform.position, BehindPos, 0.3f));
    }

    public HalfTile WhichPartOfTile(Vector3 touchPos)
    {
        float x = touchPos.x;
        float touchY = touchPos.z;

        float slopeY = (x - CenterPos.x) * Height / Width + CenterPos.z;

        return (touchY > slopeY) ? HalfTile.Front : HalfTile.Behind;
    }

    public void PutMonster(MonsterUnit monster)
    {
        if (Units_UnitCount() == 0)
        {
            Units_UnitAdd(monster);
            monster.Pos = index;
        }
        else if (Units_UnitCount() == 1)
        {
            Units_UnitAdd(monster);
            monster.Pos = index;
            MoveUnitFront(units[0]);
            MoveUnitBehind(units[1]);
        }
        else
        {
            Debug.LogError("Tile has monster more than 2.");
        }
    }


    //Highlight
    public void Clear()
    {
        //���� �ʱ�ȭ. � ���̶���Ʈ�� ����.
        affectedByBoy.isAffected = false;
        affectedByGirl.isAffected = false;
        isHighlightAttack = false;
        isHighlightConsume = false;
        ren.material.color = tileMaker.noneColor;
        center.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
    }

    public void HighlightSkillRange()
    {
        // For Drag
        center.material.color = tileMaker.blueColor;
    }

    public void HighlightCanAttackSign(SkillRangeType range)
    {
        // Both drag and click
        isHighlightAttack = true;

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

    public void ResetHighlightExceptConfirm()
    {       
        isHighlightAttack = false;
        isHighlightConsume = false;

        center.material.color = tileMaker.noneColor;
        front.material.color = tileMaker.noneColor;
        behind.material.color = tileMaker.noneColor;
        edge.material.color = tileMaker.noneColor;
        frontEdge.material.color = tileMaker.noneColor;
        behindEdge.material.color = tileMaker.noneColor;

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
        if (!affectedByBoy.isAffected && !affectedByGirl.isAffected)
            return; // ���õ��� ���� Ÿ���̶�� ����.

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

    /*
    private void ReCalculateAffectedColor()
    {
        Color color = Color.clear;
        if (affectedByBoy.isAffected)
            color += boyColor;
        if (affectedByGirl.isAffected)
            color += girlColor;
        this.confirmColor = color;
    }
    */ // (������) �� ���ϱ� �Լ�. ���� ��� �������� �ٲ�鼭 �ʿ� ������.

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

    //public void SetMiddleState()
    //{
    //    //�巡�� �� �϶��� ȣ��.
    //    //���̶���Ʈ�� �ʱ�ȭ���� ������, ���콺 �巡���߿� ǥ�õǴ� ��� ����Ʈ�� ���ش�.
    //    center.material.color = tileMaker.noneColor;
    //    if (isHighlightAttack)
    //        HighlightCanAttackSign();
    //    if (isHighlightConsume)
    //        HighlightCanConsumeSign();
    //    if (affectedByBoy.isAffected || affectedByGirl.isAffected)
    //        ResetHighlightExceptConfirm();
    //}

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
        if (!BattleManager.Instance.IsWaitingTileSelect )
            return;

        var manager = BattleManager.Instance;

        var skill = BottomUIManager.Instance.curSkillButton.skill;
        //var item = BottomUIManager.Instance.curSkillButton.item;

        var actionType = BottomUIManager.Instance.buttonState;

        if (skill.SkillTableElem.range == SkillRangeType.One)
        {
            //���� Ÿ���̶�� ��ȿ�� �˻�
            if (!BattleManager.Instance.IsVaildTargetTile(this))
            {
                manager.PrintCaution("���� Ÿ�� ��ų�� �ݵ�� ����� �����ؾ� �մϴ�.", 0.7f, 0.5f, null);
                return;
            }
            else
            {
                tileMaker.AffectedTileCancle(skill.SkillTableElem.player);
                tileMaker.LastClickPos = eventData.pointerCurrentRaycast.worldPosition;
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

        if (actionType == BottomUIManager.ButtonState.Skill)
        {
            manager.DoCommand(skill.SkillTableElem.player, index, skill);
        }
        else
        {
            //manager.DoCommand(item as DataConsumable);
        }

        BottomUIManager.Instance.curSkillButton.Cancle();
        BottomUIManager.Instance.InteractiveSkillButton(skill.SkillTableElem.player, false);
        manager.EndTileClick();
        manager.UpdateProgress();
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
