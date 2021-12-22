using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        // z가 x, x가 y로 보면 됨
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

    //public bool IsCrossingLineToDot(Vector3 dot)
    //{
    //    if ((start.z < dot.z && dot.z < end.z) &&
    //        Mathf.Approximately(dot.x - start.x, ((end.x - start.x) / (end.z - start.z) * dot.z) - start.z))
    //        return true;

    //    return false;
    //}
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

    private bool isFirst = true; // 리롤용
    private bool isAllLinked = false;

    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            StartCoroutine(InitMap());
        }
    }

    private void Update()
    {
        if (isAllLinked)
        {
            PaintLink();
        }
    }

    private IEnumerator InitMap()
    {
        if (!isFirst)
            ChildrenDestroy();
        while (!isAllLinked)
        {
            MapFirstCreateNode();
            MapNextCreateNode();
            MapRandomLink();

            yield return null;
        }

        isFirst = false;
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
            for (int j = 0; j < Random.Range(0, 3); j++)
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
    private void MapRandomLink()
    {
        var list = new List<NewMapNode>();
        
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                maps[j, i].level = i;

                var rndRange = Random.Range(0f, 1f) >= 0.2f ?       // 랜덤 거리
                    (Random.Range(0f, 1f) >= 0.3f ? 2 : 3) : 1;
                var rndLine = Random.Range(0f, 1f) >= 0.2f ?        // 랜덤 연결 선
                    (Random.Range(0f, 1f) >= 0.3f ? 2 : 3) : 1;

                if (i == 0 && rndLine < 2)
                    rndLine = Random.Range(0f, 1f) >= 0.5f ? 2 : 3;
                
                list.Clear();
                FindNode(list, i, i + rndRange);

                for (int k = 0; k < list.Count; k++)
                {
                    var isEquals = false;
                    for (int h = 0; h < maps[j, i].Children.Count; h++)
                    {
                        isEquals = maps[j, i].Children[h].Equals(list[k]);
                        if (isEquals)
                            break;
                    }
                    if (isEquals)
                        continue;
                    var pos1 = maps[j, i].prefab.transform.position;
                    var pos2 = list[k].prefab.transform.position;
                    var isCrossing = false;
                    for (int g = 0; g < edges.Count; g++)
                    {
                        isCrossing = edges[g].IsCrossing(pos1, pos2);
                        if (isCrossing)
                            break;
                    }
                    var isChack = ChackDot(i, i + rndRange, pos1, pos2);

                    if (!isCrossing && !isChack)
                    {
                        if (maps[j, i].Children.Count >= rndLine)
                            continue;

                        var edge = new Edge(maps[j, i].prefab.transform.position, list[k].prefab.transform.position);
                        edges.Add(edge);
                        maps[j, i].Children.Add(list[k]);
                        list[k].Parent.Add(maps[j, i]);
                    }
                }
            }
        }

        isAllLinked = ChackAllChildren();
        if (!isAllLinked)
            ChildrenDestroy();
    }

    //private void MapLinked()
    //{
    //    for (int i = 0; i < column; i++)
    //    {
    //        for (int j = 0; j < row; j++)
    //        {
    //            if (maps[j, i] == null)
    //                continue;
    //            maps[j, i].level = i;

    //            if(i == 0) // 시작노드
    //            {
    //                for (int k = 0; k < row; k++)
    //                {
    //                    if (maps[k, i + 1] == null)
    //                        continue;
    //                    var children = maps[j, i].Children;
    //                    if (children.Count >= 3)
    //                        break;
    //                    for (int h = 0; h < children.Count; h++)
    //                    {
    //                        if (children[h].Equals(maps[k, i + 1]))
    //                            break;
    //                        var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
    //                        edges.Add(edge);
    //                        children.Add(maps[k, i + 1]);
    //                        maps[k, i + 1].Parent.Add(maps[j, i]);
    //                    }
    //                }
    //            }
    //            else if (i < column - 1) // 1~8 까지
    //            {
    //                for (int k = 0; k < row; k++)
    //                {
    //                    if (maps[k, i + 1] == null)
    //                        continue;
    //                    var children = maps[j, i].Children;
    //                    if (children.Count >= 3)
    //                        break;
    //                    if (children.Count == 0)
    //                    {
    //                        var pos1 = maps[j, i].prefab.transform.position;
    //                        var pos2 = maps[k, i + 1].prefab.transform.position;
    //                        bool isCrossing = default;
    //                        for (int g = 0; g < edges.Count; g++)
    //                        {
    //                            isCrossing = edges[g].IsCrossing(pos1, pos2);
                                
    //                            if (isCrossing)
    //                                break;
    //                        }

    //                        if (!isCrossing)
    //                        {
    //                            var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
    //                            edges.Add(edge);
    //                            children.Add(maps[k, i + 1]); 
    //                        }
    //                    }
    //                    else
    //                    {
    //                        for (int h = 0; h < children.Count; h++)
    //                        {
    //                            if (children[h].Equals(maps[k, i + 1]))
    //                                break;
    //                            var pos1 = maps[j, i].prefab.transform.position;
    //                            var pos2 = maps[k, i + 1].prefab.transform.position;
    //                            bool isCrossing = default;
    //                            for (int g = 0; g < edges.Count; g++)
    //                            {
    //                                isCrossing = edges[g].IsCrossing(pos1, pos2);
                                    
    //                                if (isCrossing)
    //                                    break;
    //                            }

    //                            if (!isCrossing)
    //                            {
    //                                var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + 1].prefab.transform.position);
    //                                edges.Add(edge);
    //                                children.Add(maps[k, i + 1]);
    //                                maps[k, i + 1].Parent.Add(maps[j, i]);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    //private void MapRndLinked()
    //{
    //    // 2-3칸 넘어에 있는 노드도 연결하는 것(이동거리)
    //    var rnd = Random.Range(0f, 1f) >= 0.2f ?
    //        (Random.Range(0f, 1f) >= 0.3f ? 2 : 3) : 1;
    //    var rndLine = Random.Range(0f, 1f) >= 0.8f ? 2 : 3;

    //    if (rnd == 1)
    //        return;

    //    for (int i = 0; i < column; i++)
    //    {
    //        for (int j = 0; j < row; j++)
    //        {
    //            if (maps[j, i] == null)
    //                continue;

    //            if (i == 0) // 시작노드
    //            {
    //                for (int k = 0; k < row; k++)
    //                {
    //                    if (maps[k, i + rnd] == null)
    //                        continue;
    //                    var children = maps[j, i].Children;

    //                    if (children.Count >= rndLine)
    //                        break;

    //                    for (int h = 0; h < children.Count; h++)
    //                    {
    //                        if (children[h].Equals(maps[k, i + rnd]))
    //                        {
    //                            break;
    //                        }

    //                        var pos1 = maps[j, i].prefab.transform.position;
    //                        var pos2 = maps[k, i + rnd].prefab.transform.position;
    //                        bool isCrossing = default;
    //                        for (int g = 0; g < edges.Count; g++)
    //                        {
    //                            isCrossing = edges[g].IsCrossing(pos1, pos2);
                                
    //                            if (isCrossing)
    //                                break;
    //                        }

    //                        var isChack = ChackDot(i + 1, i + rnd, pos1, pos2);

    //                        if (!isCrossing && isChack)
    //                        {
    //                            if (children.Count >= rndLine)
    //                                break;
    //                            Debug.Log(children.Count);

    //                            var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + rnd].prefab.transform.position);
    //                            edges.Add(edge);
    //                            children.Add(maps[k, i + rnd]);
    //                            maps[k, i + rnd].Parent.Add(maps[j, i]); 
    //                        }
    //                    }
    //                }
    //            }
    //            else if (i < column - rnd) // 1~8 까지
    //            {
    //                for (int k = 0; k < row; k++)
    //                {
    //                    if (maps[k, i + rnd] == null)
    //                        continue;
    //                    var children = maps[j, i].Children;

    //                    if (children.Count >= rndLine)
    //                        break;

    //                    if (children.Count == 0)
    //                    {
    //                        var pos1 = maps[j, i].prefab.transform.position;
    //                        var pos2 = maps[k, i + rnd].prefab.transform.position;
    //                        bool isCrossing = default;
    //                        for (int g = 0; g < edges.Count; g++)
    //                        {
    //                            isCrossing = edges[g].IsCrossing(pos1, pos2);
                                
    //                            if (isCrossing)
    //                                break;
    //                        }

    //                        var isChack = ChackDot(i + 1, i + rnd, pos1, pos2);

    //                        if (!isCrossing && isChack)
    //                        {
    //                            if (children.Count >= rndLine)
    //                                break;
    //                            Debug.Log(children.Count);

    //                            var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + rnd].prefab.transform.position);
    //                            edges.Add(edge);
    //                            children.Add(maps[k, i + rnd]);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        for (int h = 0; h < children.Count; h++)
    //                        {
    //                            if (children[h].Equals(maps[k, i + rnd]))
    //                                break;
    //                            var pos1 = maps[j, i].prefab.transform.position;
    //                            var pos2 = maps[k, i + rnd].prefab.transform.position;
    //                            bool isCrossing = default;
    //                            for (int g = 0; g < edges.Count; g++)
    //                            {
    //                                isCrossing = edges[g].IsCrossing(pos1, pos2);
                                    
    //                                if (isCrossing)
    //                                    break;
    //                            }

    //                            var isChack = ChackDot(i + 1, i + rnd, pos1, pos2);

    //                            if (!isCrossing && isChack)
    //                            {
    //                                if (children.Count >= rndLine)
    //                                    break;
    //                                Debug.Log(children.Count);
    //                                var edge = new Edge(maps[j, i].prefab.transform.position, maps[k, i + rnd].prefab.transform.position);
    //                                edges.Add(edge);
    //                                children.Add(maps[k, i + rnd]);
    //                                maps[k, i + rnd].Parent.Add(maps[j, i]);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}


    private void FindNode(List<NewMapNode> nodeList, int startIndex, int endIndex)
    {
        var end = endIndex + 1 > column  ? column : endIndex + 1;
        for (int i = startIndex + 1; i < end; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                nodeList.Add(maps[j, i]);
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
        node.prefab.GetComponent<Button>().onClick.AddListener(() => OnClick(node));
    }

    private void OnClick(NewMapNode node)
    {
        //RaycastHit hit;
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
        //    out hit, 50, clickableLayer.value))    //Ray가 닿았는가?
        //{
        //    bool door = false;                    //닿았다면, Door인가 Target인가
        //    if (hit.collider.gameObject.tag == "Doorway")
        //    {
        //        Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
        //        door = true;
        //    }
        //    else
        //    {
        //        Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
        //    }
        //    /*클릭 시 구현 부*/
        //}


        for (int i = 0; i < node.Children.Count; i++)
        {
            Debug.Log(node.Children[i].index);
        }
    }

    private void PaintLink()
    {
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

        //for (int i = 0; i < edges.Count; i++)
        //{
        //    Debug.DrawLine(edges[i].start + new Vector3(0f, 0f, 15f), edges[i].end + new Vector3(0f, 0f, 15f), Color.blue);
        //}
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
        isAllLinked = false;
    }

    private bool ChackDot(int startRange, int endRange, Vector3 pos1, Vector3 pos2)
    {
        var end = endRange > column - 1 ? column : endRange;
        for (int i = startRange + 1; i < end; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                var dot = maps[j, i].prefab.transform.position;
                if (IsCrossingLineToDot(pos1, pos2, dot))
                    return true;
            }
        }
        return false;
    }

    private bool ChackAllChildren()
    {
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                if (maps[j, i].Children.Count == 0)
                    return false;
            }
        }
        return ChackAllParent();
    }

    private bool ChackAllParent()
    {
        for (int i = 1; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                if (maps[j, i].Parent.Count == 0)
                    return false;
            }
        }
        return true;
    }


    public bool IsCrossingLineToDot(Vector3 start, Vector3 end, Vector3 dot)
    {
        var startX = start.z;
        var startY = start.x;
        var endX = end.z;
        var endY = end.x;
        var dotX = dot.z;
        var dotY = dot.x;

        if (startY < dotY && dotY < endY)
            return Mathf.Approximately((dotY - startY) * (endX - startX) / (endY - startY) + startX, dotX);

        return false;
    }
}
