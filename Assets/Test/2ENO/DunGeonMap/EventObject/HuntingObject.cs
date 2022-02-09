using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HuntingObject : MonoBehaviour, IPointerClickHandler
{
    private EventData data;
    private int thisRoomIdx;
    public void Init(EventData dt, int roomIdx)
    {
        data = dt;
        thisRoomIdx = roomIdx;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DungeonSystem.Instance.DungeonSystemData.curDungeonRoomData.roomIdx == thisRoomIdx)
        {
            DungeonSystem.Instance.DungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
            DungeonSystem.Instance.DungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
            Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = DungeonSystem.Instance.DungeonSystemData;

            DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
            DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

            GameManager.Manager.LoadScene(GameScene.Hunt);
            Destroy(gameObject);
        }
    }
}
