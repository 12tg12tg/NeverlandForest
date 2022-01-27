using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public struct Edge
{
    public Vector3 start;
    public Vector3 end;
    public Vector3 DirVec => end - start;
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

    public static bool IsCrossingLineToDot(Vector3 start, Vector3 end, Vector3 dot)
    {
        if (start.x < dot.x && dot.x < end.x)
            return Mathf.Approximately((dot.x - start.x) * (end.z - start.z) / (end.x - start.x) + start.z, dot.z);

        return false;
    }

    public bool DistanceCheak(Vector3 pos, float dis)
    {
        // 사이각 구하기
        var dirVec2 = pos - start;
        var angle = Vector3.Angle(DirVec, dirVec2);
        
        // 빗변
        var hypotenuse = Vector3.Distance(start, pos);

        // 밑변
        var dotBase = hypotenuse * Mathf.Cos(angle * Mathf.Deg2Rad);

        // 원하는 지점
        var vec = start + (DirVec.normalized * dotBase);
        
        if (vec.x > start.x && vec.x < end.x)
        {
            // 점과 선의 거리
            var distance = Vector3.Distance(pos, vec);
            return distance > dis;
        }
        return true;
    }
}
public class WorldMap : MonoBehaviour
{
    // 미니맵을 사용하려 할 때 필요해서 열어놔야함
    [Header("프리팹")]
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public GameObject fogPrefab;
    
    [Header("행렬")]
    public int column;
    public int row;

    [Header("미니월드맵에서 쓰는 것들")]
    public Material material;
    public GameObject miniMapLand;

    // 노드의 모든 정보를 갖고있는 변수
    private WorldMapNode[][] maps;
    public WorldMapNode[][] Maps => maps;

    // 간선체크 용도
    private readonly List<Edge> edges = new List<Edge>();
    public List<Edge> Edges => edges;

    // 노드의 간격
    private readonly float posX = 15f;
    private readonly float posZ = 10f;

    // 모든 노드가 부모자식이 연결 됐는지
    private bool isAllLinked = false;

    // 안개에서 쓰는 변수
    private static int beforeDate;
    private readonly int witchFollowDate = 3;

