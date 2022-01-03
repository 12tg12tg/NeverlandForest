using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class TilePrefab : MonoBehaviour
{
    public GameObject mainLand;
    public Material[] material;
    public Material defaultMaterial;
    public int row = 7;
    public int col = 3;
    public float spacing = 1f;

    private void Start()
    {
        MakeTiles();
    }

    private void MakeTiles()
    {
        var bound = mainLand.GetComponent<MeshRenderer>().bounds;
        var maxX = bound.max.x; //가로
        var minX = bound.min.x;
        var maxZ = bound.max.z; //세로
        var minZ = bound.min.z;

        var width = maxX - minX;
        var height = maxZ - minZ;

        var tileWidth = (width - spacing * (col + 1)) / col;
        var tileHeight = (height - spacing * (row + 1)) / row;

        var startPos = new Vector3(minX + tileWidth / 2, mainLand.transform.position.y + 0.01f, minZ + tileHeight / 2);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                var x = startPos.x + spacing * (j + 1) + tileWidth * j;
                var z = startPos.z + spacing * (i + 1) + tileHeight * i;
                Vector3 offset = new Vector3(x, startPos.y, z);
                var go = CreateTileQuad(new Vector2(i, j), "Tile", tileWidth, tileHeight, defaultMaterial);
                for (int k = 0; k < material.Length; k++)
                {
                    var newOne = CreateTileQuad(new Vector2(i, j), material[k].name, tileWidth, tileHeight, material[k]);
                    newOne.transform.SetParent(go.transform);
                    newOne.SetActive(false);
                }
                go.transform.position = offset;
            }
        }
    }


    private GameObject CreateTileQuad(Vector2 index, string name, float width, float height, Material material)
    {
        GameObject plane = new GameObject(name);  //빈 게임오브젝트 생성
        plane.transform.SetParent(transform);
        if(name.Equals("PlaneWhite"))
        {
            width *= 0.9f;
            height *= 0.9f;
        }

        MeshFilter mf = plane.AddComponent<MeshFilter>();
        var ren = plane.AddComponent<MeshRenderer>();
        
        if (!name.Equals("Tile"))
        {
            plane.transform.position += new Vector3(0f, 0.01f, 0f);
        }
        else
        {
            var tile = plane.AddComponent<TileOption>();
            tile.index = index;
            tile.meshRenderer = ren;
        }

        var meshCol = plane.AddComponent<MeshCollider>();

        var mesh = new Mesh();
        mf.mesh = mesh;
        ren.material = material;
        
        meshCol.sharedMesh = mesh;

        var vertices = new Vector3[4];

        Vector3 offset = new Vector3(-width / 2, 0f, -height / 2);
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
