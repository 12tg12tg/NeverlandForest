using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TreeType
{
    // trees�� ����� ������ ��ġ�ؾ� ��
    BasicTree,
    DryTree,
    CutDryTree,
    None,
}

public class WorldMapGround : MonoBehaviour
{
    [Header("���� ������")]
    public GameObject[] trees;

    [Header("���� ���� Ȯ��")]
    public int[] percentageOfTrees;

    [Header("�ٴ�")]
    public MeshRenderer land;

    [Header("���� ���� ����")]
    public float col;
    public float row;
    public float intervalLine;
    public float intervalTree;
    public float num;

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

                    // �̹� ������ �������� �Ÿ�, ���ΰ��� �Ÿ�, ������ �Ÿ� üũ
                    if (!CheckForPosDuplicate(pos, intervalTree) ||
                        !CheckForLine(edges, pos, intervalLine) ||
                        !CheckForPosDuplicate(arrayList, pos, intervalLine))
                    {
                        // 100�� ���� �ٽ� �̾Ƶ� ������ �ѱ��
                        var check = count >= 100;
                        j = check ? j + 1 : j - 1;
                        count = check ? 0 : count + 1;
                        //Debug.Log($"{count}��° �ٽ� �̱�");
                        continue;
                    }

                    // ��ġ�� ����Ÿ�� ����
                    var rndType = GetTreeType();

                    // ���� ����
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
        // ������ ���ϰ�
        var total = 0;
        for (int i = 0; i < trees.Length; i++)
        {
            total += percentageOfTrees[i];
        }

        // ���ո�ŭ ������ ����
        var rnd = Random.Range(0, total);
        var rndType = TreeType.None;
        var percentage = 0;
        for (int i = 0; i < trees.Length; i++)
        {
            // ��ġ �������� ���
            percentage += percentageOfTrees[i];
            var percent = rnd < percentage;
            rndType = percent ? (TreeType)i : rndType;

            // ��ġ�ϸ� ��ȯ
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
