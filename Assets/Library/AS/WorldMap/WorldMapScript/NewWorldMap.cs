using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge
{
    public Vector3 start;
    public Vector3 end;

    public Edge(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }

    public bool IsCrossing(Vector3 start2, Vector3 end2)
    {
        if (System.Math.Max(start.z, end.z) <= System.Math.Min(start2.z, end2.z))
            return false;
        if (System.Math.Min(start.z, end.z) >= System.Math.Max(start2.z, end2.z))
            return false;
        if (System.Math.Max(start.x, end.x) <= System.Math.Min(start2.x, end2.x))
            return false;
        if (System.Math.Min(start.x, end.x) >= System.Math.Max(start2.x, end2.x))
            return false;

        var p1 = (end.z - start.z) * (start2.x - start.x)
            - (start2.z - start.z) * (end.x - start.x);
        var p2 = (end.z - start.z) * (end2.x - start.x)
            - (end2.z - start.z) * (end.x - start.x);
        var p3 = (end2.z - start2.z) * (start.x - start2.x)
            - (start.z - start2.z) * (end2.x - start2.x);
        var p4 = (end2.z - start2.z) * (end.x - start2.x)
            - (end.z - start2.z) * (end2.x - start2.x);

        if (p1 == 0 && p2 == 0 && p3 == 0 && p4 == 0)
            return true;

        return p1 * p2 < 0 && p3 * p4 < 0;
    }
}


public class NewMapNode
{
    public GameObject prefab;
    public List<NewMapNode> Children { get; set; } = new List<NewMapNode>();
    public List<NewMapNode> Parent { get; set; } = new List<NewMapNode>();

    public Vector2 index;
    public int level;
}

public class NewWorldMap : MonoBehaviour
{
    public GameObject cube;
    public Material material;
    
    private bool isTest = true;

    public int column;
    public int row;
    public NewMapNode[,] maps;
    public List<Edge> edges = new List<Edge>();

    private float posX = 2.5f;
    private float posY = 5f;

    private bool isFirst = true; // 테스트용

    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            if (!isFirst)
                ChildrenDestroy();
            MapFirstCreateNode();
            MapNextCreateNode();
            MapLinked();
            
