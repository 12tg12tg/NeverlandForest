using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMapGround : MonoBehaviour
{
    [Header("Tree")]
    public GameObject tree;
    public GameObject dryTree;
    public GameObject cutDryTree;

    [Header("Map")]
    public MeshRenderer land;

    private Vector3 min;
    private Vector3 max;

    private void Awake()
    {
        min = land.bounds.min;
        max = land.bounds.max;

    }

    public void CreateTree(List<Edge> edges, WorldMapNode[,] nodes)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            Debug.Log($"{edges[i].start}, {edges[i].end}");
        }

        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                if(nodes[i, j] != null)
                    Debug.Log($"{nodes[i, j].index}");
            }
        }

        //WorldMapNode[][] no = new WorldMapNode[10][];
        //no.Where(x => x.Length > 0);
        //WorldMapNode[,] no1 = new WorldMapNode[5,5];
        //no1.wh


        //nodes.where()


        var go = new GameObject("Tree");
        go.transform.SetParent(transform);

        for (int i = 0; i < 100; i++)
        {
            var x = Random.Range(min.x, max.x); // ����
            var z = Random.Range(min.z, max.z); // ����
            var tr = Instantiate(tree, new Vector3(x, 0f, z), Quaternion.identity);
            tr.transform.SetParent(go.transform);
        }

        // ������ �����ϰ� ���� �� �� ������ ������ �ȵǰ� ��嵵 ������ �ȵȴ�
    }
}
