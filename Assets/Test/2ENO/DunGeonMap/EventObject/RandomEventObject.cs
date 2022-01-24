using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventObject : MonoBehaviour
{
    private EventData data;
    private int thisRoomIdx;
    private string randomEventID;

    public void Init(EventData dt, int roomIdx, string eventID)
    {
        data = dt;
        thisRoomIdx = roomIdx;
        randomEventID = eventID;
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag is "Player")
        {
            DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
            DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);
            var rndEvent = RandomEventManager.Instance.GetEventData(randomEventID);
            RandomEventUIManager.Instance.EventInit(rndEvent);

            //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
            Destroy(gameObject);
        }
    }
}
