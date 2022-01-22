using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventObject : MonoBehaviour
{
    private DungeonSystem dungeonSystem;
    private EventData data;
    private int thisRoomIdx;
    private string randomEventID;

    public void Init(DungeonSystem system, EventData dt, int roomIdx, string eventID)
    {
        this.dungeonSystem = system;
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
            dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
            dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);
            var rndEvent = RandomEventManager.Instance.GetEventData(randomEventID);
            RandomEventUIManager.Instance.EventInit(rndEvent);

            //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
            Destroy(gameObject);
        }
    }
}
