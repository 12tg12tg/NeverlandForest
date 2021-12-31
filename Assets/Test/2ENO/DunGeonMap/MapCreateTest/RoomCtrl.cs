using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCtrl : MonoBehaviour
{
    public GameObject spawnPos;
    public GameObject[] evnetObjPos;
    //public GameObject eventObjPrefab;

    private List<Vector3> posCheckList = new List<Vector3>();
    public List<GameObject> eventObjList = new List<GameObject>();

    public DungeonSystem dungeonSystem;

    private void OnEnable()
    {
        dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();

        var childEnd = gameObject.GetComponentsInChildren<EndPos>();

        for (int i = 0; i < childEnd.Length; i++)
        {
            if (i == childEnd.Length - 1)
            {
                childEnd[i].isLastPos = true;
            }
        }
    }

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
                var rndPos = new Vector3(evnetObjPos[i].transform.position.x, evnetObjPos[i].transform.position.y,
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

//public void EventObjectSet(List<DunGeonEvent> eventList, bool isMain)
//{
//    if(isMain)
//    {
//    }
//    else
//    {
//        for (int i = 0; i < eventList.Count; i++)
//        {
//            for (int j = 0; j <= (int)DunGeonEvent.SubStory;)
//            {
//                if((eventList[i] & (DunGeonEvent)j) != 0)
//                {
//                    var pos = roomPos[i].getPosition();
//                    var vecPos = new Vector3(Random.Range(pos.x - 5, pos.x - 3), 2f, Random.Range(pos.z - 4, pos.z + 4));

//                    if((DunGeonEvent)j != DunGeonEvent.Empty)
//                    {
//                        var obj = Instantiate(eventObj, vecPos, Quaternion.identity);
//                        var mesh = obj.GetComponent<MeshRenderer>();
//                        objList.Add(obj);
//                        switch ((DunGeonEvent)j)
//                        {
//                            case DunGeonEvent.Battle:
//                                mesh.material.color = Color.red;
//                                break;
//                            case DunGeonEvent.Gathering:
//                                mesh.material.color = Color.green;
//                                break;
//                            case DunGeonEvent.Hunt:
//                                mesh.material.color = Color.blue;
//                                break;
//                            case DunGeonEvent.RandomIncount:
//                                mesh.material.color = Color.black;
//                                break;
//                            case DunGeonEvent.SubStory:
//                                mesh.material.color = Color.grey;
//                                break;
//                            case DunGeonEvent.Count:
//                                break;
//                        }
//                    }
//                }
//                if (j == 0)
//                    j++;
//                else
//                    j <<= 1;
//            }
//        }
//    }
//}
