using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DirectionInho
{
    Right,
    Left,
    Top,
    Bottom,
    Count,
}

public class DunGeonMapGenerate : MonoBehaviour
{
    public int distance = 7;
    public int startId = 100;
    public int col = 20;

    public int RoomCount = 6;
    public int remainRoom = 4;

    public DirectionInho direction;

    public GameObject mainRoomPrefab;
    public GameObject roadPrefab;
    // ���� �ʱ�ȭ ���
    public bool isSet = false;

    public DungeonRoom[] DungeonRoomList = new DungeonRoom[400];
    public List<DungeonRoom> testList = new List<DungeonRoom>();
    public int[] testMap = new int[400];
    public void Start()
    {
        //MapInit();
        //showMapTest();

        StartCoroutine(MapCorutine());

        //RandomMapInit();
        //showMap();
        //var dungen = new DungeonRoom();

        //dungen.SetEvent(DunGeonEvent.Hunt);
        //dungen.SetEvent(DunGeonEvent.Gathering);
        //dungen.SetEvent(DunGeonEvent.RandomIncount);

        //dungen.CheckEvent(DunGeonEvent.Hunt);
        //dungen.CheckEvent(DunGeonEvent.Gathering);
        //dungen.CheckEvent(DunGeonEvent.RandomIncount);
    }

    IEnumerator MapCorutine()
    {
        MapInit();
        while (remainRoom > 0)
        {
            // �ٽ� �ʱ�ȭ
            MapInit();
            TestMapSet(startId, DirectionInho.Right, 0);
            yield return null;
        }
        //MapMarking();
        CreateMapObject();
        DunGeonRoomSetting.DungeonLink(DungeonRoomList);
        DunGeonRoomSetting.DungeonLink(DungeonRoomList[startId],DungeonRoomList, testList);
        isSet = true;
    }

