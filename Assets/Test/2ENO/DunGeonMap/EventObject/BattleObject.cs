using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleObject : MonoBehaviour
{
    private DungeonSystem dungeonSystem;
    private EventData data;
    private int thisRoomIdx;
    public void Init(DungeonSystem system, EventData dt, int roomIdx)
    {
        this.dungeonSystem = system;
        data = dt;
        thisRoomIdx = roomIdx;
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
            //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
            SceneManager.LoadScene("JYK_Test_Battle");
            Destroy(gameObject);
        }
    }
}