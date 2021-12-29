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

        currentIndex = Vector2.right; // �ε��� 1,0 �̶�� �ǹ�.. ���� ���� ���� ���

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
        // �÷��̾ �̵��ϸ� ��� �� Ȯ�� ����(��ĭ ������ �̵� �� 10����, ���󹰿� ������ �� 10����)
        // ������ �÷��̾ �߰� �� Ȯ�� ����

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
            Debug.Log("����");
        }
        else
        {
            Debug.Log("����");
        }
    }

}
