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
    public Obstacle obstacle = null;

    //Instance
    private TileMaker tileMaker;
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
        var obstacleType = BottomUIManager.Instance.curObstacleType;
        var bm = BattleManager.Instance;
        if (!bm.IsWaitingTileSelect && obstacleType == ObstacleType.None)
            return;

        if(obstacle != null && obstacleType != ObstacleType.None)
        {
            Debug.Log("��ֹ��� �̹� ��ġ�Ǿ� �ֽ��ϴ�");
            return;
        }

        // ��繮�� �ƴ� �� �庮�� ��ġ ���ϵ��� ���ƾ� ��
        var manager = BattleManager.Instance;
        if(manager.FSM.curState == BattleState.Start)
        {
            if (!Inventory_Virtual.instance.isLasso)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i] != null) // �÷��̾ ��ġ�� �� ��ֹ� ��ġ ���ϵ��� ���� �뵵
                        return;
                }

                // ���߿� ������ ���̺��� ����� �ѹ��� ������ �� ����
                GameObject os = default;
                if(!obstacleType.Equals(ObstacleType.Barrier))
                {
                    // �ð��̸� ��ġ �Ϸ��� �� �� �ֺ��� �ι� ° �ð��̸� ��ġ�� ���� ���� ��� �ȵǰԲ�
                    if (obstacleType.Equals(ObstacleType.Lasso))
                    {
                        var list = tileMaker.GetNearUpDownTiles(index).ToList();
                        var obList = list.Where(x => x.obstacle == null).Select(x => x).ToList();
                        if (obList.Count == 0)
                        {
                            Debug.Log("�ι� ° �ð��̸� ��ġ�� ������ �����ϴ�. �ٸ����� ��ġ�ϼ���");
                            return;
                        }
                    }

                    var prefab = Inventory_Virtual.instance.obstaclePrefab[(int)obstacleType - 1];
                    os = Instantiate(prefab, transform);
                    var rigid = os.AddComponent<Rigidbody>();
                    rigid.useGravity = false;
                    rigid.isKinematic = true;

                    // ��ֹ� �ɷ� �ο�(������ ���̺� ����� �ٲ��� ��)
                    obstacle = new Obstacle();
                    obstacle.prefab = os;
                    obstacle.type = obstacleType;
                    obstacle.trapDamage = obstacleType.Equals(ObstacleType.Lasso) ? 0 : 1;
                    obstacle.numberOfInstallation = obstacleType.Equals(ObstacleType.Lasso) ? 2 : 1;
                    obstacle.hp = obstacleType.Equals(ObstacleType.Barrier) ? 10 : 0;

                    obstacle.numberOfInstallation--;
                    if (obstacle.numberOfInstallation == 0)
                        BottomUIManager.Instance.curObstacleType = ObstacleType.None;
                    else
                    {
                        Inventory_Virtual.instance.isLasso = true;
                    }
                }
                else if(obstacleType.Equals(ObstacleType.Barrier))
                {
                    if ((int)index.y == tileMaker.col - 1) // �庮�� ������ ���� ���� �ϴ� ������ ��ġ ���ϵ���
                        return;

                    // �庮�� ��ġ�� �ش� ���� �ƹ��͵� ����� ��ġ�� �����ϴ�
                    var list = tileMaker.GetNearRowTiles(index).ToList();
                    var obList = list.Where(x => x.obstacle != null).Select(x => x).ToList();
                    if (obList.Count != 0)
                    {
                        Debug.Log("�庮�� ��ġ�� ������ �����մϴ�. �ٸ����� ��ġ�ϼ���");
                        return;
                    }

                    var barrierTile = tileMaker.GetNearRowTiles(index).ToList();
                    Obstacle barrier = default;
                    for (int i = 0; i < barrierTile.Count; i++)
                    {
                        if ((int)(barrierTile[i].index.x) == 1)
                        {
                            var prefab = Inventory_Virtual.instance.obstaclePrefab[(int)obstacleType - 1];
                            barrierTile[i].obstacle = new Obstacle();
                            barrierTile[i].obstacle.prefab = Instantiate(prefab, barrierTile[i].transform);
                            barrierTile[i].obstacle.type = obstacleType;
                            barrierTile[i].obstacle.hp = 10;
                            barrier = barrierTile[i].obstacle;
                        }
                        else
                        {
                            barrierTile[i].obstacle = new Obstacle();
                            barrierTile[i].obstacle.type = obstacleType;
                        }
                    }
                    barrierTile.ForEach(x => x.obstacle.pair.Add(barrier));

                    BottomUIManager.Instance.curObstacleType = ObstacleType.None;
                }
            }
            else
            {
                // �ð��� �ι� ° ��ġ ��ġ�ϴ� ��
                var lassoTile = tileMaker.TileList
                    .Where(x => x.obstacle != null)
                    .Where(x => x.obstacle.type.Equals(ObstacleType.Lasso))
                    .Where(x => x.obstacle.numberOfInstallation == 1)
                    .Select(x => x).First();

                var list = tileMaker.GetNearUpDownTiles(lassoTile.index).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].index.Equals(index))
                    {
                        var prefab = Inventory_Virtual.instance.obstaclePrefab[(int)obstacleType - 1];
                        var os = Instantiate(prefab, transform);
                        var rigid = os.AddComponent<Rigidbody>();
                        rigid.useGravity = false;
                        rigid.isKinematic = true;
                        obstacle = new Obstacle();
                        obstacle.prefab = os;
                        obstacle.type = obstacleType;
                        obstacle.pair.Add(lassoTile.obstacle);
                        lassoTile.obstacle.pair.Add(obstacle);
                        lassoTile.obstacle.numberOfInstallation--;
                        Inventory_Virtual.instance.isLasso = false;
                        BottomUIManager.Instance.curObstacleType = ObstacleType.None;
                        return;
                    }
                }
            }
        }
        else
        {
            var skill = BottomUIManager.Instance.curSkillButton.skill;
            //var item = BottomUIManager.Instance.curSkillButton.item;

            var actionType = BottomUIManager.Instance.buttonState;

            if (skill.SkillTableElem.range == SkillRangeType.One)
            {
                //���� Ÿ���̶�� ��ȿ�� �˻�
                if (!bm.tileLink.IsVaildTargetTile(this))
                {
                    manager.uiLink.PrintCaution("���� Ÿ�� ��ų�� �ݵ�� ����� �����ؾ� �մϴ�.", 0.7f, 0.5f, null);
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
            manager.tileLink.EndTileClick();
            manager.uiLink.UpdateProgress(); 
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
