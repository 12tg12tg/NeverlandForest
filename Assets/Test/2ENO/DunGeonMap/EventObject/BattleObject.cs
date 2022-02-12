using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleObject : MonoBehaviour
{
    private EventData data;
    private int thisRoomIdx;
    public void Init(EventData dt, int roomIdx)
    {
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
            var dungeonSystemData = DungeonSystem.Instance.DungeonSystemData;

            dungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
            dungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
            Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;

            dungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
            dungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);

            if(thisRoomIdx == Vars.UserData.dungeonLastIdx)
                BattleManager.initState = BattleInitState.Dungeon;
            else
                BattleManager.initState = BattleInitState.None;

            GameManager.Manager.LoadScene(GameScene.Battle);
            Destroy(gameObject);
        }
    }
}