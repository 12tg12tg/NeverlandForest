using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    private static TileMaker instance;
    public static TileMaker Instance { get => instance; }

    //Instance
    public GameObject wholeTile;
    public BattleManager manager;

    //Prefabs
    public Material material;

    //Property
    public Vector2 LastDropPos { get; set; }
    public Tiles LastDropTile { get => GetTile(LastDropPos); }

    //Vars
    public int row = 3;
    public int col = 7;
    public float spacing = 1f;
    private Tiles[,] allTiles;
    private List<Tiles> tileList = new List<Tiles>();

    private void Awake()
    {
        instance = this;
        MakeTiles();
    }

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
                var go = CreateTileQuad(new Vector2(i, j), tileWidth, tileHeight);
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
            var go = CreateTileQuad(new Vector2(i, 0), tileWidth, tileHeight);
            go.transform.position = offset;
        }
    }

    private GameObject CreateTileQuad(Vector2 index, float width, float height)
    {
        GameObject plane = new GameObject("Tile Plane");  //�� ���ӿ�����Ʈ ����
        plane.transform.SetParent(transform);
        plane.layer = LayerMask.NameToLayer("Tile");

        MeshFilter mf = plane.AddComponent<MeshFilter>();
        var ren = plane.AddComponent<MeshRenderer>();
        var tile = plane.AddComponent<Tiles>();
        tile.index = index;
        tile.ren = ren;
        allTiles[(int)index.x, (int)index.y] = tile;
        tileList.Add(tile);

        var col = plane.AddComponent<MeshCollider>();

        var mesh = new Mesh();
        mf.mesh = mesh;
        ren.material = material;
        col.sharedMesh = mesh;

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

    public Tiles GetTile(Vector2 position)
    {
        if (IsValidTile(position))
            return allTiles[(int)position.x, (int)position.y];
        else return null;
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

    public void SetAllTileMiddleState()
    {
        var count = tileList.Count;
        for (int i = 0; i < count; i++)
        {
            tileList[i].SetMiddleState();
        }
    }

    public void SetAllTileSoftClear()
    {
        tileList.ForEach((n) => n.ResetHighlightExceptConfirm());
    }

    public void SetAllTileHardClear(PlayerType type)
    {
        tileList.Where((n) => n.affectedPlayer == type).ToList().ForEach((n) => n.Clear());
    }

    public bool IsObstacleTile(Vector2 position)
    {
        return allTiles[(int)position.x, (int)position.y].isObstacle;
    }

    private bool IsValidTile(Vector2 tilePos)
    {
        var x = (int)tilePos.x;
        var y = (int)tilePos.y;
        return 0 <= x && x < row && y >= 0 && y < col;
    }

    public List<UnitBase> UnitOnTile(Vector2 pos)
    {
        var tile = GetTile(pos);
        return tile.units;
    }

}
