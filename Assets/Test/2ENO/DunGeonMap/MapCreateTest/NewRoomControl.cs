using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomControl : MonoBehaviour
{
    public Vector3 spawnPos;
    public List<Vector3> objPosList = new List<Vector3>();
    //private List<Vector3> posCheckList = new();

    // TODO : �ϴ� �ӽ�?
    public List<NewRoomInstance> prefabList = new List<NewRoomInstance>();
    public List<NewRoomInstance> roomList = new List<NewRoomInstance>();
    // �ӽ� Pool
    public List<NewRoomInstance> pool = new List<NewRoomInstance>();


    public void RoomPrefabSet(DungeonRoom curDungeonRoom)
    {
        var roadCount = curDungeonRoom.roadCount;
        if (curDungeonRoom.RoomType == DunGeonRoomType.MainRoom)
            roadCount = 1;

        var roadNumList = RoomListSet(roadCount);

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
            objPosList.Clear();
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
    }
    // �� �� �ѹ��� �� �������� üũ
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

    // �ϴ� z�����θ� �̵�  -> x������ ����
    private Vector3 NewPos(GameObject baseObj)
    {
        Vector3 baseCenter = baseObj.transform.position;
        Vector3 baseSize = baseObj.GetComponent<MeshCollider>().bounds.size;

        var newPos = new Vector3(baseCenter.x + baseSize.x, baseCenter.y, baseCenter.z);
        return newPos;
    }

    private List<int> RoomListSet(int roadCount)
    {
        List<int> prefabNumberList = new List<int>();
        // ������ 0 ~ 2 �ѹ� ������ 3�� - �����ϴ� �� ���̿��� ���� ����
        // �����ؼ� �̾������� ������ �ߺ��� ��� x
        // �˻� : ������ ���� ��

        for (int i = 0; i < roadCount; i++)
        {
            var randumNum = Random.Range(0, 3);
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
