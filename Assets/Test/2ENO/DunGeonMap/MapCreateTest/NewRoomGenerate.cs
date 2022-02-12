using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomGenerate : MonoBehaviour
{
    public Vector3 spawnPos;
    public List<Vector3> objPosList = new List<Vector3>();
    //private List<Vector3> posCheckList = new();

    // TODO : 일단 임시?
    [SerializeField] private List<NewRoomInstance> mainPrefabList = new List<NewRoomInstance>();
    [SerializeField] private List<NewRoomInstance> subPrefabList = new List<NewRoomInstance>();

    public List<NewRoomInstance> roomList = new List<NewRoomInstance>();
    // 임시 Pool
    private List<NewRoomInstance> pool = new List<NewRoomInstance>();

    private List<int> roadNumList = new List<int>();

    public void EndInit()
    {
        gameObject.SetActive(false);
    }
    public void Init()
    {
        gameObject.SetActive(true);
    }

    public void RoadListClear()
    {
        roadNumList.Clear();
    }

    public void RoomPrefabSet(DungeonRoom curDungeonRoom)
    {
        gameObject.SetActive(true);

        var roadCount = curDungeonRoom.roadCount;
        bool isMain = false;
        if (curDungeonRoom.RoomType == DunGeonRoomType.MainRoom)
        {
            roadCount = 1;
            isMain = true;
        }

        if(roadNumList.Count <= 0)
            roadNumList = RoomListSet(roadCount, isMain);

        //if (pool.Count <= 0)
        //{
        //    // 현재 만들어진 프리팹 돌면서 한개씩 미리 생성해놓기
        //    for (int i = 0; i < subPrefabList.Count; i++)
        //    {
        //        var roomObj = Instantiate(subPrefabList[i], transform);
        //        roomObj.prefabId = i;
        //        roomObj.isActive = false;
        //        roomObj.isMain = false;
        //        roomObj.gameObject.SetActive(false);
        //        pool.Add(roomObj);
        //    }

        //    for(int i=0; i< mainPrefabList.Count; i++)
        //    {
        //        var roomObj = Instantiate(mainPrefabList[i], transform);
        //        roomObj.prefabId = i;
        //        roomObj.isActive = false;
        //        roomObj.isMain = true;
        //        roomObj.gameObject.SetActive(false);
        //        pool.Add(roomObj);
        //    }
        //}
        // 방 세팅전 초기화
        foreach (var room in roomList)
        {
            objPosList.Clear();
            Destroy(room.gameObject);
            //room.isActive = false;
            //room.gameObject.SetActive(false);
        }
        roomList.Clear();

        // 길방 수만큼 만듬 (메인은 1)
        for (int i = 0; i < roadCount; i++)
        {
            NewRoomInstance roomPrefab;
            if(roadCount == 1)
            {
                //roomPrefab = pool.Find(x => x.prefabId == roadNumList[i] && x.isMain == true && x.isActive == false);
                //if (roomPrefab == null)
                //{
                //    var roomObj = Instantiate(subPrefabList[roadNumList[i]], transform);
                //    roomObj.prefabId = roadNumList[i];
                //    roomObj.isActive = false;
                //    roomObj.gameObject.SetActive(false);
                //    pool.Add(roomObj);
                //    roomPrefab = roomObj;
                //}

                var roomObj = Instantiate(mainPrefabList[roadNumList[i]], transform);
                roomPrefab = roomObj;
            }
            else
            {
                //roomPrefab = pool.Find(x => x.prefabId == roadNumList[i] && x.isActive == false);
                //if (roomPrefab == null)
                //{
                //    var roomObj = Instantiate(subPrefabList[roadNumList[i]], transform);
                //    roomObj.prefabId = roadNumList[i];
                //    roomObj.isActive = false;
                //    roomObj.gameObject.SetActive(false);
                //    pool.Add(roomObj);
                //    roomPrefab = roomObj;
                //}
                var roomObj = Instantiate(subPrefabList[roadNumList[i]], transform);
                roomPrefab = roomObj;
            }
            

            // Number 1 방
            if (roomList.Count <= 0)
            {
                roomPrefab.gameObject.SetActive(true);
                //roomPrefab.isActive = true;
                roomPrefab.transform.position = transform.position;
                roomList.Add(roomPrefab);

                spawnPos = roomPrefab.transform.GetChild(0).position;
                objPosList.Add(roomPrefab.transform.GetChild(1).position);
            }
            // 2부터의 방
            else
            {
                var newPosition = NewPos(roomList[i - 1].gameObject);
                roomPrefab.gameObject.SetActive(true);
                //roomPrefab.isActive = true;
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
            foreach (var obj in curDungeonRoom.eventObjDataList)
            {
                obj.eventBasePos = objPosList[i];
            }
            // TODO : 옆으로 길어지는 코드 깔금하게 정리필요
            if (curDungeonRoom.nextRoomIdx == -1)
                break;
            curDungeonRoom = DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[curDungeonRoom.nextRoomIdx];
        }
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
            childEnd[i].roomNumber = i;
            if (i == childEnd.Length - 1)
            {
                childEnd[i].isLastPos = true;
            }
        }
    }

    // 일단 z축으로만 이동  -> x축으로 변경
    private Vector3 NewPos(GameObject baseObj)
    {
        Vector3 baseCenter = baseObj.transform.position;
        Vector3 baseSize = baseObj.GetComponent<MeshCollider>().bounds.size;

        var newPos = new Vector3(baseCenter.x + baseSize.x, baseCenter.y, baseCenter.z);
        return newPos;
    }

    private List<int> RoomListSet(int roadCount, bool isMain)
    {
        List<int> prefabNumberList = new List<int>();
        // 지금은 0 ~ 2 넘버 프리팹 3개 - 존재하니 이 사이에서 숫자 뽑음
        // 연속해서 이어지지만 않으면 중복도 상관 x
        // 검사 : 이전과 현재 비교
        int rangeNum;
        if (isMain)
            rangeNum = mainPrefabList.Count;
        else
            rangeNum = subPrefabList.Count;

        for (int i = 0; i < roadCount; i++)
        {
            var randumNum = Random.Range(0, rangeNum);
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