    public void OnGUI()
    {
        if (GUILayout.Button("reStart"))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void MapInit()
    {
        for (int i = 0; i < DungeonRoomList.Length; i++)
        {
            DungeonRoomList[i] = new DungeonRoom();
            DungeonRoomList[i].IsCheck = false;
            if (i == 0)
            {
                DungeonRoomList[i].Pos = Vector2.zero;
            }
            else
            {
                var row = i % col;
                var colum = i / col;
                DungeonRoomList[i].Pos = new Vector2(row * 2f, colum * 2f);
            }
            testMap[i] = 0;
        }
        remainRoom = RoomCount;
    }

    //public void showMapTest()
    //{
    //    while (remainRoom > 0)
    //    {
    //        // �ٽ� �ʱ�ȭ
    //        MapInit();
    //        TestMapSet(startId, DirectionInho.Right, 0);
    //    }

    //    MapMarking();
    //    CreateMapObject();
    //}
    // �ʿ��������?
    public void MapMarking()
    {
        for (int i = 0; i < DungeonRoomList.Length; i++)
        {
            if (testMap[i] == 3)
            {
                DungeonRoomList[i].IsCheck = true;
                DungeonRoomList[i].RoomType = DunGeonRoomType.MainRoom;
                DunGeonRoomSetting.RoomEventSet(DungeonRoomList[i]);
            }
            else if (testMap[i] == 4)
            {
                DungeonRoomList[i].IsCheck = true;
                DungeonRoomList[i].RoomType = DunGeonRoomType.SubRoom;
                DunGeonRoomSetting.RoomEventSet(DungeonRoomList[i]);
            }
        }
    }

    public void TestMapSet(int curId, DirectionInho lastDir, int roadCount)
    {
        if (remainRoom <= 0)
            return;

        if (!RoomException(curId))
            return;

        if (roadCount <= 0)
        {
            DungeonRoomList[curId].IsCheck = true;
            DungeonRoomList[curId].RoomType = DunGeonRoomType.MainRoom;
            DunGeonRoomSetting.RoomEventSet(DungeonRoomList[curId]);
            remainRoom--;

            // ���� ����
            var rnd = Random.Range(0, (int)DirectionInho.Count);

            while (rnd == (int)oppsiteDir(lastDir))
            {
                rnd = Random.Range(0, (int)DirectionInho.Count);
            }

            var rndCount = Random.Range(2, 6);
            var nextId = NextRoomId(curId, (DirectionInho)rnd);
            
            DungeonRoomList[curId].nextRoomIdx = nextId;
            if (remainRoom <= 0)
                DungeonRoomList[curId].nextRoomIdx = -1;
            TestMapSet(nextId, (DirectionInho)rnd, rndCount);
        }
        else
        {
            roadCount--;
            DungeonRoomList[curId].IsCheck = true;
            DungeonRoomList[curId].RoomType = DunGeonRoomType.SubRoom;
            DunGeonRoomSetting.RoomEventSet(DungeonRoomList[curId]);

            var perCent = Random.Range(0, 10);
            // ���� ����� ���� ����, �� ����
            var nextId = NextRoomId(curId, lastDir);

            // Ȯ���� ���� ����
            if (perCent > 7)
            {
                var rnd = Random.Range(0, (int)DirectionInho.Count);
                while (rnd == (int)oppsiteDir(lastDir))
                {
                    rnd = Random.Range(0, (int)DirectionInho.Count);
                }
                lastDir = (DirectionInho)rnd;
                nextId = NextRoomId(curId, (DirectionInho)rnd);
            }

            DungeonRoomList[curId].nextRoomIdx = nextId;
            TestMapSet(nextId, lastDir, roadCount);
        }
        return;
    }

    public bool RoomException(int curId)
    {
        if (curId == -1)
            return false;

        if (testMap[curId] != 0)
            return false;

        return true;
    }

    public void CreateMapObject()
    {
        for (int i = 0; i < DungeonRoomList.Length; i++)
        {
            if (DungeonRoomList[i].IsCheck)
            {
                if (DungeonRoomList[i].RoomType == DunGeonRoomType.MainRoom)
                {
                    var obj = Instantiate(mainRoomPrefab, new Vector3(DungeonRoomList[i].Pos.x, DungeonRoomList[i].Pos.y, 0f)
                         , Quaternion.identity);
                    var text =  obj.GetComponent<TestObject>();
                    text.text.SetText(DungeonRoomList[i].GetEvent().ToString());
                }
                else
                {
                    var obj = Instantiate(roadPrefab, new Vector3(DungeonRoomList[i].Pos.x, DungeonRoomList[i].Pos.y, 0f)
                    , Quaternion.identity);
                    var text = obj.GetComponent<TestObject>();
                    text.text.SetText(DungeonRoomList[i].GetEvent().ToString());
                }
            }
        }
    }
    public DirectionInho oppsiteDir(DirectionInho dir)
    {
        var direction = DirectionInho.Count;

        switch (direction)
        {
            case DirectionInho.Right:
                direction = DirectionInho.Left;
                break;
            case DirectionInho.Left:
                direction = DirectionInho.Right;
                break;
            case DirectionInho.Top:
                direction = DirectionInho.Bottom;
                break;
            case DirectionInho.Bottom:
                direction = DirectionInho.Top;
                break;
            case DirectionInho.Count:
                break;
        }

        return direction;
    }

    public int NextRoomId(int currentId, DirectionInho dir)
    {
        int result = -1;
        switch (dir)
        {
            case DirectionInho.Right:
                if (currentId % col == col - 1)
                {
                    break;
                }
                result = currentId + 1;
                break;
            case DirectionInho.Left:
                //if (currentId % col == 0)
                //{
                //    break;
                //}
                //result = currentId - 1;
                break;
            case DirectionInho.Top:
                if (currentId < col)
                {
                    break;
                }
                result = currentId - col;
                break;
            case DirectionInho.Bottom:
                if (col * (col - 1) <= currentId && currentId < col * col)
                {
                    break;
                }
                result = currentId + col;
                break;
        }
        if (result < -1)
        {
            Debug.Log("d");
        }
        return result;
    }
}


// id�� 2���� �迭 ����,  �˻�����
// ��ĭ = ���� id - ��ü col / �Ʒ�ĭ = ���� id + ��ü col
// ��ĭ = ���� id - 1 / ����ĭ = ���� id + 1

// �����ڸ� �Ǵ�(nũ�� �簢��) ������ = id < n
// �Ʒ����� =  n*(n-1) <= id < n*n
// �޶��� = id % n == 0 ( id�� 0�ϋ��� ����ó�� )
// �������� = id % n == n-1 ( id�� 0�ϋ��� ����ó�� )

// ��ġ��ǥ = ���� position�������� ���⿡ ���� x��ǥ +-, z ��ǥ +-

// �ӽ÷� �켱���� ������ �ð���� ����

// ���簢 ���� id �˻�,  createId, 
// createid - col*2 - 2 ~ createid - col*2 + 2 
// createid - col*1 - 2 ~ createid - col*2 + 2 
// createid - col*0 - 2 ~ createid - col*2 + 2 
// createid - col*-1 - 2 ~ createid - col*2 + 2 
// createid - col*-2 - 2 ~ createid - col*2 + 2 



//public void RandomMapInit()
//{ 
//    while (remainRoom > 0)
//    {
//        CreateRoom(startId, 0, Direction.Count);
//    }
//}

//public int CreateRoom(int curId, int count, Direction dir)
//{
//    // ��ŸƮ Direction�� �ƴҶ�
//    if(dir != Direction.Count)
//    {
//        if (remainRoom < 0)
//        {
//            return 0;
//        }
//        if(curId == -1)
//        {
//            return -1;
//        }
//        Debug.Log(curId);
//        if(DungeonRoomList[curId].IsCheck == true)
//        {
//            return -1;
//        }
//    }

//    // �� �������϶�
//    if (count > 0)
//    {
//        count--;
//        DungeonRoomList[curId].IsCheck = true;
//        DungeonRoomList[curId].IsMain = false;

//        var nextid = NextRoomId(curId, dir);
//        CreateRoom(nextid,count, dir);
//    }
//    // ���ι� �������϶�
//    else
//    {
//        DungeonRoomList[curId].IsCheck = true;
//        DungeonRoomList[curId].IsMain = true;

//        //if (curId == startId)
//        //{
//        //}
//        //else
//        //{
//        //}

//        remainRoom--;

//        Direction setDir;
//        bool noneDir = true;

//        // 50% Ȯ���� �ش� �������� �� �������� ���ϱ�
//        if(Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Right;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }
//        if (Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Left;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }
//        if (Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Bottom;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }
//        if (Random.Range(0, 2) == 1)
//        {
//            noneDir = false;
//            setDir = Direction.Top;
//            var nextIdx = NextRoomId(curId, setDir);
//            CreateRoom(nextIdx, roadCount, setDir);
//        }

//        // 4���� �� ������ ��� �������� ��
//        if(noneDir)
//        {
//            for (int i = 0; i < (int)Direction.Count; i++)
//            {
//                // ���ݱ����� �������� �����ʴ��϶�? -> ���⼭ ���� ���°� 
//                if(dir != (Direction)i)
//                {
//                    var nextIdx = NextRoomId(curId, (Direction)i);
//                    CreateRoom(nextIdx, roadCount, (Direction)i);
//                    return 1;
//                }
//            }
//        }
//    }
//    return 1;
//}



//StringBuilder sb = new StringBuilder();
//for (int j = 1; j < (int)DunGeonEvent.Count;)
//{
//    DunGeonEvent curEvent = DunGeonEvent.Empty;
//    if(((DunGeonEvent)j & DungeonRoomList[i].GetEvent()) != 0)
//    {
//        curEvent = (DunGeonEvent)j;
//    }
//    switch (curEvent)
//    {
//        case DunGeonEvent.Battle:
//            sb.Append($"{DunGeonEvent.Battle.ToString()} + ");
//            break;
//        case DunGeonEvent.Gathering:
//            sb.Append($"{DunGeonEvent.Gathering.ToString()} + ");
//            break;
//        case DunGeonEvent.Hunt:
//            sb.Append($"{DunGeonEvent.Hunt.ToString()} + ");
//            break;
//        case DunGeonEvent.RandomIncount:
//            sb.Append($"{DunGeonEvent.RandomIncount.ToString()} + ");
//            break;
//        case DunGeonEvent.SubStory:
//            sb.Append($"{DunGeonEvent.SubStory.ToString()} + ");
//            break;
//    }

//    text.text.SetText(sb);
//    j <<= j;
//    exeptemp++;
//    if(exeptemp > 100)
//    {
//        Debug.Log("����");
//        break;
//    }    
//}