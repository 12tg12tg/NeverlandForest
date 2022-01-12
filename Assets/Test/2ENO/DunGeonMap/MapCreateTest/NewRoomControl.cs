using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomControl : MonoBehaviour
{
    public Vector3 spawnPos;
    public List<Vector3> objPosList = new List<Vector3>();

    //public GameObject spawnObj;
    //public List<GameObject> eventPosObjList = new List<GameObject>();

    //private List<Vector3> posCheckList = new();

    // �ϴ� �ӽ�?
    public List<NewRoomInstance> prefabList = new List<NewRoomInstance>();
    public List<NewRoomInstance> roomList = new List<NewRoomInstance>();
    // �ӽ� Pool
    public List<NewRoomInstance> pool = new List<NewRoomInstance>();

    void Awake()
    {
        //var childEnd = gameObject.GetComponentsInChildren<EndPos>();
        //for (int i = 0; i < childEnd.Length; i++)
        //{
        //    childEnd[i].roomNumber = i + 1;
        //    if (i == childEnd.Length - 1)
        //    {
        //        childEnd[i].isLastPos = true;
        //    }
        //}
    }

    public void RoomPrefabSet(DungeonRoom curDungeonData)
    {
        var roadCount = curDungeonData.roadCount;
        var roadNumList = RoomListSet(roadCount);

        if (curDungeonData.RoomType == DunGeonRoomType.MainRoom)
            roadCount = 1;

        if (pool.Count <= 0)
        {
            // ���� ������� ������ ���鼭 �Ѱ��� �̸� �����س���
            for (int i = 0; i < prefabList.Count; i++)
            {
                var roomObj = Instantiate(prefabList[i], transform);
                roomObj.prefabId = i;
                roomObj.isActive = false;
                roomObj.gameObject.SetActive(false);
                pool.Add(roomObj);
            }
        }
        // �� ������ �ʱ�ȭ
        foreach (var room in roomList)
        {
            room.isActive = false;
            room.gameObject.SetActive(false);
        }
        roomList.Clear();

        // ��� ����ŭ ���� (������ 1)
        for (int i = 0; i < roadCount; i++)
        {
            var roomPrefab = pool.Find(x => x.prefabId == roadNumList[i] && x.isActive == false);
            if (roomPrefab == null)
            {
                var roomObj = Instantiate(prefabList[roadNumList[i]], transform);
                roomObj.prefabId = roadNumList[i];
                roomObj.isActive = false;
                roomObj.gameObject.SetActive(false);
                pool.Add(roomObj);
                roomPrefab = roomObj;
            }

            // ù ���۹��̸�
            if (roomList.Count <= 0)
            {
                roomPrefab.gameObject.SetActive(true);
                roomPrefab.isActive = true;
                roomPrefab.transform.position = transform.position;
                roomList.Add(roomPrefab);

                spawnPos = roomPrefab.transform.GetChild(0).position;
                objPosList.Add(roomPrefab.transform.GetChild(1).position);
            }
            // ���� ��
            else
            {
                var newPosition = NewPos(roomList[i - 1].gameObject);
                roomPrefab.gameObject.SetActive(true);
                roomPrefab.isActive = true;
                roomPrefab.transform.position = newPosition;
                roomList.Add(roomPrefab);

                objPosList.Add(roomPrefab.transform.GetChild(1).position);
            }
        }
        // �� ������ ���� �Ϸ�
        RoomEndPosAndNumberSet();
    }
    // �� �� �ѹ��� �� �������� üũ
    private void RoomEndPosAndNumberSet()
    {
        var childEnd = gameObject.GetComponentsInChildren<EndPos>();
        for (int i = 0; i < childEnd.Length; i++)
        {
            childEnd[i].roomNumber = i + 1;
            if (i == childEnd.Length - 1)
            {
                childEnd[i].isLastPos = true;
            }
        }
    }

    // �ϴ� z�����θ� �̵�
    private Vector3 NewPos(GameObject baseObj)
    {
        Vector3 baseCenter = baseObj.transform.position;
        Vector3 baseSize = baseObj.GetComponent<MeshCollider>().bounds.size;

        var newPos = new Vector3(baseCenter.x, baseCenter.y, baseCenter.z + baseSize.z);
        return newPos;
    }

    private List<int> RoomListSet(int roadCount)
    {
        List<int> prefabNumberList = new List<int>();
        // ������ 0 ~ 4 �ѹ� ������ �����ϴ� �� ���̿��� ���� ����
        // �����ؼ� �̾������� ������ �ߺ��� ��� x
        // �˻� : ������ ���� ��

        for (int i = 0; i < roadCount; i++)
        {
            var randumNum = Random.Range(0, 5);
            if(prefabNumberList.Count <= 0)
                prefabNumberList.Add(randumNum);
            else
            {
                // ���� number�� ���� ���� number�� ���ٸ� �ٽü�
                if(randumNum == prefabNumberList[i-1])
                {
                    i--;
                    continue;
                }
                else
                    prefabNumberList.Add(randumNum);
            }
        }

        return prefabNumberList;
    }

    // ������Ʈ ��ġ ���� ��ġ��, ��ġ�°� ���� ������ �ϴ� �Ⱦ�
    //public bool PositionCheck(Vector3 pos)
    //{
    //    if (posCheckList.Count > 1)
    //    {
    //        for (int i = 0; i < posCheckList.Count; i++)
    //        {
    //            if (Mathf.Abs(posCheckList[i].z - pos.z) < 0.3f)
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}

}

//// test
//public List<GameObject> prefabList = new List<GameObject>();
//public List<TestPrefabId> roomList = new List<TestPrefabId>();
//// objectPool
//public List<TestPrefabId> pool = new List<TestPrefabId>();

//var randomRoad = 7;
//var roomNumberList = RoomListSet(randomRoad);

//if(pool.Count <= 0)
//{
//    for (int i = 0; i < prefabList.Count; i++)
//    {
//        var obj = Instantiate(prefabList[i], transform);
//        var prefab = obj.AddComponent<TestPrefabId>();
//        prefab.prefabId = i;
//        prefab.isActive = false;
//        prefab.gameObject.SetActive(false);
//        pool.Add(prefab);
//    }
//}
//foreach (var room in roomList)
//    room.gameObject.SetActive(false);
//roomList.Clear();

//for (int i = 0; i < randomRoad; i++)
//{
//    var prefab = pool.Find(x => x.prefabId == roomNumberList[i] && x.isActive == false);
//    if(prefab == null)
//    {
//        var obj = Instantiate(prefabList[roomNumberList[i]], transform);
//        var tempPrefab = obj.AddComponent<TestPrefabId>();
//        tempPrefab.prefabId = roomNumberList[i];
//        tempPrefab.isActive = false;
//        tempPrefab.gameObject.SetActive(false);
//        pool.Add(tempPrefab);
//        prefab = tempPrefab;
//    }


//    if (roomList.Count <= 0)
//    {
//        prefab.gameObject.SetActive(true);
//        prefab.isActive = true;
//        prefab.transform.position = transform.position;
//        roomList.Add(prefab);
//    }
//    else
//    {
//        var newPosition = NewPos(roomList[i-1].gameObject);
//        prefab.gameObject.SetActive(true);
//        prefab.isActive = true;
//        prefab.transform.position = newPosition;
//        roomList.Add(prefab);
//    }