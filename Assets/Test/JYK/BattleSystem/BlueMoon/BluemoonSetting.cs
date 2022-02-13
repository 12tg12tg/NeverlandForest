using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluemoonSetting : MonoBehaviour
{
    private TileMaker tm;
    private BottomUIManager bottomUI;
    private BattleManager bm;

    // Instance
    private Tiles lastSnareTile;

    private void Start()
    {
        bottomUI = BottomUIManager.Instance;
        tm = TileMaker.Instance;
        bm = BattleManager.Instance;
    }

    public void LoadTrapData()
    {
        var trapPos = Vars.UserData.trapPos;
        var trapType = Vars.UserData.trapType;

        for (int i = 0; i < trapPos.Count; i++) // 저장정보를 바탕으로 설치
        {
            var tile = tm.GetTile(trapPos[i]);

            // 펜스가 아닌경우
            if (trapType[i] != TrapTag.Fence)
            {
                var obs = TrapPool.Instance.GetObject(trapType[i]).GetComponent<Obstacle>();
                obs.transform.SetParent(bm.trapParent);

                tile.obstacle = obs;
                obs.transform.SetParent(tile.transform);

                var pos = tile.transform.position;
                pos.y = obs.transform.position.y;
                obs.transform.position = pos;

                //올가미가 아닌경우
                if (obs.type != TrapTag.Snare)
                    obs.Init(tile);
            }
            else
            {
                //펜스인경우
                if (tile.index.x == 1)
                {
                    var obs = TrapPool.Instance.GetObject(trapType[i]).GetComponent<Obstacle>();

                    int col = (int)tile.index.y;
                    var tile0 = tm.GetTile(new Vector2(0, col));
                    var tile2 = tm.GetTile(new Vector2(2, col));
                    tile0.obstacle = obs;
                    tile.obstacle = obs;
                    tile2.obstacle = obs;

                    obs.transform.SetParent(tile.transform);

                    var pos = tile.transform.position;
                    pos.y = obs.transform.position.y;
                    obs.transform.position = pos;

                    obs.Init(tile);
                }
            }
        }

        for (int y = 1; y < 5; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                var tile = tm.GetTile(new Vector2(x, y));
                if(tile.obstacle!=null && tile.obstacle.type == TrapTag.Snare)
                {
                    var another = tm.GetTile(new Vector2(x + 1, y));
                    var obs = tile.obstacle;
                    var anotherObs = another.obstacle;

                    obs.Init(tile);
                    anotherObs.Init(another, obs);
                    break;
                }
            }
        }
    }
}
