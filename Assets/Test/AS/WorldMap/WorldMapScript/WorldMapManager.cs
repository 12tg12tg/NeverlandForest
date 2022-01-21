using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    [Header("월드맵에 쓰는 오브젝트")]
    public WorldMapPlayer player;
    public WorldMapCamera worldMapCamera;
    public WorldMapGround ground;

    [Header("프리팹")]
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public GameObject fogPrefab;

    [Header("노드 행렬")]
    public int column;
    public int row;

    private WorldMap worldMap;

    private void Start()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapData);
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        var loadData = Vars.UserData.WorldMapNodeStruct;
        worldMap = gameObject.AddComponent<WorldMap>();
        worldMap.Init(column, row, nodePrefab, linePrefab, fogPrefab);
        if (loadData.Count.Equals(0)) // 저장 데이터가 없을 때 실행
        {
            StartCoroutine(worldMap.InitMap(() => {
                NodeLinkToPlayer();
                player.Init();
                worldMapCamera.FollowPlayer();
                if (ground != null)
                    ground.CreateTree(worldMap.Edges, worldMap.Maps);
            }));
        }
        else
        {
            worldMap.LoadWorldMap(loadData);
            NodeLinkToPlayer();
            player.ComeBackWorldMap();
            worldMapCamera.FollowPlayer();
            if (ground != null)
                ground.Load();
        }
    }

    private void NodeLinkToPlayer()
    {
        var maps = worldMap.Maps;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[i][j] == null)
                    continue;
                var node = maps[i][j].GetComponent<WorldMapNode>();
                node.OnClick += (x) =>
                {
                    for (int i = 0; i < x.Parent.Count; i++)
                    {
                        if (x.Parent[i].index.Equals(player.CurrentIndex))
                        {
                            player.PlayerWorldMap(x);
                            return;
                        }
                    }
                };
            }
        }
    }
}
