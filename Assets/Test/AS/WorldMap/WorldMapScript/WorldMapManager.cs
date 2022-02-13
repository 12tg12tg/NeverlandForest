using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapManager : MonoBehaviour
{
    [Header("월드맵에 쓰는 오브젝트")]
    public WorldMapPlayer player;
    public WorldMapCamera worldMapCamera;
    public WorldMapGround ground;

    [Header("월드맵 생성")]
    public WorldMapMaker worldMapMaker;

    [Header("프리팹")]
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public GameObject fogPrefab;

    [Header("노드 행렬")]
    public int column;
    public int row;

    [Header("UI")]
    public GameObject backDungeonBt;

    public void Awake()
    {
        worldMapCamera.Init();
        var loadData = Vars.UserData.WorldMapNodeStruct;
        worldMapMaker.Init(column, row, nodePrefab, linePrefab, fogPrefab);
        if (loadData.Count.Equals(0)) // 저장 데이터가 없을 때 실행
        {
            StartCoroutine(worldMapMaker.CreateWorldMap(() => {
                NodeLinkToPlayer();
                player.Init();
                worldMapCamera.FollowPlayer();
                if (ground != null)
                    ground.CreateTree(worldMapMaker.Edges, worldMapMaker.Maps);
                GameManager.Manager.Production.FadeOut();
            }));
        }
        else
        {
            worldMapMaker.LoadWorldMap(loadData);
            NodeLinkToPlayer();
            player.ComeBackWorldMap();
            if (ground != null)
                ground.Load();
            worldMapCamera.FollowPlayer(() =>
            {
                worldMapMaker.FogMove(Vars.UserData.uData.Date, false, player.PlayerDeathChack);
            });
            if ((int)player.CurrentIndex.y >= 1)
                backDungeonBt.SetActive(true);
            GameManager.Manager.Production.FadeOut(() =>
            {
                if ((int)player.CurrentIndex.y > 8)
                {
                    GameManager.Manager.isClear = true;
                    Title.isClear = true;
                    GameManager.Manager.Production.FadeIn(() => SceneManager.LoadScene("Game"));
                }
            });
        }

        Vars.UserData.isPlayerDungeonIn = false;

        
    }

    private void NodeLinkToPlayer()
    {
        var maps = worldMapMaker.Maps;
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
