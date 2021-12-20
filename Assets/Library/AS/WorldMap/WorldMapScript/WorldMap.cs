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

    private static bool isFirst = true; // 테스트용

    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            if (!isFirst) // 리롤용
                ChildrenDestroy();

            MapSetting();
            MapShow();
        }
    }


    private void MapSetting()
    {
        map = new int[row, column]; // 현재 기준 4, 5[3,4]

        var startEnd = Random.Range(0, row);
        map[startEnd, 0] = 1; // 처음
        map[startEnd, column - 1] = 1; // 끝

        var startNext = 0;
        var nextRnd = 0;
        while (startNext.Equals(nextRnd)) // 처음 다음 노드에서 중복되지 않는 한 줄과 2갈래 용도 하나
        {
            startNext = Random.Range(0, row);
            nextRnd = Random.Range(0, row);
        }
        for (int i = 1; i < column - 1; i++)
        {
            map[startNext, i] = 1; // 한줄 깔기 용도 현재 기준 3개
        }
        map[nextRnd, 1] = 1; // 처음 다음 노드에 2갈래 길 만들기 위한 애
        
        nextRnd = Random.Range(0, row); // 마지막 노드 전에 2갈래 길 만들기 위하여 한번 더 랜덤을 돌려서 중복되지 않는 노드 생성
        while (startNext.Equals(nextRnd)) // 중복되면 다시 돌리기 위한 용도
        {
            nextRnd = Random.Range(0, row);
        }
        map[nextRnd, column - 2] = 1;

        //[,(????)]: 0(처음), 1(두번째) ,column - 1(마지막), column - 2(마지막 전)
        for (int i = 2; i < column - 2; i++) // 가운데 맵
        {
            for (int j = 0; j < 2; j++)
            {
                map[Random.Range(0, row), i] = 1; // 중복 검사하지 않고 2번 돌려서 노드 생성하게끔 해놓음
            }
        }


        isFirst = false; // 테스트용
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
    // 리롤용
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