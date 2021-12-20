using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public GameObject cube;

    public int column;
    public int row;
    public int[,] map;

    private List<MapNode<int>> Map;
    private MapNode<int> startMapNode;
    private float posX;
    private float posY;

    private static bool isFirst = true; // �׽�Ʈ��

    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            if (!isFirst) // ���ѿ�
                ChildrenDestroy();

            MapSetting();
            MapShow();
        }
    }


    private void MapSetting()
    {
        map = new int[row, column]; // ���� ���� 4, 5[3,4]

        var startEnd = Random.Range(0, row);
        map[startEnd, 0] = 1; // ó��
        map[startEnd, column - 1] = 1; // ��

        var startNext = 0;
        var nextRnd = 0;
        while (startNext.Equals(nextRnd)) // ó�� ���� ��忡�� �ߺ����� �ʴ� �� �ٰ� 2���� �뵵 �ϳ�
        {
            startNext = Random.Range(0, row);
            nextRnd = Random.Range(0, row);
        }
        for (int i = 1; i < column - 1; i++)
        {
            map[startNext, i] = 1; // ���� ��� �뵵 ���� ���� 3��
        }
        map[nextRnd, 1] = 1; // ó�� ���� ��忡 2���� �� ����� ���� ��
        
        nextRnd = Random.Range(0, row); // ������ ��� ���� 2���� �� ����� ���Ͽ� �ѹ� �� ������ ������ �ߺ����� �ʴ� ��� ����
        while (startNext.Equals(nextRnd)) // �ߺ��Ǹ� �ٽ� ������ ���� �뵵
        {
            nextRnd = Random.Range(0, row);
        }
        map[nextRnd, column - 2] = 1;

        //[,(????)]: 0(ó��), 1(�ι�°) ,column - 1(������), column - 2(������ ��)
        for (int i = 2; i < column - 2; i++) // ��� ��
        {
            for (int j = 0; j < 2; j++)
            {
                map[Random.Range(0, row), i] = 1; // �ߺ� �˻����� �ʰ� 2�� ������ ��� �����ϰԲ� �س���
            }
        }


        isFirst = false; // �׽�Ʈ��
    }
    private void MapShow()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (map[i, j].Equals(1))
                {
                    var newCube = Instantiate(cube, new Vector3(posX, 0, posY), Quaternion.identity);
                    newCube.transform.parent = gameObject.transform;
                }
                posX += 10f;
            }
            posX = 0f;
            posY += 5f;
        }
        posX = posY = 0f;
    }
    // ���ѿ�
    private void ChildrenDestroy()
    {
        Transform[] childList = GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }
}