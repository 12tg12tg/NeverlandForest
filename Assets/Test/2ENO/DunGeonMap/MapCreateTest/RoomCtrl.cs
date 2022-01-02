using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCtrl : MonoBehaviour
{
    public GameObject spawnPos;
    public GameObject[] evnetObjPos;

    private List<Vector3> posCheckList = new();
    public List<GameObject> eventObjList = new();

    public DungeonSystem dungeonSystem;

    private void OnEnable()
    {
        dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();

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
    // 오브젝트 위치 랜덤 배치시, 겹치는거 방지 지금은 일단 안씀
    public bool PositionCheck(Vector3 pos)
    {
        if(posCheckList.Count > 1)
        {
            for (int i = 0; i < posCheckList.Count; i++)
            {
                if(Mathf.Abs(posCheckList[i].z - pos.z) < 0.3f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void CreateAllEventObject(List<DungeonRoom> roomInfoList, GameObject eventObjPrefab)
    {
        for (int i = 0; i < roomInfoList.Count; i++)
        {
            for (int j = 0; j < roomInfoList[i].eventList.Count; j++)
            {
                var rndPos = new Vector3(evnetObjPos[i].transform.position.x, evnetObjPos[i].transform.position.y + 1f,
                    Random.Range(evnetObjPos[i].transform.position.z - 5, evnetObjPos[i].transform.position.z + 5));

                if(!PositionCheck(rndPos))
                {
                    j--;
                    continue;
                }
                
                posCheckList.Add(rndPos);

                var eventObj = Instantiate(eventObjPrefab, rndPos, Quaternion.identity);
                eventObj.GetComponent<EventObject>().Init(roomInfoList[i], roomInfoList[i].eventList[j], dungeonSystem, rndPos);

                eventObjList.Add(eventObj);
            }
        }
    }

    public void DestroyAllEventObject()
    {
        if (eventObjList.Count <= 0)
            return;
        for(int i=0; i< eventObjList.Count; i++)
        {
            Destroy(eventObjList[i]);
        }
        eventObjList.Clear();
    }
}
