using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
}
public class WorldMap : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public GameObject fogPrefab;
    public Material material;

    public int column;
    public int row;
    public int beforeDate;

    private readonly float posX = 5f;
    private readonly float posY = 15f;

    public WorldMapNode[,] maps;
    private readonly List<Edge> edges = new List<Edge>();
    private bool isAllLinked = false;


    public void Init(int column, int row, GameObject nodePrefab, GameObject linePrefab, GameObject fogPrefab)
    {
        this.column = column;
        this.row = row;
        this.nodePrefab = nodePrefab;
        this.linePrefab = linePrefab;
        this.fogPrefab = fogPrefab;
    }

    public void LoadWorldMap(List<MapNodeStruct_0> loadData)
    {
        Load(loadData);
        PaintLink();
        Fog(3);
    }

    public void InitWorldMiniMap()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapPlayerData);
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapNode);
        var loadData = Vars.UserData.WorldMapNodeStruct;
        var layerName = "WorldMap";
        Load(loadData, layerName);
        PaintLink(layerName);
        Fog(Vars.UserData.date);
    }

    public IEnumerator InitMap(UnityAction action)
    {
        while (!isAllLinked)
        {
            MapCreateNode();
            MapRandomLink();

            yield return null;
        }
        
        action.Invoke();
        PaintLink();
        Save();
    }

    private void MapCreateNode()
    {
        maps = new WorldMapNode[row, column];

        var startEnd = Random.Range(0, row);
        
        InitNode(out maps[startEnd, 0], new Vector2(startEnd, 0));

        for (int i = 1; i < column - 1; i++)
        {
            var selectNode = Random.Range(0, row);
            InitNode(out maps[selectNode, i], new Vector2(selectNode, i));
        }
        InitNode(out maps[startEnd, column - 1], new Vector2(startEnd, column - 1));

        var startNextOther = Random.Range(0, row);
        while (maps[startNextOther, 1] != null)
        {
            startNextOther = Random.Range(0, row);
        }
        InitNode(out maps[startNextOther, 1], new Vector2(startNextOther, 1));

        for (int i = 2; i < column - 2; i++) // 가운데 맵
        {
            for (int j = 0; j < Random.Range(0, 3); j++)
            {
                var rnd = Random.Range(0, row);
                while (maps[rnd, i] != null)
                {
                    rnd = Random.Range(0, row);
                }
                InitNode(out maps[rnd, i], new Vector2(rnd, i));
            }
        }

        var endBeforeOther = Random.Range(0, row);
        while (maps[endBeforeOther, column - 2] != null)
        {
            endBeforeOther = Random.Range(0, row);
        }
        InitNode(out maps[endBeforeOther, column - 2], new Vector2(endBeforeOther, column - 2));
    }
    
    private void MapRandomLink()
    {
        var list = new List<WorldMapNode>();
        
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                maps[j, i].level = i;

                var rndRange = Random.Range(0f, 1f) > 0.1f ?       // 랜덤 거리
                    (Random.Range(0f, 1f) > 0.8f ? 2 : 3) : 1;
                var rndLine = Random.Range(0f, 1f) > 0.3f ?        // 랜덤 연결 선
                    (Random.Range(0f, 1f) > 0.3f ? 2 : 3) : 1;

                if (i == 0 && rndLine < 2)
                    rndLine = Random.Range(0f, 1f) >= 0.5f ? 2 : 3;
                
                list.Clear();
                FindNode(list, i, i + rndRange);
                Utility.Shuffle(list);
                for (int k = 0; k < list.Count; k++)
                {
                    if (maps[j, i].Children.Count >= rndLine)
                        break;
                    var isEquals = false;
                    for (int h = 0; h < maps[j, i].Children.Count; h++)
                    {
                        isEquals = maps[j, i].Children[h].Equals(list[k]);
                        if (isEquals)
                            break;
                    }
                    if (isEquals)
                        continue;
                    var pos1 = maps[j, i].transform.position;
                    var pos2 = list[k].transform.position;
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
                        var edge = new Edge(maps[j, i].transform.position, list[k].transform.position);
                        edges.Add(edge);
                        maps[j, i].Children.Add(list[k]);
                        list[k].Parent.Add(maps[j, i]);
                        list[k].difficulty = (Difficulty)(list[k].index.y - maps[j, i].index.y);
                        ColorChange(list[k]);
                    }
                }
            }
        }

        isAllLinked = ChackAllLinked();
        if (!isAllLinked)
        {
            Utility.ChildrenDestroy(gameObject);
            edges.Clear();
            isAllLinked = false;
        }
    }
    private void PaintLink()
    {
        var lines = new GameObject("Lines");
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null )
                    continue;
                var lineGo = Instantiate(linePrefab, lines.transform);
                var lineRender = lineGo.GetComponent<LineRenderer>();
                lineRender.startWidth = lineRender.endWidth = 0.2f;
                lineRender.material.color = Color.white;
                lineRender.SetPosition(0, maps[j, i].transform.position);
                lineRender.SetPosition(1, maps[j, i].Children[0].transform.position);

                if (maps[j, i].Children.Count >= 2)
                {
                    var lineGoSecond = Instantiate(linePrefab, lines.transform);
                    var lineRenderSecond = lineGoSecond.GetComponent<LineRenderer>();
                    lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.2f;
                    lineRenderSecond.material.color = Color.white;
                    lineRenderSecond.SetPosition(0, maps[j, i].transform.position);
                    lineRenderSecond.SetPosition(1, maps[j, i].Children[1].transform.position);
                }
                if (maps[j, i].Children.Count >= 3)
                {
                    var lineGoThird = Instantiate(linePrefab, lines.transform);
                    var lineRenderThird = lineGoThird.GetComponent<LineRenderer>();
                    lineRenderThird.startWidth = lineRenderThird.endWidth = 0.2f;
                    lineRenderThird.material.color = Color.white;
                    lineRenderThird.SetPosition(0, maps[j, i].transform.position);
                    lineRenderThird.SetPosition(1, maps[j, i].Children[2].transform.position);
                }
            }
        }
    }

    public void PaintLink(string LayerName)
    {
        var lines = new GameObject();
        lines.transform.SetParent(transform);
        lines.layer = LayerMask.NameToLayer(LayerName);
        var curIndex = Vars.UserData.WorldMapPlayerData.currentIndex;
        var goalIndex = Vars.UserData.WorldMapPlayerData.goalIndex;
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;

                var lineGo = Instantiate(linePrefab, lines.transform);
                lineGo.layer = LayerMask.NameToLayer(LayerName);
                var lineRender = lineGo.GetComponent<LineRenderer>();
                if (maps[j, i].index.Equals(curIndex) && maps[j, i].Children[0].index.Equals(goalIndex))
                {
                    lineRender.startWidth = lineRender.endWidth = 0.5f;
                    lineRender.material = material;
                }
                else
                {
                    lineRender.startWidth = lineRender.endWidth = 0.2f;
                    lineRender.material.color = Color.white;
                }
                lineRender.SetPosition(0, maps[j, i].transform.position);
                lineRender.SetPosition(1, maps[j, i].Children[0].transform.position);

                if (maps[j, i].Children.Count >= 2)
                {
                    var lineGoSecond = Instantiate(linePrefab, lines.transform);
                    lineGoSecond.layer = LayerMask.NameToLayer(LayerName);
                    var lineRenderSecond = lineGoSecond.GetComponent<LineRenderer>();
                    if (maps[j, i].index.Equals(curIndex) && maps[j, i].Children[1].index.Equals(goalIndex))
                    {
                        lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.5f;
                        lineRenderSecond.material = material;
                    }
                    else
                    {
                        lineRenderSecond.material.color = Color.white;
                        lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.2f;
                    }
                    lineRenderSecond.SetPosition(0, maps[j, i].transform.position);
                    lineRenderSecond.SetPosition(1, maps[j, i].Children[1].transform.position);
                }
                if (maps[j, i].Children.Count >= 3)
                {
                    var lineGoThird = Instantiate(linePrefab, lines.transform);
                    lineGoThird.layer = LayerMask.NameToLayer(LayerName);
                    var lineRenderThird = lineGoThird.GetComponent<LineRenderer>();
                    if (maps[j, i].index.Equals(curIndex) && maps[j, i].Children[2].index.Equals(goalIndex))
                    {
                        lineRenderThird.startWidth = lineRenderThird.endWidth = 0.5f;
                        lineRenderThird.material = material;
                    }
                    else
                    {
                        lineRenderThird.startWidth = lineRenderThird.endWidth = 0.2f;
                        lineRenderThird.material.color = Color.white;
                    }
                    lineRenderThird.SetPosition(0, maps[j, i].transform.position);
                    lineRenderThird.SetPosition(1, maps[j, i].Children[2].transform.position);
                }
            }
        }
    }
    public void Fog(int date)
    {
        for (int i = 3; i <= date; i++)
        {
            if (i.Equals(3))
            {
                fogPrefab = Instantiate(fogPrefab);
                fogPrefab.layer = LayerMask.NameToLayer("WorldMap");
                var posX = (fogPrefab.transform.localScale.x * 10f) - (posY / 2);
                fogPrefab.transform.position = transform.GetChild(0).position - new Vector3(posX, 0f, 0f);
            }
            else
            {
                fogPrefab.transform.position += new Vector3(posY, 0f, 0f);
            }
        }
        beforeDate = date;
    }
    public void FogComing()
    {
        var date = Vars.UserData.date;
        if (date == 3)
            Fog(date);

        for (int i = beforeDate; i < date; i++)
        {
            fogPrefab.transform.position += new Vector3(posY, 0f, 0f);
        }
        beforeDate = Vars.UserData.date;
    }

    private void InitNode(out WorldMapNode node, Vector2 index)
    {
        var go = Instantiate(nodePrefab, new Vector3(index.y * posY, 0f, index.x * posX), Quaternion.identity);
        go.transform.SetParent(gameObject.transform);
        node = go.AddComponent<WorldMapNode>();
        node.index = index;
    }

    private void InitNode(out WorldMapNode node, Vector2 index, string LayerName)
    {
        var go = Instantiate(nodePrefab, new Vector3(index.y * posY - 200f, 0f, index.x * posX + 200f), Quaternion.identity);
        go.layer = LayerMask.NameToLayer(LayerName);
        go.transform.SetParent(gameObject.transform);
        node = go.AddComponent<WorldMapNode>();
        node.index = index;
    }

    private void FindNode(List<WorldMapNode> nodeList, int startIndex, int endIndex)
    {
        var end = endIndex + 1 > column  ? column : endIndex + 1;
        for (int j = 0; j < row; j++)
        {
            for (int i = startIndex + 1; i < end; i++)
            {
                if (maps[j, i] == null)
                    continue;
                nodeList.Add(maps[j, i]);
                break;
            }
        }
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
                var dot = maps[j, i].transform.position;
                if (IsCrossingLineToDot(pos1, pos2, dot))
                    return true;
            }
        }
        return false;
    }
    private bool ChackAllLinked()
    {
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                if ((i < column - 1 && maps[j, i].Children.Count == 0) ||
                    (i > 0 && maps[j, i].Parent.Count == 0))
                {
                    Debug.Log("연결 안되어 있음");
                    return false;
                }
            }
        }
        return true;
    }
    public bool IsCrossingLineToDot(Vector3 start, Vector3 end, Vector3 dot)
    {
        if (start.x < dot.x && dot.x < end.x)
            return Mathf.Approximately((dot.x - start.x) * (end.z - start.z) / (end.x - start.x) + start.z, dot.z);

        return false;
    }
    public void Save()
    {
        Vars.UserData.WorldMapNodeStruct.Clear();
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                var data = new MapNodeStruct_0();
                data.index = maps[j, i].index;
                data.level = maps[j, i].level;
                data.difficulty = maps[j, i].difficulty;
                data.children = new List<Vector2>();
                data.parent = new List<Vector2>();
                for (int k = 0; k < maps[j, i].Children.Count; k++)
                {
                    data.children.Add(maps[j, i].Children[k].index);
                }
                for (int k = 0; k < maps[j, i].Parent.Count; k++)
                {
                    data.parent.Add(maps[j, i].Parent[k].index);
                }
                Vars.UserData.WorldMapNodeStruct.Add(data);
            }
        }
        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapNode);
    }
    private void Load(List<MapNodeStruct_0> loadData, string LayerName = "null")
    {
        maps = new WorldMapNode[row, column];
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var index = new Vector2(j, i);
                for (int k = 0; k < loadData.Count; k++)
                {
                    if (loadData[k].index.Equals(index))
                    {
                        if (LayerName.Equals("null"))
                        {
                            InitNode(out maps[j, i], index);
                        }
                        else
                        {
                            InitNode(out maps[j, i], index, LayerName);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var index = new Vector2(j, i);
                for (int k = 0; k < loadData.Count; k++)
                {
                    if (loadData[k].index.Equals(index))
                    {
                        maps[j, i].difficulty = loadData[k].difficulty;
                        ColorChange(maps[j, i]);
                        var children = loadData[k].children;
                        var parent = loadData[k].parent;
                        for (int h = 0; h < children.Count; h++)
                        {
                            maps[j, i].Children.Add(maps[(int)children[h].x, (int)children[h].y]);
                        }
                        for (int h = 0; h < parent.Count; h++)
                        {
                            maps[j, i].Parent.Add(maps[(int)parent[h].x, (int)parent[h].y]);
                        }
                    }
                }
            }
        }

    }
    private void ColorChange(WorldMapNode node)
    {
        switch (node.difficulty)
        {
            case Difficulty.easy:
                break;
            case Difficulty.normal:
                node.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case Difficulty.hard:
                node.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                break;
        }
    }
}

