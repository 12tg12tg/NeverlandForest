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

    private void OnTriggerExit(Collider other)
    {
        SoundManager.Instance.PlayWalkSound(false);

        if (other.tag is "Player")
        {
            var dungeonSystem = DungeonSystem.Instance;
            dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
            dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

            var randEventMgr = RandomEventManager.Instance;

            if (!randEventMgr.isRandomEventTutorialExcute)
            {
                var rndEvent = randEventMgr.tutorialEvent;
                RandomEventUIManager.Instance.EventInit(rndEvent);

                dungeonSystem.randomEventTutorial.StartRandomEventTutorial();
            }
            else
            {
                var rndEvent = randEventMgr.GetEventData(randomEventID);
                RandomEventUIManager.Instance.EventInit(rndEvent);
            }
            //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);


            Destroy(gameObject);
        }
    }
}