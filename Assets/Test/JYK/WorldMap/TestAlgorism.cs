using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestAlgorism : MonoBehaviour
{
    private struct Edge
    {
        public Vector2 start;
        public Vector2 end;

        public Edge(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }
        public float Equation(float x)
        {
            return (x - start.x) * (end.y - start.y) / (end.x - start.x) + start.y;
        }
        public bool IsPointOnEquation(Vector2 point)
        {
            if (point.y > start.y && point.y < end.y)
                return Mathf.Approximately((point.x - start.x) * (end.y - start.y) / (end.x - start.x) + start.y, point.y);
            else
                return false;
        }
    }

    public int row = 5;
    public int col = 8;
    private StageNode startNode;
    private StageNode[,] stageArr;
    private static int[,] baseArr;
    private List<Edge> allEdge = new List<Edge>();
    private bool isGenerateEnd;
    private static bool isResetArr = true;

    private int StartRow { get => row / 2; }

    private void Start()
    {
        StartCoroutine(CoGenerateWorldMap());
    }

    private IEnumerator CoGenerateWorldMap()
    {
        bool isDone = false;
        while (!isDone)
        {
            Debug.Log("asd");
            isDone = true;

            var old = parent.GetComponentsInChildren<StageNode>();
            foreach (var go in old)
                if(go != rootNode && go != lastNode)
                    Destroy(go.gameObject);

            rootNode.downVertice.Clear();
            lastNode.upVertice.Clear();

            //랜덤생성. 배열의 배열로 교체예정.
            if(isResetArr)
            {
                baseArr = new int[row, col];
                CreateBaseArr();
                //{
                //{
                //    { 1, 1, 0, 0, 1, 0, 1, 1 },
                //    { 0, 0, 1, 0, 0, 1, 0, 0 },
                //    { 0, 1, 0, 1, 1, 0, 1, 0 },
                //    { 1, 0, 0, 0, 0, 0, 0, 0 },
                //    { 0, 0, 1, 0, 0, 1, 0, 1 },
                //};
            }


            //컴포넌트 부착 게임오브젝트 생성 - 나중에 프리펩으로 대체. 불필요해질 과정.
            stageArr = new StageNode[row, col];
            var offset = new Vector3(100, 100, 0);
            for (int i = 0; i < baseArr.GetLength(0); i++)
            {
                for (int j = 0; j < baseArr.GetLength(1); j++)
                {
                    if (baseArr[i, j] == 1)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = offset + new Vector3(j * 10, i * -5, 0);
                        cube.transform.SetParent(parent);
                        var stageNode = cube.AddComponent<StageNode>();
                        stageNode.Index = new Vector2(i, j);
                        stageArr[i, j] = stageNode;
                    }
                }
            }

            //1회 순회. 정점 연결.
            allEdge.Clear();
            List<StageNode> candidate = new List<StageNode>();
            for (int j = 0; j < baseArr.GetLength(1); j++)
            {
                for (int i = 0; i < baseArr.GetLength(0); i++)
                {
                    if (stageArr[i, j] == null) continue;

                    if (j == 0)
                    {
                        rootNode.downVertice.Add(stageArr[i, j]);
                        stageArr[i, j].upVertice.Add(rootNode);
                    }
                    else
                    {
                        candidate.Clear();
                        //부모후보찾기.
                        FindParents(candidate, stageArr[i, j].index);

                        if (candidate.Count == 0)
                        {
                            Debug.LogWarning($"No Parents for {stageArr[i, j].index}");
                            isDone = false;
                        }
                        else
                        {
                            int rand = Random.Range(0, candidate.Count);
                            candidate[rand].downVertice.Add(stageArr[i, j]);
                            stageArr[i, j].upVertice.Add(candidate[rand]);
                            allEdge.Add(new Edge(candidate[rand].index, stageArr[i, j].index));
                        }

                    }

                    if (j == baseArr.GetLength(1) - 1)
                    {
                        stageArr[i, j].downVertice.Add(lastNode);
                        lastNode.upVertice.Add(stageArr[i, j]);
                    }
                }
            }

            if (!isDone)
                continue;

            //2회순회 확률기반 추가 정점 연결.
            for (int j = 0; j < baseArr.GetLength(1); j++)
            {
                for (int i = 0; i < baseArr.GetLength(0); i++)
                {
                    if (stageArr[i, j] == null) continue;
                    if (j != 0)
                    {
                        candidate.Clear();
                        //부모후보찾기.
                        FindParents(candidate, stageArr[i, j].index);

                        if (candidate.Count != 0)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                bool isCreateNewNode = false;
                                if (stageArr[i, j].upVertice.Count == 1 && Random.Range(0f, 1f) < 0.3f) //30%
                                {
                                    isCreateNewNode = true;
                                }
                                else if (stageArr[i, j].upVertice.Count == 2 && Random.Range(0f, 1f) < 0.15f) //15%
                                {
                                    isCreateNewNode = true;
                                }

                                if (isCreateNewNode)
                                {
                                    int rand = Random.Range(0, candidate.Count);
                                    candidate[rand].downVertice.Add(stageArr[i, j]);
                                    stageArr[i, j].upVertice.Add(candidate[rand]);
                                    allEdge.Add(new Edge(candidate[rand].index, stageArr[i, j].index));
                                }
                            }
                        }
                    }
                }
            }

            //3회순회 자식 없는 노드 연결.
            for (int j = 0; j < baseArr.GetLength(1); j++)
            {
                for (int i = 0; i < baseArr.GetLength(0); i++)
                {
                    if (stageArr[i, j] == null) continue;
                    if (stageArr[i, j].downVertice.Count == 0)
                    {
                        candidate.Clear();
                        //부모후보찾기.
                        FindChildren(candidate, stageArr[i, j].index);

                        if (candidate.Count != 0)
                        {
                            int rand = Random.Range(0, candidate.Count);
                            candidate[rand].upVertice.Add(stageArr[i, j]);
                            stageArr[i, j].downVertice.Add(candidate[rand]);
                            allEdge.Add(new Edge(candidate[rand].index, stageArr[i, j].index));
                        }
                        else
                        {
                            Debug.LogWarning("Final Step Error! There is no Child Node to connect.");
                            isDone = false;
                            break;
                        }
                    }
                }
                if (!isDone)
                    break;
            }

            yield return null;
        }
        isGenerateEnd = true;
    }

    private void Awake()
    {
        //Debug.Log(IsCrossEdge(new Edge(new Vector2(1, 5), new Vector2(2, 5)),
        //    new Edge(new Vector2(1, 10), new Vector2(2, 5))));
        //Debug.Log(IsCrossEdge(new Edge(new Vector2(1, 5), new Vector2(2, 5)),
        //    new Edge(new Vector2(1, 10), new Vector2(2, 4.9f))));
        //Debug.Log(IsCrossEdge(new Edge(new Vector2(0, 1), new Vector2(4, 2)),
        //    new Edge(new Vector2(3, 0), new Vector2(2, 4))));
        //Debug.Log(IsCrossEdge(new Edge(new Vector2(0, 1), new Vector2(4, 2)),
        //    new Edge(new Vector2(2, 1), new Vector2(2, 3))));

        //Debug.Log(IsCrossEdge(new Edge(new Vector2(0, 1), new Vector2(0, 4)),
        //    new Edge(new Vector2(0, 4), new Vector2(0, 6)))); // False
        //Debug.Log(IsCrossEdge(new Edge(new Vector2(2, 1), new Vector2(0, 4)),
        //    new Edge(new Vector2(0, 4), new Vector2(0, 6)))); // False
        //Debug.Log(IsCrossEdge(new Edge(new Vector2(2, 1), new Vector2(0, 4)),
        //    new Edge(new Vector2(2, 1), new Vector2(0, 4)))); // True

        //Debug.Log(new Edge(new Vector2(0, 1), new Vector2(2, 5)).IsPointOnEquation(new Vector2(0, 1))); // False
        //Debug.Log(new Edge(new Vector2(0, 1), new Vector2(2, 5)).IsPointOnEquation(new Vector2(2, 5))); // False
        //Debug.Log(new Edge(new Vector2(0, 1), new Vector2(2, 5)).IsPointOnEquation(new Vector2(1, 3))); // True
        //Debug.Log(new Edge(new Vector2(2, 4), new Vector2(0, 6)).IsPointOnEquation(new Vector2(1, 5))); // True
    }

    private void CreateBaseArr()
    {
        for (int j = 0; j < baseArr.GetLength(1); j++)
        {
            var prof = Random.Range(0f, 1f);
            var nodeNum = prof < 0.3f ? 1 : (prof < 0.9f ? 2 : 3);

            for (int k = 0; k < nodeNum; k++)
            {
                var rand = Random.Range(0, row - k);
                int count = 0;
                for (int i = 0; i < baseArr.GetLength(0); i++)
                {
                    if (baseArr[i, j] == 0)
                    {
                        if (count == rand)
                        {
                            baseArr[i, j] = 1;
                            break;
                        }
                        else
                            count++;
                    }
                }
            }
        }
    }
    private bool IsEdgeThroughAnyNode(Edge edge)
    {
        for (int i = 0; i < stageArr.GetLength(0); i++)
        {
            for (int j = 0; j < stageArr.GetLength(1); j++)
            {
                if (stageArr[i, j] == null) continue;
                if (edge.IsPointOnEquation(stageArr[i, j].index))
                    return true;
            }
        }
        return false;
    }

    private void FindParents(List<StageNode> list, Vector2 nodeIndex)
    {
        for (int i = 0; i < stageArr.GetLength(0); i++)
        {
            int limit = (int)nodeIndex.y - 3;
            limit = limit < 0 ? 0 : limit;

            for (int j = (int)nodeIndex.y - 1; j >= limit; j--)
            {
                if (stageArr[i, j] == null) continue;

                Edge thisEdge = new Edge(stageArr[i, j].index, nodeIndex);
                bool isCross = false;
                for (int k = 0; k < allEdge.Count; k++)
                {
                    if (IsCrossEdge(allEdge[k], thisEdge))
                    {
                        isCross = true;
                        break;
                    }
                }

                if(!isCross && stageArr[i, j].downVertice.Count < 3 && !IsEdgeThroughAnyNode(thisEdge))
                {
                    list.Add(stageArr[i, j]);
                    break;
                }

            }
        }
    }

    private void FindChildren(List<StageNode> list, Vector2 nodeIndex)
    {
        for (int i = 0; i < stageArr.GetLength(0); i++)
        {
            int limit = (int)nodeIndex.y + 3;
            limit = limit > stageArr.GetLength(1) - 1 ? stageArr.GetLength(1) - 1 : limit;

            for (int j = (int)nodeIndex.y + 1; j <= limit; j++)
            {
                if (stageArr[i, j] == null) continue;

                Edge thisEdge = new Edge(nodeIndex, stageArr[i, j].index);
                bool isCross = false;
                for (int k = 0; k < allEdge.Count; k++)
                {
                    if (IsCrossEdge(allEdge[k], thisEdge))
                    {
                        isCross = true;
                        break;
                    }
                }

                if (!isCross && !IsEdgeThroughAnyNode(thisEdge))
                {
                    list.Add(stageArr[i, j]);
                    break;
                }
            }
        }
    }

    private bool IsCrossEdge(Edge e1, Edge e2)
    {
        Vector2 newPoint_e1_start = new Vector2(e1.start.y, e1.start.x);
        Vector2 newPoint_e1_end = new Vector2(e1.end.y, e1.end.x);
        e1 = new Edge(newPoint_e1_start, newPoint_e1_end);

        Vector2 newPoint_e2_start = new Vector2(e2.start.y, e2.start.x);
        Vector2 newPoint_e2_end = new Vector2(e2.end.y, e2.end.x);
        e2 = new Edge(newPoint_e2_start, newPoint_e2_end);

        float start;
        float end;
        if (e1.start.x != e2.start.x || e1.end.x != e2.end.x)
        {
            start = (e1.start.x > e2.start.x) ? e1.start.x : e2.start.x;
            end = (e1.end.x < e2.end.x) ? e1.end.x : e2.end.x;
        }
        else
        {
            start = e1.start.x;
            end = e1.end.x;
        }

        if (start >= end)
            return false;

        Edge edge1 = new Edge(new Vector2(start, e1.Equation(start)), new Vector2(end, e1.Equation(end)));
        Edge edge2 = new Edge(new Vector2(start, e2.Equation(start)), new Vector2(end, e2.Equation(end)));

        if (edge1.start == edge2.start && edge1.end == edge2.end)
            return true;

        bool isCross = (edge1.start.y - edge2.start.y) * (edge1.end.y - edge2.end.y) < 0;
        return isCross;
    }

    public Transform parent;
    public StageNode rootNode;
    public StageNode lastNode;
    private void Update()
    {
        if (!isGenerateEnd)
            return;

        for (int i = 0; i < rootNode.downVertice.Count; i++)
        {
            Debug.DrawLine(rootNode.transform.position, rootNode.downVertice[i].transform.position);
        }


        for (int j = 0; j < stageArr.GetLength(1); j++)
        {
            for (int i = 0; i < stageArr.GetLength(0); i++)
            {
                if (stageArr[i, j] == null) continue;

                for (int k = 0; k < stageArr[i, j].downVertice.Count; k++)
                {
                    Debug.DrawLine(stageArr[i, j].transform.position, stageArr[i, j].downVertice[k].transform.position);
                }
            }

        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            isResetArr = true;
            SceneManager.LoadScene(0);
        }
        if (GUILayout.Button("ReConnect"))
        {
            isResetArr = false;
            SceneManager.LoadScene(0);
        }
        //if (GUILayout.Button("Test"))
        //{
        //    //Debug.Log(IsEdgeThroughAnyNode(new Edge(new Vector2(2, 4), new Vector2(0, 6)))); // True
        //}
    }
}
