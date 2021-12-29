using System.Collections;
using UnityEngine;
using System.Linq;

public class WorldMapPlayer : MonoBehaviour
{
    public GameObject map;
    private WorldMapNode[] totalMap;
    private Coroutine coMove;
    private Vector2 currentIndex;
    public Vector2 CurrentIndex => currentIndex;

    public void Init()
    {
        totalMap = new WorldMapNode[map.transform.childCount];
        for (int i = 0; i < map.transform.childCount; i++)
        {
            totalMap[i] = map.transform.GetChild(i).GetComponent<WorldMapNode>();
        }
        totalMap.OrderBy(n => n.level);
        currentIndex = totalMap[0].index;

        transform.position = totalMap[0].transform.position + new Vector3(0f, 1.5f, 0f);
    }

    public void ComeBackWorldMap()
    {
        for (int i = 0; i < totalMap.Length; i++)
        {
            if(totalMap[i].index.Equals(currentIndex))
            {
                transform.position = totalMap[i].transform.position + new Vector3(0f, 1.5f, 0f);
                return;
            }
        }
    }

    public void PlayerWorldMap(Vector3 pos, Vector2 index)
    {
        currentIndex = coMove == null ? index : currentIndex;
        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, pos, 1f, () => coMove = null));
    }
}