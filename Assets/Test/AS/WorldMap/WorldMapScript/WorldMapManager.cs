using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public WorldMapPlayer player;
    public WorldMapCamera worldMapCamera;

    public GameObject nodePrefab;
    public GameObject linePrefab;
    public GameObject fogPrefab;

    public int column;
    public int row;

    private WorldMap worldMap;

    private void Start()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapNode);
        var loadData = Vars.UserData.WorldMapNodeStruct;
        worldMap = gameObject.AddComponent<WorldMap>();
        worldMap.Init(column, row, nodePrefab, linePrefab, fogPrefab);
        if (loadData.Count.Equals(0)) // ���� �����Ͱ� ���� �� ����
        {
            StartCoroutine(worldMap.InitMap(() => {
                NodeLinkToPlayer();
                player.Init();
                worldMapCamera.FollowPlayer();
            }));
        }
        else
        {
            worldMap.LoadWorldMap(loadData);
            NodeLinkToPlayer();
            player.ComeBackWorldMap();
            worldMapCamera.FollowPlayer();
        }
    }

    private void NodeLinkToPlayer()
    {
        var maps = worldMap.maps;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;
                var node = maps[j, i].GetComponent<WorldMapNode>();
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
