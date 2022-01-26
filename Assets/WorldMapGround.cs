using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TreeType
{
    // trees에 연결된 순서랑 일치해야 함
    BasicTree,
    DryTree,
    CutDryTree,
    None,
}

public class WorldMapGround : MonoBehaviour
{
    [Header("나무 프리팹")]
    public GameObject[] trees;

    [Header("나무 등장 확률")]
    public int[] percentageOfTrees;

    [Header("바닥")]
    public MeshRenderer land;

    [Header("가로 세로 간격")]
    public float col;
    public float row;
    public float intervalLine;
    public float intervalTree;
    public float num;

    public GameObject testCube;

    private readonly List<WorldMapTreeInfo> treeList = new List<WorldMapTreeInfo>();

    public void CreateTree(List<Edge> edges, WorldMapNode[][] nodes)
    {
        var go = new GameObject("Tree");
        go.transform.SetParent(transform);
        
        var arrayList = nodes.Select(x => x.Where(x => x != null)
                                           .Select(x => x.transform.position)
                                           .ToArray())
                             .ToList();

        var colRatio = 1f / col;
        var rowRatio = 1f / row;

        var sizeX = land.bounds.size.x;
        var sizeZ = land.bounds.size.z;

        for (int i = 0; i < row; i++)
        {
            for (int k = 0; k < col; k++)
            {
                var count = 0;

                var minX = land.bounds.min.x + k * (sizeX * colRatio);
                var maxX = minX + (sizeX * colRatio);

                var minZ = land.bounds.min.z + i * (sizeZ * rowRatio);
                var maxZ = minZ + (sizeZ * rowRatio);

                for (int j = 0; j < num; j++)
                {
                    var pos = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ, maxZ));

                    // 이미 생성된 나무와의 거리, 라인과의 거리, 노드와의 거리 체크
                    if (!CheckForPosDuplicate(pos, intervalTree) ||
                        !CheckForLine(edges, pos, intervalLine) ||
                        !CheckForPosDuplicate(arrayList, pos, intervalLine))
                    {
                        // 100번 동안 다시 뽑아도 없으면 넘기기
                        var check = count >= 100;
                        j = check ? j + 1 : j - 1;
                        count = check ? 0 : count + 1;
                        //Debug.Log($"{count}번째 다시 뽑기");
                        continue;
                    }

                    // 수치별 나무타입 랜덤
                    var rndType = GetTreeType();

                    // 나무 생성
                    var tr = Instantiate(trees[(int)rndType], pos, Quaternion.identity);
                    tr.transform.SetParent(go.transform);
                    tr.name = rndType.ToString();

                    var treeInfo = new WorldMapTreeInfo
                    {
                        treePos = tr.transform.position,
                        type = (int)rndType
                    };

                    treeList.Add(treeInfo);
                } 
            }
        }

        Save();
    }

    private TreeType GetTreeType()
    {
        // 총합을 구하고
        var total = 0;
        for (int i = 0; i < trees.Length; i++)
        {
            total += percentageOfTrees[i];
        }

        // 총합만큼 랜덤을 돌림
        var rnd = Random.Range(0, total);
        var rndType = TreeType.None;
        var percentage = 0;
        for (int i = 0; i < trees.Length; i++)
        {
            // 수치 누적으로 계산
            percentage += percentageOfTrees[i];
            var percent = rnd < percentage;
            rndType = percent ? (TreeType)i : rndType;

            // 일치하면 반환
            if (percent)
                return rndType;
        }
        return rndType;
    }
    private bool CheckForLine(List<Edge> edges, Vector3 pos, float dis)
    {
        for (int j = 0; j < edges.Count; j++)
        {
            if (!edges[j].DistanceCheak(pos, dis))
                return false;
        }
        return true;
    }
    private bool CheckForPosDuplicate(Vector3 pos, float dis)
    {
        for (int i = 0; i < treeList.Count; i++)
        {
            if (Vector3.Distance(treeList[i].treePos, pos) < dis)
                return false;
        }
        return true;
    }

    private bool CheckForPosDuplicate(List<Vector3[]> nodes, Vector3 pos, float dis)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].Length; j++)
            {
                if (Vector3.Distance(nodes[i][j], pos) < dis)
                    return false;
            }
        }
        return true;
    }

    private void Save()
    {
        var treeData = Vars.UserData.WorldMapTree;
        treeData.Clear();
        for (int i = 0; i < treeList.Count; i++)
        {
            var treeInfo = new WorldMapTreeInfo
            {
                treePos = treeList[i].treePos,
                type = treeList[i].type
            };
            treeData.Add(treeInfo);
        }
        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapData);
    }
    public void Load()
    {
        var treeData = Vars.UserData.WorldMapTree;
        var go = new GameObject("Tree");
        go.transform.SetParent(transform);
        for (int i = 0; i < treeData.Count; i++)
        {
            var type = (TreeType)treeData[i].type;
            var tr = Instantiate(trees[(int)type], treeData[i].treePos, Quaternion.identity);
            tr.transform.SetParent(go.transform);
            tr.name = type.ToString();
        }
    }
}
