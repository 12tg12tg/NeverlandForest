using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    public enum PlaneType
    {
        Base, Center, Edge, Front, Behind, FrontEdge, BehindEdge, Count
    }

    private static TileMaker instance;
    public static TileMaker Instance { get => instance; }

    //Instance
    public GameObject wholeTile;
    public BattleManager manager;

    //Prefabs
    public Material baseMaterial;
    public Material centerMaterial;
    public Material edgeMaterial;
    public Material frontMaterial;
    public Material behindMaterial;
    public Material frontEdgeMaterial;
    public Material behindEdgeMaterial;

    //Property
    public HalfTile LastHalfTile { get; set; }
    public Vector3 LastClickPos { get; set; }
    public Vector2 LastDropPos { get; set; }
    public Tiles LastDropTile { get => GetTile(LastDropPos); }

    //Vars
    public Color blueColor;
    public Color consumeColor;
    public Color targetColor;
    public Color noneColor;
    private float reductionRatio = 0.9f;
    public int row = 3;
    public int col = 7;
    public float spacing = 1f;
    private Tiles[,] allTiles;
    private List<Tiles> tileList = new List<Tiles>();
    private Dictionary<int, int> lanternToCol = new Dictionary<int, int>()
    {
        { 0, 0 }, { 1, 2 }, { 2, 4 }, { 3, 5 }, { 4, 6 }
    };

    private void Awake()
    {
        instance = this;
        MakeTiles();
    }

    // ����
    private void MakeTiles()
    {
        allTiles = new Tiles[row, col];
        var bound = wholeTile.GetComponent<MeshRenderer>().bounds;
        var maxX = bound.max.x; //����
        var minX = bound.min.x;
        var maxZ = bound.max.z; //����
        var minZ = bound.min.z;

        var width = maxX - minX;
        var height = maxZ - minZ;

        var tileWidth = (width - spacing * (col + 1)) / col;
        var tileHeight = (height - spacing * (row + 1)) / row;

        var startPos = new Vector3(minX + tileWidth / 2, wholeTile.transform.position.y + 0.01f, minZ + tileHeight / 2);
        for (int i = 0; i < row; i++)
        {
            for (int j = 1; j < col; j++)
            {
                var x = startPos.x + spacing * (j + 1) + tileWidth * j;
                var z = startPos.z + spacing * (i + 1) + tileHeight * i;
                Vector3 offset = new Vector3(x, startPos.y, z);
                Vector2 index = new Vector2(i, j);
                var go = CreateTileQuad(index, PlaneType.Base, tileWidth, tileHeight);
                var tile = go.GetComponent<Tiles>();
                CreateSubQuads(tileWidth, tileHeight, index, tile);
                go.transform.position = offset;
            }
        }

        //Player Tile : 2
        var playerSpacing = (height - tileHeight * 2) / 3;
        for (int i = 0; i < 2; i++)
        {
            var x = startPos.x + spacing;
            var z = startPos.z + playerSpacing * (i + 1) + tileHeight * i;
            Vector3 offset = new Vector3(x, startPos.y, z);
            Vector2 index = new Vector2(i, 0);
            var go = CreateTileQuad(index, PlaneType.Base, tileWidth, tileHeight);
            var tile = go.GetComponent<Tiles>();
            CreateSubQuads(tileWidth, tileHeight, index, tile);
            go.transform.position = offset;
        }
    }

    private void CreateSubQuads(float tileWidth, float tileHeight, Vector3 index, Tiles parent)
    {
        int count = (int)PlaneType.Count;
        for (int i = 1; i < count; i++)
        {
            var type = (PlaneType)(i);

            var child = CreateTileQuad(index, type, tileWidth, tileHeight);
            child.transform.SetParent(parent.transform);

            switch (type)
            {
                case PlaneType.Center:
                    parent.center = child.GetComponent<MeshRenderer>();
                    break;
                case PlaneType.Edge:
                    parent.edge = child.GetComponent<MeshRenderer>();
                    break;
                case PlaneType.Front:
                    parent.front = child.GetComponent<MeshRenderer>();
                    break;
                case PlaneType.Behind:
                    parent.behind = child.GetComponent<MeshRenderer>();
                    break;
                case PlaneType.FrontEdge:
                    parent.frontEdge = child.GetComponent<MeshRenderer>();
                    break;
                case PlaneType.BehindEdge:
                    parent.behindEdge = child.GetComponent<MeshRenderer>();
                    break;
            }
        }
    }

    private GameObject CreateTileQuad(Vector2 index, PlaneType type, float width, float height)
    {
        GameObject plane = new GameObject(type.ToString());  //�� ���ӿ�����Ʈ ����
        Material material = null;

        MeshFilter mf = plane.AddComponent<MeshFilter>();
        var ren = plane.AddComponent<MeshRenderer>();

        var mesh = new Mesh();
        mf.mesh = mesh;


        switch (type)
        {
            case PlaneType.Base:
                plane.layer = LayerMask.NameToLayer("Tile");
                plane.transform.SetParent(transform);
                material = baseMaterial;

                var col = plane.AddComponent<MeshCollider>();
                col.sharedMesh = mesh;

                var tile = plane.AddComponent<Tiles>();
                tile.index = index;
                tile.ren = ren;

                allTiles[(int)index.x, (int)index.y] = tile;
                tileList.Add(tile);
                break;

            case PlaneType.Center:
                width *= reductionRatio;
                height *= reductionRatio;
                material = centerMaterial;
                plane.transform.position += new Vector3(0f, 0.01f, 0f);
                break;

            case PlaneType.Edge:
                material = edgeMaterial;
                plane.transform.position += new Vector3(0f, 0.01f, 0f);
                break;

            case PlaneType.Front:
                width *= reductionRatio;
                height *= reductionRatio;
                material = frontMaterial;
                plane.transform.position += new Vector3(0f, 0.015f, 0f);
                break;

            case PlaneType.Behind:
                width *= reductionRatio;
                height *= reductionRatio;
                material = behindMaterial;
                plane.transform.position += new Vector3(0f, 0.015f, 0f);
                break;

            case PlaneType.FrontEdge:
                material = frontEdgeMaterial;
                plane.transform.position += new Vector3(0f, 0.02f, 0f);
                break;

            case PlaneType.BehindEdge:
                material = behindEdgeMaterial;
                plane.transform.position += new Vector3(0f, 0.02f, 0f);
                break;
        }

        ren.material = material;


        var vertices  = new Vector3[4];

        Vector3 offset = new Vector3(- width / 2, 0f, - height / 2);
        vertices[0] = offset + new Vector3(0f, 0f, 0f);
        vertices[1] = offset + new Vector3(width, 0f, 0f);
        vertices[2] = offset + new Vector3(0f, 0f, height);
        vertices[3] = offset + new Vector3(width, 0f, height);

        mesh.vertices = vertices;

        var tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        var normals = new Vector3[4];

        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;

        mesh.normals = normals;

        var uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        return plane;
    }


    // Ÿ�� ��ȯ
    public Tiles GetTile(Vector2 position)
    {
        if (IsValidTile(position))
            return allTiles[(int)position.x, (int)position.y];
        else return null;
    }

    public Tiles[] GetMovableTilesInSameRow(Tiles curTile)
    {
        //���� �࿡ �̵������� �ݰ��̸鼭 �̹� 2���� ������ ������ ���� Ÿ�ϵ��� ��� ��ȯ.
        var curX = curTile.index.x;
        var curY = curTile.index.y;
        var maxMovableRange = curTile.MaxMovableRange();
        var list = from n in tileList
                   where n.index.x == curX && (n.index.y >= curY - maxMovableRange && n.index.y <= curY - 1) && n.CanStand
                   select n;
        return list.ToArray();
    }

    public Tiles[] GetMovableTiles(Tiles curTile)
    {
        //���� ������� �ʰ� �̵������� �ݰ��̸鼭 �̹� 2���� ������ ������ ���� Ÿ�ϵ��� ��� ��ȯ.
        var curX = curTile.index.x;
        var curY = curTile.index.y;
        var maxMovableRange = curTile.MaxMovableRange();
        var list = from n in tileList
                   where (n.index.y >= curY - maxMovableRange && n.index.y <= curY - 1) && n.CanStand
                   select n;
        return list.ToArray();
    }

    public IEnumerable<Tiles> GetMovablePathTiles(Tiles curTile, Tiles goalTile)
    {
        var curX = curTile.index.x;
        var curY = curTile.index.y;
        var goalY = goalTile.index.y;

        var list = from n in tileList
                   where n.index.x == curX && n.index.y <= curY - 1f && n.index.y >= goalY
                   select n;
        return list;
    }


    public IEnumerable<Tiles> GetMonsterTiles()
    {
        var list = from n in tileList
                   where n.HaveUnit && n.index.y != 0
                   select n;
        return list;
    }

    public IEnumerable<Tiles> GetPlayerTiles()
    {
        var list = from n in tileList
                   where n.index.y == 0 && n.HaveUnit
                   select n;
        return list;
    }

    public IEnumerable<Tiles> GetSkillRangedTiles(Vector2 choicesTile, SkillRangeType rangeType)
    {
        switch (rangeType)
        {
            case SkillRangeType.One:
            case SkillRangeType.Tile:
                return from n in tileList where n == GetTile(choicesTile) select n;

            case SkillRangeType.Line:
                int col = (int)choicesTile.y;
                return from n in tileList where (int)n.index.y == col select n;

            case SkillRangeType.Lantern:
                int currentLantern = (int)Vars.UserData.uData.lanternState;
                var maxCol = lanternToCol[currentLantern];
                return from n in tileList where (int)n.index.y != 0 && (int)n.index.y <= maxCol select n;

            default:
                return null;
        }
    }    

    // Ÿ�� �Ӽ� ����
    //public void SetAllTileMiddleState()
    //{
    //    var count = tileList.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        tileList[i].SetMiddleState();
    //    }
    //}

    public void SetAllTileSoftClear()
    {
        tileList.ForEach((n) => n.ResetHighlightExceptConfirm());
    }

    internal void AffectedTileCancle(PlayerType type)
    {
        if(type == PlayerType.Boy)
            tileList.Where(n => n.affectedByBoy.isAffected).ToList().ForEach(n => n.CancleConfirmTarget(type));
        else
            tileList.Where(n => n.affectedByGirl.isAffected).ToList().ForEach(n => n.CancleConfirmTarget(type));
    }

    private bool IsValidTile(Vector2 tilePos)
    {
        var x = (int)tilePos.x;
        var y = (int)tilePos.y;
        return 0 <= x && x < row && y >= 0 && y < col;
    }

    // ���ֹ�ȯ
    public IEnumerable<UnitBase> GetUnitsOnTile(Vector2 pos)
    {
        return GetTile(pos).Units;
    }

    public IEnumerable<MonsterUnit> GetTargetList(Vector2 targetPos, SkillRangeType rangeType)
    {
        var tiles = GetSkillRangedTiles(targetPos, rangeType);
        IEnumerable<MonsterUnit> list = null;
        switch (rangeType)
        {
            case SkillRangeType.One:
                list = from tile in tiles
                       from n in tile.Units
                       where (tile.Units_UnitCount() == 1)? true : 
                                (tile.WhichPartOfTile(LastClickPos) == HalfTile.Front ? n == tile.FrontMonster : n == tile.BehindMonster)
                       select n as MonsterUnit;
                break;
            case SkillRangeType.Tile:
            case SkillRangeType.Line:
            case SkillRangeType.Lantern:
                list = from tile in tiles
                       from n in tile.Units
                       select n as MonsterUnit;
                break;
        }

        return list;
    }
}