    public void Init(int column, int row, GameObject nodePrefab, GameObject linePrefab, GameObject fogPrefab)
    {
        this.column = column;
        this.row = row;
        this.nodePrefab = nodePrefab;
        this.linePrefab = linePrefab;
        this.fogPrefab = fogPrefab;
    }
    public void LoadWorldMap(List<WorldMapNodeStruct> loadData)
    {
        Load(loadData);
        PaintLink();
        FogInit(Vars.UserData.uData.Date);
    }
    public void InitWorldMiniMap()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapPlayerData);
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapData);
        var loadData = Vars.UserData.WorldMapNodeStruct;
        var layerName = "WorldMap";
        var date = Vars.UserData.uData.Date;
        Load(loadData, layerName);
        PaintLink(layerName);
        FogInit(date);
        FogMove(date, true);
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
        maps = new WorldMapNode[column][];
        for (int i = 0; i < column; i++)
        {
            maps[i] = new WorldMapNode[row];
        }

        var startEnd = Random.Range(0, row);

        InitNode(out maps[0][startEnd], new Vector2(startEnd, 0));

        for (int i = 1; i < column - 1; i++)
        {
            var selectNode = Random.Range(0, row);
            InitNode(out maps[i][selectNode], new Vector2(selectNode, i));
        }
        InitNode(out maps[column - 1][startEnd], new Vector2(startEnd, column - 1));

        var startNextOther = Random.Range(0, row);
        while (maps[1][startNextOther] != null)
        {
            startNextOther = Random.Range(0, row);
        }
        InitNode(out maps[1][startNextOther], new Vector2(startNextOther, 1));

        for (int i = 2; i < column - 2; i++) // 가운데 맵
        {
            for (int j = 0; j < Random.Range(0, 3); j++)
            {
                var rnd = Random.Range(0, row);
                while (maps[i][rnd] != null)
                {
                    rnd = Random.Range(0, row);
                }
                InitNode(out maps[i][rnd], new Vector2(rnd, i));
            }
        }

        var endBeforeOther = Random.Range(0, row);
        while (maps[column - 2][endBeforeOther] != null)
        {
            endBeforeOther = Random.Range(0, row);
        }
        InitNode(out maps[column - 2][endBeforeOther], new Vector2(endBeforeOther, column - 2));
    }

    private void MapRandomLink()
    {
        var list = new List<WorldMapNode>();

        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[i][j] == null)
                    continue;
                maps[i][j].level = i;

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
                    if (maps[i][j].Children.Count >= rndLine)
                        break;
                    var isEquals = false;
                    for (int h = 0; h < maps[i][j].Children.Count; h++)
                    {
                        isEquals = maps[i][j].Children[h].Equals(list[k]);
                        if (isEquals)
                            break;
                    }
                    if (isEquals)
                        continue;
                    var pos1 = maps[i][j].transform.position;
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
                        var edge = new Edge(maps[i][j].transform.position, list[k].transform.position);
                        edges.Add(edge);
                        maps[i][j].Children.Add(list[k]);
                        list[k].Parent.Add(maps[i][j]);
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



    // PaintLink 메서드 둘의 차이는 미니 월드맵에서 사용되어 layer 부여하여 카메라가 지정된 레이어만 보이도록 하기 위함과
    // 라인을 그릴때 노드에서 부터 그리느냐, 노드에서 일정 범위가 떨어져 있는 곳부터 그리느냐의 차이
    private void PaintLink()
    {
        var lines = new GameObject("Lines");
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[i][j] == null)
                    continue;
                var lineGo = Instantiate(linePrefab, lines.transform);
                var lineRender = lineGo.GetComponent<LineRenderer>();
                lineRender.startWidth = lineRender.endWidth = 0.2f;
                var startPos = maps[i][j].transform.position;
                var endPos = maps[i][j].Children[0].transform.position;
                var dis = 2f / Vector3.Distance(startPos, endPos);
                var newStartPos = Vector3.Lerp(startPos, endPos, dis);
                var newEndPos = Vector3.Lerp(startPos, endPos, 1 - dis);
                lineRender.SetPosition(0, newStartPos);
                lineRender.SetPosition(1, newEndPos);

                if (maps[i][j].Children.Count >= 2)
                {
                    var lineGoSecond = Instantiate(linePrefab, lines.transform);
                    var lineRenderSecond = lineGoSecond.GetComponent<LineRenderer>();
                    lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.2f;
                    endPos = maps[i][j].Children[1].transform.position;
                    dis = 2f / Vector3.Distance(startPos, endPos);
                    newStartPos = Vector3.Lerp(startPos, endPos, dis);
                    newEndPos = Vector3.Lerp(startPos, endPos, 1 - dis);
                    lineRenderSecond.SetPosition(0, newStartPos);
                    lineRenderSecond.SetPosition(1, newEndPos);
                }
                if (maps[i][j].Children.Count >= 3)
                {
                    var lineGoThird = Instantiate(linePrefab, lines.transform);
                    var lineRenderThird = lineGoThird.GetComponent<LineRenderer>();
                    lineRenderThird.startWidth = lineRenderThird.endWidth = 0.2f;
                    endPos = maps[i][j].Children[2].transform.position;
                    dis = 2f / Vector3.Distance(startPos, endPos);
                    newStartPos = Vector3.Lerp(startPos, endPos, dis);
                    newEndPos = Vector3.Lerp(startPos, endPos, 1 - dis);
                    lineRenderThird.SetPosition(0, newStartPos);
                    lineRenderThird.SetPosition(1, newEndPos);
                }
            }
        }
    }
    private void PaintLink(string LayerName)
    {
        var lines = new GameObject("Lines");
        lines.transform.SetParent(transform);
        lines.layer = LayerMask.NameToLayer(LayerName);
        var curIndex = Vars.UserData.WorldMapPlayerData.currentIndex;
        var goalIndex = Vars.UserData.WorldMapPlayerData.goalIndex;
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[i][j] == null)
                    continue;

                var lineGo = Instantiate(linePrefab, lines.transform);
                lineGo.layer = LayerMask.NameToLayer(LayerName);
                var lineRender = lineGo.GetComponent<LineRenderer>();
                if (maps[i][j].index.Equals(curIndex) && maps[i][j].Children[0].index.Equals(goalIndex))
                {
                    lineRender.startWidth = lineRender.endWidth = 0.5f;
                    lineRender.material = material;
                }
                else
                {
                    lineRender.startWidth = lineRender.endWidth = 0.2f;
                    lineRender.material.color = Color.white;
                }
                lineRender.SetPosition(0, maps[i][j].transform.position);
                lineRender.SetPosition(1, maps[i][j].Children[0].transform.position);

                if (maps[i][j].Children.Count >= 2)
                {
                    var lineGoSecond = Instantiate(linePrefab, lines.transform);
                    lineGoSecond.layer = LayerMask.NameToLayer(LayerName);
                    var lineRenderSecond = lineGoSecond.GetComponent<LineRenderer>();
                    if (maps[i][j].index.Equals(curIndex) && maps[i][j].Children[1].index.Equals(goalIndex))
                    {
                        lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.5f;
                        lineRenderSecond.material = material;
                    }
                    else
                    {
                        lineRenderSecond.material.color = Color.white;
                        lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.2f;
                    }
                    lineRenderSecond.SetPosition(0, maps[i][j].transform.position);
                    lineRenderSecond.SetPosition(1, maps[i][j].Children[1].transform.position);
                }
                if (maps[i][j].Children.Count >= 3)
                {
                    var lineGoThird = Instantiate(linePrefab, lines.transform);
                    lineGoThird.layer = LayerMask.NameToLayer(LayerName);
                    var lineRenderThird = lineGoThird.GetComponent<LineRenderer>();
                    if (maps[i][j].index.Equals(curIndex) && maps[i][j].Children[2].index.Equals(goalIndex))
                    {
                        lineRenderThird.startWidth = lineRenderThird.endWidth = 0.5f;
                        lineRenderThird.material = material;
                    }
                    else
                    {
                        lineRenderThird.startWidth = lineRenderThird.endWidth = 0.2f;
                        lineRenderThird.material.color = Color.white;
                    }
                    lineRenderThird.SetPosition(0, maps[i][j].transform.position);
                    lineRenderThird.SetPosition(1, maps[i][j].Children[2].transform.position);
                }
            }
        }
    }

    private void FogInit(int date)
    {
        if (date > witchFollowDate - 1)
        {
            fogPrefab = Instantiate(fogPrefab);
            fogPrefab.layer = LayerMask.NameToLayer("WorldMap");
            var posX = (fogPrefab.transform.localScale.x * 10f) - (this.posX / 2);
            var addPos = miniMapLand != null ? 
                new Vector3(miniMapLand.transform.localScale.z * 10f, 0f, miniMapLand.transform.localScale.z * 10f) : 
                Vector3.zero;
            var endPos = Vector3.zero - new Vector3(posX, 0f, 0f) - addPos;
            fogPrefab.transform.position = endPos;
            NodeColorChange(beforeDate - witchFollowDate);
        }
        for (int i = witchFollowDate; i < date; i++)
        {
            fogPrefab.transform.position += new Vector3(posX, 0f, 0f);
        }
        //beforeDate = date;
    }

    public void FogMove(int date, bool isMiniMap = false, UnityAction action = null)
    {
        for (int i = witchFollowDate; i <= date; i++)
        {
            if (isMiniMap)
            {
                fogPrefab.transform.position += new Vector3(posX, 0f, 0f);
            }
            else
            {
                var endPos = new Vector3(posX * (date - beforeDate), 0f, 0f);
                var coMove = Utility.CoTranslate(fogPrefab.transform, fogPrefab.transform.position, fogPrefab.transform.position + endPos, 1f, () =>
                {
                    NodeColorChange(date - witchFollowDate);
                    action?.Invoke();
                });
                StartCoroutine(coMove);
                beforeDate = date;
                return;
            }
        }
        NodeColorChange(date - witchFollowDate);
        beforeDate = date;
    }

    public void FogCheck() // 미니맵을 켰을 때(버튼 클릭 시) 실행되는 메서드
    {
        var date = Vars.UserData.uData.Date;
        if (date == witchFollowDate)
            FogInit(date);

        for (int i = beforeDate; i < date; i++)
        {
            fogPrefab.transform.position += new Vector3(posX, 0f, 0f);
        }
        //beforeDate = date;
    }

    public void NodeColorChange(int indexY)
    {
        for (int i = 0; i <= indexY; i++)
        {
            var passNodes = maps.Select(x => x.Where(x => x != null && Mathf.Approximately(x.index.y, i))
                                          .Select(x => x)
                                          .ToArray())
                            .ToArray();

            for (int j = 0; j < passNodes.Length; j++)
            {
                for (int k = 0; k < passNodes[j].Length; k++)
                {
                    passNodes[j][k].GetComponent<MeshRenderer>().material.color = Color.black;
                    var main = passNodes[j][k].GetComponentInChildren<ParticleSystem>().main;
                    main.startColor = Color.white;
                }
            }
        }
    }

    private void InitNode(out WorldMapNode node, Vector2 index)
    {
        var go = Instantiate(nodePrefab, new Vector3(index.y * posX, 0f, index.x * posZ), Quaternion.identity);
        go.transform.SetParent(gameObject.transform);
        go.layer = LayerMask.NameToLayer("Node");
        node = go.AddComponent<WorldMapNode>();
        node.index = index;
    }
    private void InitNode(out WorldMapNode node, Vector2 index, string LayerName)
    {
        var go = Instantiate(nodePrefab, new Vector3(index.y * (posX + 10f) - 100f, 0f, index.x * posZ - 100f), Quaternion.identity);
        go.transform.SetParent(gameObject.transform);
        go.layer = LayerMask.NameToLayer(LayerName);
        node = go.AddComponent<WorldMapNode>();
        node.index = index;
    }
    private void FindNode(List<WorldMapNode> nodeList, int startIndex, int endIndex)
    {
        var end = endIndex + 1 > column ? column : endIndex + 1;
        for (int j = 0; j < row; j++)
        {
            for (int i = startIndex + 1; i < end; i++)
            {
                if (maps[i][j] == null)
                    continue;
                nodeList.Add(maps[i][j]);
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
                if (maps[i][j] == null)
                    continue;
                var dot = maps[i][j].transform.position;
                if (Edge.IsCrossingLineToDot(pos1, pos2, dot))
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
                if (maps[i][j] == null)
                    continue;
                if ((i < column - 1 && maps[i][j].Children.Count == 0) ||
                    (i > 0 && maps[i][j].Parent.Count == 0))
                {
                    Debug.Log("연결 안되어 있음");
                    return false;
                }
            }
        }
        return true;
    }
    
    public void Save()
    {
        var node = Vars.UserData.WorldMapNodeStruct;
        node.Clear();
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[i][j] == null)
                    continue;
                var data = new WorldMapNodeStruct();
                data.index = maps[i][j].index;
                data.level = maps[i][j].level;
                data.children = new List<Vector2>();
                data.parent = new List<Vector2>();
                for (int k = 0; k < maps[i][j].Children.Count; k++)
                {
                    data.children.Add(maps[i][j].Children[k].index);
                }
                for (int k = 0; k < maps[i][j].Parent.Count; k++)
                {
                    data.parent.Add(maps[i][j].Parent[k].index);
                }
                node.Add(data);
            }
        }
        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapData);
    }
    private void Load(List<WorldMapNodeStruct> loadData, string LayerName = "null")
    {
        maps = new WorldMapNode[column][];
        for (int i = 0; i < column; i++)
        {
            maps[i] = new WorldMapNode[row];
        }

        // 노드 초기화
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
                            InitNode(out maps[i][j], index);
                        }
                        else
                        {
                            InitNode(out maps[i][j], index, LayerName);
                        }

                        // 노드 레벨 연결
                        maps[i][j].level = loadData[k].level;
                    }
                }
            }
        }

        // 노드 연결
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var index = new Vector2(j, i);
                for (int k = 0; k < loadData.Count; k++)
                {
                    if (loadData[k].index.Equals(index))
                    {
                        var children = loadData[k].children;
                        var parent = loadData[k].parent;
                        for (int h = 0; h < children.Count; h++)
                        {
                            maps[i][j].Children.Add(maps[(int)children[h].y][(int)children[h].x]);
                        }
                        for (int h = 0; h < parent.Count; h++)
                        {
                            maps[i][j].Parent.Add(maps[(int)parent[h].y][(int)parent[h].x]);
                        }
                    }
                }
            }
        }
    }
}