            isFirst = false;
        }
    }

    private void Update()
    {
        PaintLink();
    }

    private void MapFirstCreateNode()
    {
        maps = new NewMapNode[row, column];

        var startEnd = Random.Range(0, row);
        maps[startEnd, 0] = new NewMapNode();
        InitNode(maps[startEnd, 0], new Vector2(startEnd, 0));

        for (int i = 1; i < column - 1; i++)
        {
            var selectNode = Random.Range(0, row);
            maps[selectNode, i] = new NewMapNode();
            InitNode(maps[selectNode, i], new Vector2(selectNode, i));
        }

        maps[startEnd, column - 1] = new NewMapNode();
        InitNode(maps[startEnd, column - 1], new Vector2(startEnd, column - 1));

        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                maps[j, i].level = i;

                if (i < column - 1)
                {
                    for (int k = 0; k < row; k++)
                    {
                        if (maps[k, i + 1] == null)
                            continue;
                        var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
                        edges.Add(edge);

                        maps[j, i].Children.Add(maps[k, i + 1]); // 서로 연결
                        maps[k, i + 1].Parent.Add(maps[j, i]);
                    }
                }
            }
        }
        isTest = false;
    }
    private void MapNextCreateNode()
    {
        var startNextOther = Random.Range(0, row);
        while (maps[startNextOther, 1] != null)
        {
            startNextOther = Random.Range(0, row);
        }
        maps[startNextOther, 1] = new NewMapNode();
        InitNode(maps[startNextOther, 1], new Vector2(startNextOther, 1));

        for (int i = 2; i < column - 2; i++) // 가운데 맵
        {
            for (int j = 0; j < Random.Range(1, 2); j++)
            {
                var rnd = Random.Range(0, row);
                while (maps[rnd, i] != null)
                {
                    rnd = Random.Range(0, row);
                }
                maps[rnd, i] = new NewMapNode();
                InitNode(maps[rnd, i], new Vector2(rnd, i));
            }
        }

        var endBeforeOther = Random.Range(0, row);
        while (maps[endBeforeOther, column - 2] != null)
        {
            endBeforeOther = Random.Range(0, row);
        }
        maps[endBeforeOther, column - 2] = new NewMapNode();
        InitNode(maps[endBeforeOther, column - 2], new Vector2(endBeforeOther, column - 2));
    }
    private void MapLinked()
    {
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                maps[j, i].level = i;

                if(i == 0) // 시작노드
                {
                    for (int k = 0; k < row; k++)
                    {
                        if (maps[k, i + 1] == null)
                            continue;
                        var children = maps[j, i].Children;
                        for (int h = 0; h < children.Count; h++)
                        {
                            if (children[h].Equals(maps[k, i + 1]))
                                break;
                            var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
                            edges.Add(edge);
                            children.Add(maps[k, i + 1]);
                            maps[k, i + 1].Parent.Add(maps[j, i]);
                        }
                    }
                }
                else if (i < column - 1) // 1~8 까지
                {
                    for (int k = 0; k < row; k++)
                    {
                        if (maps[k, i + 1] == null)
                            continue;
                        var children = maps[j, i].Children;
                        if(children.Count == 0)
                        {
                            var pos1 = maps[j, i].prefab.transform.position;
                            var pos2 = maps[k, i + 1].prefab.transform.position;
                            bool isCrossing = default;
                            for (int g = 0; g < edges.Count; g++)
                            {
                                isCrossing = edges[g].IsCrossing(pos1, pos2);
                                Debug.Log(isCrossing);
                                if (isCrossing)
                                    break;
                            }

                            if (!isCrossing)
                            {
                                var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
                                edges.Add(edge);
                                children.Add(maps[k, i + 1]); 
                            }
                        }
                        else
                        {
                            for (int h = 0; h < children.Count; h++)
                            {
                                if (children[h].Equals(maps[k, i + 1]))
                                    break;
                                var pos1 = maps[j, i].prefab.transform.position;
                                var pos2 = maps[k, i + 1].prefab.transform.position;
                                bool isCrossing = default;
                                for (int g = 0; g < edges.Count; g++)
                                {
                                    isCrossing = edges[g].IsCrossing(pos1, pos2);
                                    Debug.Log(isCrossing);
                                    if (isCrossing)
                                        break;
                                }

                                if (!isCrossing)
                                {
                                    var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
                                    edges.Add(edge);
                                    children.Add(maps[k, i + 1]);
                                    maps[k, i + 1].Parent.Add(maps[j, i]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void InitNode(NewMapNode node, Vector2 index)
    {
        node.index = index;
        node.prefab = Instantiate(cube, new Vector3(index.y * posY, 0f, index.x * posX), Quaternion.identity);
        if (isTest)
            node.prefab.GetComponent<MeshRenderer>().material = material;
        node.prefab.transform.SetParent(gameObject.transform);
        node.prefab.AddComponent<LineRenderer>();
    }

    private void PaintLink()
    {
        if (maps == null)
            return;
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null || maps[j, i].Children.Count == 0)
                    continue;
                Debug.DrawLine(maps[j, i].prefab.transform.position, maps[j, i].Children[0].prefab.transform.position, Color.white);
                if (maps[j, i].Children.Count >= 2)
                    Debug.DrawLine(maps[j, i].prefab.transform.position, maps[j, i].Children[1].prefab.transform.position, Color.red);
                if (maps[j, i].Children.Count >= 3)
                    Debug.DrawLine(maps[j, i].prefab.transform.position, maps[j, i].Children[2].prefab.transform.position, Color.blue);
            }
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Debug.DrawLine(edges[i].start + new Vector3(0f, 0f, 15f), edges[i].end + new Vector3(0f, 0f, 15f), Color.blue);
        }
    }
    private void ChildrenDestroy()
    {
        Transform[] childList = GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
        edges.Clear();
        isTest = true;
    }
}
