using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomGenerate : MonoBehaviour
{
    public Vector3 spawnPos;
    public List<Vector3> objPosList = new List<Vector3>();

    [SerializeField] private List<NewRoomInstance> mainPrefabList = new List<NewRoomInstance>();
    [SerializeField] private List<NewRoomInstance> subPrefabList = new List<NewRoomInstance>();
    [SerializeField] private List<NewRoomInstance> sideFloorPrefab = new List<NewRoomInstance>();
    public List<NewRoomInstance> roomList = new List<NewRoomInstance>();
    private List<int> roadNumList = new List<int>();

    private List<NewRoomInstance> sideList = new List<NewRoomInstance>();

    //private List<Vector3> posCheckList = new();
    // �ӽ� Pool
    //private List<NewRoomInstance> pool = new List<NewRoomInstance>();

    public void EndInit()
    {
        gameObject.SetActive(false);
    }
    public void Init()
    {
        gameObject.SetActive(true);
        sideList.Add(Instantiate(sideFloorPrefab[0]));
        sideList.Add(Instantiate(sideFloorPrefab[1]));

        foreach(var floor in sideList)
        {
            floor.gameObject.SetActive(false);
        }    
    }

    public void RoadListClear()
    {
        roadNumList.Clear();
    }

    public void RoomPrefabSet(DungeonRoom curDungeonRoom)
    {
        gameObject.SetActive(true);
        var thisDungeonRoom = curDungeonRoom;
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
        //    // ���� ������� ������ ���鼭 �Ѱ��� �̸� �����س���
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
        // �� ������ �ʱ�ȭ
        foreach (var room in roomList)
        {
            objPosList.Clear();
            room.gameObject.SetActive(false);
            Destroy(room.gameObject);
            Debug.Log("�� ����");
            //room.isActive = false;
            //room.gameObject.SetActive(false);
        }
        roomList.Clear();

        // ��� ����ŭ ���� (������ 1)
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
            

            // Number 1 ��
            if (roomList.Count <= 0)
            {
                roomPrefab.gameObject.SetActive(true);
                //roomPrefab.isActive = true;
                roomPrefab.transform.position = transform.position;
                roomList.Add(roomPrefab);

                spawnPos = roomPrefab.transform.GetChild(0).position;
                objPosList.Add(roomPrefab.transform.GetChild(1).position);
            }
            // 2������ ��
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

        // ����������� �� ������ ���� �Ϸ�

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
            // TODO : ������ ������� �ڵ� ����ϰ� �����ʿ�
            if (curDungeonRoom.nextRoomIdx == -1)
                break;
            curDungeonRoom = DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[curDungeonRoom.nextRoomIdx];
        }

        if(thisDungeonRoom.RoomType == DunGeonRoomType.MainRoom)
        {
            sideList[0].gameObject.SetActive(false);
            sideList[1].gameObject.SetActive(false);
            roomList[0].PaticleStart();
        }
        else
        {
            sideList[0].gameObject.SetActive(true);
            sideList[1].gameObject.SetActive(true);
            sideList[1].PaticleStart();
            var newStartPos = new Vector3(roomList[0].transform.position.x - 20f, roomList[0].transform.position.y, roomList[0].transform.position.z);
            var newEndPos = NewPos(roomList[roomList.Count - 1].gameObject);
            sideList[0].transform.position = newStartPos;
            sideList[1].transform.position = newEndPos;
        }

    }
    // �� �� �ѹ��� �� �������� üũ
    private void RoomEndPosAndNumberSet()
    {
        var childEnd = gameObject.GetComponentsInChildren<EndPos>();
        for (int i = 0; i < childEnd.Length; i++)
        {
            childEnd[i].roomNumber = i;
            Debug.Log(childEnd[i].gameObject.name);
            if (i == childEnd.Length - 1)
            {
                childEnd[i].isLastPos = true;
            }
        }
    }

    // �ϴ� z�����θ� �̵�  -> x������ ����
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
        // ������ 0 ~ 2 �ѹ� ������ 3�� - �����ϴ� �� ���̿��� ���� ����
        // �����ؼ� �̾������� ������ �ߺ��� ��� x
        // �˻� : ������ ���� ��
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
