using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class HuntTilesMaker : MonoBehaviour
{
    public HuntPlayer player;
    public GameObject bush;
    public Material[] bushMaterials;

    public GameObject wholeTile;
    public Material material;
    public int row = 3;
    public int col = 7;
    public float spacing = 1f;
    public int[] randomBush;

    private void Start()
    {
        MakeTiles();
    }

    private void MakeTiles()
    {
        // 은폐물
        randomBush = new int[col - 2];
        for (int i = 0; i < randomBush.Length; i++)
        {
            randomBush[i] = Random.Range(0, 3);
        }

        var bound = wholeTile.GetComponent<MeshRenderer>().bounds;
        var maxX = bound.max.x; //가로
        var minX = bound.min.x;
        var maxZ = bound.max.z; //세로
        var minZ = bound.min.z;

        var width = maxX - minX;
        var height = maxZ - minZ;

        var tileWidth = (width - spacing * (col + 1)) / col;
        var tileHeight = (height - spacing * (row + 1)) / row;

        var startPos = new Vector3(minX + tileWidth / 2, wholeTile.transform.position.y + 0.01f, minZ + tileHeight / 2);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                var x = startPos.x + spacing * (j + 1) + tileWidth * j;
                var z = startPos.z + spacing * (i + 1) + tileHeight * i;
                Vector3 offset = new Vector3(x, startPos.y, z);
                var go = CreateTileQuad(new Vector2(i, j), tileWidth, tileHeight);
                go.transform.position = offset;
            }
        }
    }


    private GameObject CreateTileQuad(Vector2 index, float width, float height)
    {
        GameObject plane = new GameObject("Tile Plane");  //빈 게임오브젝트 생성
        plane.transform.SetParent(transform);

        MeshFilter mf = plane.AddComponent<MeshFilter>();
        var ren = plane.AddComponent<MeshRenderer>();
        var tile = plane.AddComponent<HuntTile>();
        tile.materials = bushMaterials;
        tile.player = player;
        tile.index = index;
        tile.ren = ren;

        // 은폐물
        var bushIndex = (int)index.y;
        if(bushIndex > 0 && bushIndex < col - 1)
        {
            if(randomBush[bushIndex - 1].Equals((int)index.x))
            {
                var go = Instantiate(bush, tile.transform);
                tile.bush = go.GetComponent<Bush>();
            }
        }

        var meshCol = plane.AddComponent<MeshCollider>();

        var mesh = new Mesh();
        mf.mesh = mesh;
        ren.material = material;
        meshCol.isTrigger = meshCol.convex = true;
        meshCol.sharedMesh = mesh;

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
}
