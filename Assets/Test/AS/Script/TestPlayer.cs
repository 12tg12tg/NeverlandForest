using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TestPlayer : MonoBehaviour
{
    public GameObject map;
    private MapNode[] totalMap;

    private Vector2 currentIndex;
    public Vector2 CurrentIndex => currentIndex;

    public void Init()
    {
        totalMap = new MapNode[map.transform.childCount];
        for (int i = 0; i < map.transform.childCount; i++)
        {
            totalMap[i] = map.transform.GetChild(i).gameObject.GetComponent<MapNode>();
        }
        totalMap.OrderBy(n => n.level);
        currentIndex = totalMap[0].index;

        gameObject.transform.position = totalMap[0].transform.position + new Vector3(0f, 1.5f, 0f);

    }

    public void PlayerWorldMap(Vector3 pos, Vector2 index)
    {
        currentIndex = index;
        //StartCoroutine(PlayerWorldMove(pos));
        StartCoroutine(Utility.CoTranslate(transform, transform.position, pos, 1f));
        Debug.Log("�̵� ��");
    }

}
