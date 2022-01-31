using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrapSelecter : MonoBehaviour
{
    // Singleton
    private BottomUIManager bottomUI;
    private BattleManager bm;
    private TileMaker tm;

    // Instance
    private Tiles lastSnareTile;

    // Vars
    private DataAllItem curItem;
    private TrapTag curObstacleType;

    private void Start()
    {
        bottomUI = BottomUIManager.Instance;
        bm = BattleManager.Instance;
        tm = TileMaker.Instance;
    }

    public void WaitUntilTrapTileSelect(DataAllItem item)
    {
        curItem = item;
        curObstacleType = item.ItemTableElem.obstacleType;
        StartCoroutine(CoWaitUntilSelectTrapTile());
    }

    public IEnumerator CoWaitUntilSelectTrapTile()
    {
        bm.inputLink.DisableStartButton();

        // �κ��丮���� ��ġ�� ������ Ŭ���� ���۵Ǵ� �ڷ�ƾ.
        int count = curObstacleType == TrapTag.Snare ? 2 : 1;
        int prog = 0;
        while (prog < count)
        {
            // 1) Ÿ�� ���� ���
            tm.IsWaitingToSelectTrapTile = true;
            bm.uiLink.PrintCaution($"��ġ�� Ÿ���� Ŭ���ϼ���. {prog} / {count}", 1f, 0.2f, null);
            DisPlayTrapableTile(curObstacleType, prog);

            yield return new WaitUntil(() => !tm.IsWaitingToSelectTrapTile);

            // 2) Ÿ�� ǥ�� �������
            tm.SetAllTileSoftClear();

            // 3) Ÿ�� - Ʈ�� ����
            var obs = TrapPool.Instance.GetObject(curObstacleType).GetComponent<Obstacle>();
            var tile = tm.LastSelectedTile;
            InstallTrapOnTile(obs, tile, prog);

            // 4) ���� ����.
            ++prog;
            yield return null;
        }

        bm.inputLink.EnableStartButton();
    }









    private void DisPlayTrapableTile(TrapTag type, int progress)
    {
        // ��ġ ������ Ÿ�� ���͸�.
        var list = tm.TileList;
        int count = list.Count;
        switch (type)
        {
            case TrapTag.Snare:
                for (int i = 0; i < count; i++)
                {
                    bool notPlayerTile = list[i].index.y != 0;
                    bool selectable;

                    if(progress == 0)
                    {
                        bool emptyTile = list[i].obstacle == null;

                        var up = tm.GetTile(new Vector2(list[i].index.x + 1, list[i].index.y));
                        var down = tm.GetTile(new Vector2(list[i].index.x - 1, list[i].index.y));
                        bool haveNearTile = (up != null && up.obstacle == null) || (down != null && down.obstacle == null);

                        selectable = emptyTile && haveNearTile;
                    }
                    else
                    {
                        selectable = list[i].obstacle == null 
                            && list[i].index.y == lastSnareTile.index.y
                            && Mathf.Abs((int)list[i].index.x - (int)lastSnareTile.index.x) == 1f;
                    }

                    if (notPlayerTile && selectable)
                    {
                        list[i].HighlightCanAttackSign(SkillRangeType.Tile);
                    }
                }
                break;

            case TrapTag.BoobyTrap:
            case TrapTag.WoodenTrap:
            case TrapTag.ThornTrap:
                for (int i = 0; i < count; i++)
                {
                    bool notPlayerTile = list[i].index.y != 0;
                    bool selectable = list[i].obstacle == null;
                    if(notPlayerTile && selectable)
                    {
                        list[i].HighlightCanAttackSign(SkillRangeType.Tile);
                    }
                }
                break;

            case TrapTag.Fence:
                for (int i = 0; i < count; i++)
                {
                    bool notPlayerTile = list[i].index.y != 0;
                    bool notStartLine = list[i].index.y != 6;

                    int col = (int)list[i].index.y;
                    var tile0 = tm.GetTile(new Vector2(0, col));
                    var tile1 = tm.GetTile(new Vector2(1, col));
                    var tile2 = tm.GetTile(new Vector2(2, col));
                    bool selectable = tile0.obstacle == null 
                        && tile1.obstacle == null
                        && tile2.obstacle == null;

                    if (notPlayerTile && notStartLine && selectable)
                    {
                        list[i].HighlightCanAttackSign(SkillRangeType.Tile);
                    }
                }
                break;
        }
    }

    private void InstallTrapOnTile(Obstacle obs, Tiles tile, int progress)
    {
        // Ÿ�Ͽ� Ʈ�� ���� ����.
        Vector3 pos;
        switch (curObstacleType)
        {
            case TrapTag.Snare:
                tile.obstacle = obs;
                obs.transform.SetParent(tile.transform);

                pos = tile.transform.position;
                pos.y = obs.transform.position.y;
                obs.transform.position = pos;

                if (progress == 0)
                {
                    obs.Init();
                    lastSnareTile = tile;
                }
                else
                {
                    obs.Init(lastSnareTile.obstacle);
                }
                break;



            case TrapTag.BoobyTrap:
            case TrapTag.WoodenTrap:
            case TrapTag.ThornTrap:
                tile.obstacle = obs;
                obs.transform.SetParent(tile.transform);

                pos = tile.transform.position;
                pos.y = obs.transform.position.y;
                obs.transform.position = pos;
                obs.Init();
                break;




            case TrapTag.Fence:
                int col = (int)tile.index.y;
                var tile0 = tm.GetTile(new Vector2(0, col));
                var tile1 = tm.GetTile(new Vector2(1, col));
                var tile2 = tm.GetTile(new Vector2(2, col));
                tile0.obstacle = obs;
                tile1.obstacle = obs;
                tile2.obstacle = obs;

                obs.transform.SetParent(tile1.transform);

                pos = tile1.transform.position;
                pos.y = obs.transform.position.y;
                obs.transform.position = pos;

                obs.Init();
                break;
            default:
                break;
        }
    }
}
