using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingPlayer : MonoBehaviour
{
    public TileMaker tileMaker;
    public Tiles[] tiles;
    public Vector2 currentIndex;
    public int huntingPercentage;

    private void Start()
    {
        var count = tileMaker.transform.childCount;
        tiles = new Tiles[count];
        for (int i = 0; i < count; i++)
        {
            tiles[i] = tileMaker.transform.GetChild(i).GetComponent<Tiles>();
        }

        var basicPercentage = Random.Range(20, 31);
        var staminaPercentage = 15;

        huntingPercentage += basicPercentage + staminaPercentage;

        currentIndex = Vector2.right; // 인덱스 1,0 이라는 의미.. 최초 시작 왼쪽 가운데

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].index.Equals(currentIndex))
            {
                StartCoroutine(Utility.CoTranslate(transform, transform.position, tiles[i].transform.position, 1f));
            }
        }
    }


    private void OnGUI()
    {
        if(GUILayout.Button("Hunt"))
        {
            Hunting();
        }
    }

    public void Move(Vector2 index)
    {
        // 플레이어가 이동하면 사냥 할 확률 증가(한칸 앞으로 이동 시 10프로, 은폐물에 숨었을 때 10프로)
        // 동물이 플레이어를 발견 할 확률 증가

        for (int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i].index.Equals(index))
            {
                currentIndex = index;
                huntingPercentage += 10;
                StartCoroutine(Utility.CoTranslate(transform, transform.position, tiles[i].transform.position, 1f));
            }
        }


    }


    private void Hunting()
    {
        if (Random.Range(0f, 1f) < huntingPercentage * 0.01f)
        {
            Debug.Log("성공");
        }
        else
        {
            Debug.Log("실패");
        }
    }

}
