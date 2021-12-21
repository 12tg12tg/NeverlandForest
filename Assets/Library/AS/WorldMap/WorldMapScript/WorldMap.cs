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

    private bool isFirst = true; // �׽�Ʈ��

    private List<MapNode> Map = new List<MapNode>();


    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            // �׷������·� �ϰ������ �ƴ� ver
            //if (!isFirst) // ���ѿ�
            //{
            //    ChildrenDestroy();
            //}
            //SetMap();
            //MapView();

            // �Ϲ����� ver
            if (!isFirst) // ���ѿ�
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
    //    isFirst = false; // �׽�Ʈ��
    //}
    //private void MapView()
    //{
    //    foreach (var item in Map)
    //    {
    //        var newCube = Instantiate(cube, new Vector3(item.pos.x, 0f, item.pos.z), Quaternion.identity);
    //        newCube.transform.SetParent(gameObject.transform);
    //    }
    //}




    // ���� �Ϲݿ� �޼���
    private void MapRndNode()
    {
        map = new int[row, column]; // ���� ���� 5, 10 [4,9]

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
            map[startNext, i] = 1; // ���� ��� �뵵 ���� ���� 8��
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