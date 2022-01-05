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

    private void Awake()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapNode);
        var loadData = Vars.UserData.WorldMapNodeStruct;
        worldMap = gameObject.AddComponent<WorldMap>();
        worldMap.Init(column, row, nodePrefab, linePrefab, fogPrefab);
        if (loadData.Count.Equals(0))
        {
            StartCoroutine(worldMap.InitMap(() => {
                NodeLinkToPlayer();
                player.Init();
            }));
            Debug.Log("들어왔음");
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
                            var pos = x.transform.position + new Vector3(0f, 1.5f, 0f);
                            player.PlayerWorldMap(pos, x.index);
                            return;
                        }
                    }
                };
            }
        }
    }
}
