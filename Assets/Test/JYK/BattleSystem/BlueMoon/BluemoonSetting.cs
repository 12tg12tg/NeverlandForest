using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluemoonSetting : MonoBehaviour
{
    private TileMaker tm;
    private BottomUIManager bottomUI;

    // Instance
    private Tiles lastSnareTile;

    // Vars
    private DataAllItem curItem;
    private TrapTag curObstacleType;
    private SerializeDictionary<Vector2, TrapTag> trapDict = new SerializeDictionary<Vector2, TrapTag>();

    private void Start()
    {
        bottomUI = BottomUIManager.Instance;
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
        bottomUI.tags.ForEach(n => n.interactable = false);

        // 인벤토리에서 설치물 아이템 클릭시 시작되는 코루틴.
        int count = curObstacleType == TrapTag.Snare ? 2 : 1;
        int prog = 0;
        while (prog < count)
        {
            // 1) 타일 선택 대기
            tm.IsWaitingToSelectTrapTile = true;
            //bm.uiLink.PrintCaution($"설치할 타일을 클릭하세요. {prog} / {count}", 1f, 0.5f, null);
            DisPlayTrapableTile(curObstacleType, prog);

            yield return new WaitUntil(() => !tm.IsWaitingToSelectTrapTile);

            // 2) 타일 표시 원래대로
            tm.SetAllTileSoftClear();

            // 3) 타일 - 트랩 연결
            var obs = TrapPool.Instance.GetObject(curObstacleType).GetComponent<Obstacle>();
            //obs.transform.SetParent(bm.trapParent);
            var tile = tm.LastSelectedTile;
            InstallTrapOnTile(obs, tile, prog);

            // 4) 진행도 + 1
            ++prog;

            // 5) 아이템 소모
            if (curObstacleType != TrapTag.Snare || (curObstacleType == TrapTag.Snare && prog == 2))
            {
                var allItem = new DataAllItem(curItem);
                allItem.OwnCount = 1;
                if (Vars.UserData.RemoveItemData(allItem))
                {
                    bottomUI.popUpWindow.gameObject.SetActive(false);
                    bottomUI.selectItem = null;
                    bottomUI.isPopUp = false;
                }
                bottomUI.ItemListInit();
            }

            yield return null;
        }
    }

    private void DisPlayTrapableTile(TrapTag type, int progress)
    {
        // 설치 가능한 타일 필터링.
        var list = tm.TileList;
        int count = list.Count;
        switch (type)
        {
            case TrapTag.Snare:
                for (int i = 0; i < count; i++)
                {
                    bool notPlayerTileAndStartLine = list[i].index.y != 0 && list[i].index.y != 6;
                    bool selectable;

                    if (progress == 0)
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

                    if (notPlayerTileAndStartLine && selectable)
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
                    bool notPlayerTileAndStartLine = list[i].index.y != 0 && list[i].index.y != 6;
                    bool selectable = list[i].obstacle == null;
                    if (notPlayerTileAndStartLine && selectable)
                    {
                        list[i].HighlightCanAttackSign(SkillRangeType.Tile);
                    }
                }
                break;

            case TrapTag.Fence:
                for (int i = 0; i < count; i++)
                {
                    bool notPlayerTileAndStartLine = list[i].index.y != 0 && list[i].index.y != 6;
                    bool notStartLine = list[i].index.y != 6;

                    int col = (int)list[i].index.y;
                    var tile0 = tm.GetTile(new Vector2(0, col));
                    var tile1 = tm.GetTile(new Vector2(1, col));
                    var tile2 = tm.GetTile(new Vector2(2, col));
                    bool selectable = tile0.obstacle == null
                        && tile1.obstacle == null
                        && tile2.obstacle == null;

                    if (notPlayerTileAndStartLine && notStartLine && selectable)
                    {
                        list[i].HighlightCanAttackSign(SkillRangeType.Tile);
                    }
                }
                break;
        }
    }

    private void InstallTrapOnTile(Obstacle obs, Tiles tile, int progress)
    {
        // 타일에 트랩 정보 쓰기.
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
                    obs.Init(tile);
                    lastSnareTile = tile;
                }
                else
                {
                    obs.Init(tile, lastSnareTile.obstacle);
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
                obs.Init(tile);
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

                obs.Init(tile);
                break;
            default:
                break;
        }
    }
}
