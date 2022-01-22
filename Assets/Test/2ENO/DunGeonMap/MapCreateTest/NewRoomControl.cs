using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomControl : MonoBehaviour
{
    public Vector3 spawnPos;
    public List<Vector3> objPosList = new List<Vector3>();
    private DungeonSystem dungeonSystem;

    //public GameObject spawnObj;
    //public List<GameObject> eventPosObjList = new List<GameObject>();

    //private List<Vector3> posCheckList = new();

    // 일단 임시?
    public List<NewRoomInstance> prefabList = new List<NewRoomInstance>();
    public List<NewRoomInstance> roomList = new List<NewRoomInstance>();
    // 임시 Pool
    public List<NewRoomInstance> pool = new List<NewRoomInstance>();

    void Awake()
    {
        dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();
    }

    public void RoomPrefabSet(DungeonRoom curDungeonRoom)
    {
        var roadCount = curDungeonRoom.roadCount;
        if (curDungeonRoom.RoomType == DunGeonRoomType.MainRoom)
            roadCount = 1;

        var roadNumList = RoomListSet(roadCount);

        if (pool.Count <= 0)
        {
            // 현재 만들어진 프리팹 돌면서 한개씩 미리 생성해놓기
            for (int i = 0; i < prefabList.Count; i++)
            {
                var roomObj = Instantiate(prefabList[i], transform);
                roomObj.prefabId = i;
                roomObj.isActive = false;
                roomObj.gameObject.SetActive(false);
                pool.Add(roomObj);
            }
        }
        // 방 세팅전 초기화
        foreach (var room in roomList)
        {
            objPosList.Clear();
            room.isActive = false;
            room.gameObject.SetActive(false);
        }
        roomList.Clear();

        // 길방 수만큼 만듬 (메인은 1)
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

            // 첫 시작방이면
            if (roomList.Count <= 0)
            {
                roomPrefab.gameObject.SetActive(true);
                roomPrefab.isActive = true;
                roomPrefab.transform.position = transform.position;
                roomList.Add(roomPrefab);

                spawnPos = roomPrefab.transform.GetChild(0).position;
                objPosList.Add(roomPrefab.transform.GetChild(1).position);
            }
            // 이후 방
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
        // 방 프리팹 세팅 완료
        for (int i = roomList.Count - 1; i >= 0; i--)
        {
            roomList[i].transform.SetAsFirstSibling();
        }
        RoomEndPosAndNumberSet();
        for (int i = 0; i < objPosList.Count; i++)
        {
            // 임시, 가라로 만듬
            if (curDungeonRoom.randomEventData != null)
                curDungeonRoom.randomEventData.eventBasePos = objPosList[i];
            foreach (var obj in curDungeonRoom.eventObjDataList)
            {
                obj.eventBasePos = objPosList[i];
            }
            // TODO : 옆으로 길어지는 코드 깔금하게 정리필요
            if (curDungeonRoom.nextRoomIdx == -1)
                break;
            curDungeonRoom = dungeonSystem.DungeonSystemData.dungeonRoomArray[curDungeonRoom.nextRoomIdx];
        }
        //dungeonSystem.DungeonSystemData.curRoomInstanceData = 
    }
    // 각 방 넘버링 및 마지막방 체크
    private void RoomEndPosAndNumberSet()
    {
        var childEnd = gameObject.GetComponentsInChildren<EndPos>();
        for (int i = 0; i < childEnd.Length; i++)
        {
            childEnd[i].isLastPos = false;
        }

        for (int i = 0; i < childEnd.Length; i++)
        {
            childEnd[i].roomNumber = i + 1;
            if (i == childEnd.Length - 1)
            {
                childEnd[i].isLastPos = true;
            }
        }
    }

    // 일단 z축으로만 이동
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
        // 지금은 0 ~ 2 넘버 프리팹 3개 - 존재하니 이 사이에서 숫자 뽑음
        // 연속해서 이어지지만 않으면 중복도 상관 x
        // 검사 : 이전과 현재 비교

        for (int i = 0; i < roadCount; i++)
        {
            var randumNum = Random.Range(0, 3);
            if(prefabNumberList.Count <= 0)
                prefabNumberList.Add(randumNum);
            else
            {
                // 이전 number와 현재 뽑힌 number가 같다면 다시셋
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

    // 오브젝트 위치 랜덤 배치시, 겹치는거 방지 지금은 일단 안씀
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

//var childEnd = gameObject.GetComponentsInChildren<EndPos>();
//for (int i = 0; i < childEnd.Length; i++)
//{
//    childEnd[i].roomNumber = i + 1;
//    if (i == childEnd.Length - 1)
//    {
//        childEnd[i].isLastPos = true;
//    }
//}