using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TreeType
{
    BasicTree,
    DryTree,
    CutDryTree,
}

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

    private List<WorldMapTreeInfo> treeList = new List<WorldMapTreeInfo>();

    private void Awake()
    {
        min = land.bounds.min;
        max = land.bounds.max;

    }

    public void CreateTree(List<Edge> edges, WorldMapNode[][] nodes)
    {
        var go = new GameObject("Tree");
        go.transform.SetParent(transform);

        for (int i = 0; i < 100; i++)
        {
            var x = Random.Range(min.x, max.x); // 가로
            var z = Random.Range(min.z, max.z); // 세로

            var rndType = (TreeType)Random.Range(0, 3);
            GameObject treeGo = default;
            switch (rndType)
            {
                case TreeType.BasicTree:
                    treeGo = tree;
                    break;
                case TreeType.DryTree:
                    treeGo = dryTree;
                    break;
                case TreeType.CutDryTree:
                    treeGo = cutDryTree;
                    break;
            }
            var tr = Instantiate(treeGo, new Vector3(x, 0f, z), Quaternion.identity);
            tr.transform.SetParent(go.transform);
            tr.name = rndType.ToString();
            var treeInfo = new WorldMapTreeInfo
            {
                treePos = tr.transform.position,
                index = (int)rndType
            };
            treeList.Add(treeInfo);
        }

        // 나무가 랜덤하게 생성 될 때 간선에 있으면 안되고 노드도 가리면 안된다

        Save();
    }

    public void Save()
    {
        var treeData = Vars.UserData.WorldMapTree;
        treeData.Clear();
        for (int i = 0; i < treeList.Count; i++)
        {
            var treeInfo = new WorldMapTreeInfo();
            treeInfo.treePos = treeList[i].treePos;
            treeInfo.index = treeList[i].index;
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
            GameObject treeGo = default;
            var type = (TreeType)treeData[i].index;
            switch (type)
            {
                case TreeType.BasicTree:
                    treeGo = tree;
                    break;
                case TreeType.DryTree:
                    treeGo = dryTree;
                    break;
                case TreeType.CutDryTree:
                    treeGo = cutDryTree;
                    break;
            }
            var tr = Instantiate(treeGo, treeData[i].treePos, Quaternion.identity);
            tr.transform.SetParent(go.transform);
            tr.name = type.ToString();
        }
    }
}
