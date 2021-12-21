using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public List<MapNode> Children { get; set; } = new List<MapNode>();

    public Vector3 pos;
    public int level;
}

public class WorldMap : MonoBehaviour
{
    public GameObject cube;

    public int column;
    public int row;
    public int[,] map;
    public MapNode[,] linkMap;
    private float posX;
    private float posY;

    private bool isFirst = true; // 테스트용

    private List<MapNode> Map = new List<MapNode>();


    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            // 그래프형태로 하고싶지만 아닌 ver
            //if (!isFirst) // 리롤용
            //{
            //    ChildrenDestroy();
            //}
            //SetMap();
            //MapView();

            // 일반형태 ver
            if (!isFirst) // 리롤용
                ChildrenDestroy();

            MapRndNode();
            MapInit();
            MapLink();
        }
    }
    private void Update()
    {
        if (linkMap == null)
            return;
        foreach (var item in linkMap)
        {
            if (item == null)
                return;
            var line = item.GetComponent<LineRenderer>();
            line.startWidth = 0.25f;
            line.endWidth = 0.25f;
            line.SetPosition(0, item.pos);
            foreach (var elem in item.Children)
            {
                line.SetPosition(1, elem.pos);
            }
        }
    }

    //private void SetMap()
    //{
    //    Map.Clear();
    //    posX = posY = 0f;
    //    for (int i = 0; i < column; i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                var startNode = new MapNode<GameObject>();
    //                startNode.level = i;
    //                startNode.pos = new Vector3(posX, 0f, posY);
    //                Map.Add(startNode);
    //                break;
    //            case 1:
    //                posX += 10f;
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    var NextNode = new MapNode<GameObject>();
    //                    NextNode.level = i;
    //                    NextNode.pos = new Vector3(posX, 0f, posY);
    //                    posY += 5f;
    //                    Map.Add(NextNode);
    //                }
    //                posY = 0f;
    //                break;
    //            case 2:
    //            case 3:
    //            case 4:
    //            case 5:
    //            case 6:
    //            case 7:
    //                posX += 10f;
    //                for (int j = 0; j < Random.Range(1, row); j++)
    //                {
    //                    var NextNode = new MapNode<GameObject>();
    //                    NextNode.level = i;
    //                    NextNode.pos = new Vector3(posX, 0f, posY);
    //                    posY += 5f;
    //                    Map.Add(NextNode);
    //                }
    //                posY = 0f;
    //                break;
    //            case 8:
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    var NextNode = new MapNode<GameObject>();
    //                    NextNode.level = i;
    //                    NextNode.pos = new Vector3(posX, 0f, posY);
    //                    posY += 5f;
    //                    Map.Add(NextNode);
    //                }
    //                posY = 0f;
    //                break;
    //            case 9:
    //                var endNode = new MapNode<GameObject>();
    //                posX += 10f;
    //                endNode.level = i;
    //                endNode.pos = new Vector3(posX, 0f, posY);
    //                Map.Add(endNode);
    //                break;
    //        }
    //    }
    //    isFirst = false; // 테스트용
    //}
    //private void MapView()
    //{
    //    foreach (var item in Map)
    //    {
    //        var newCube = Instantiate(cube, new Vector3(item.pos.x, 0f, item.pos.z), Quaternion.identity);
    //        newCube.transform.SetParent(gameObject.transform);
    //    }
    //}




    // 이하 일반용 메서드
    private void MapRndNode()
    {
        map = new int[row, column]; // 현재 기준 5, 10 [4,9]

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
            map[startNext, i] = 1; // 한줄 깔기 용도 현재 기준 8개
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
    private void MapInit()
    {
        linkMap = new MapNode[row, column];
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (map[j, i].Equals(1))
                {
                    var newCube = Instantiate(cube, new Vector3(posX, 0, posY), Quaternion.identity);
                    newCube.transform.SetParent(gameObject.transform);
                    newCube.AddComponent<LineRenderer>();
                    newCube.AddComponent<MapNode>();
                    var node = newCube.GetComponent<MapNode>();
                    node.level = i;
                    node.pos = newCube.transform.position;
                    linkMap[j, i] = node;
                }
                posY += 5f;
            }
            posY = 0f;
            posX += 10f;
        }
        posX = posY = 0f;
    }
    private void MapLink()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (linkMap[i, j] == null)
                    continue;
                if (i < row - 1)
                {
                    for (int k = 0; k < row; k++)
                    {
                        if (linkMap[k, j] == null)
                            continue;
                        linkMap[i, j].Children.Add(linkMap[k, j]);
                    }
                }
            }
        }
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