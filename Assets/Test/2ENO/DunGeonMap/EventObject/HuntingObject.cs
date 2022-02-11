using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HuntingObject : MonoBehaviour, IPointerClickHandler
{
    private EventData data;
    private int thisRoomIdx;
    GameObject popUpWindow;
    public void Init(EventData dt, int roomIdx, GameObject popUp)
    {
        data = dt;
        thisRoomIdx = roomIdx;
        popUpWindow = popUp;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DungeonSystem.Instance.DungeonSystemData.curDungeonRoomData.roomIdx == thisRoomIdx)
        {
            popUpWindow.SetActive(true);
            var button = popUpWindow.GetComponentsInChildren<Button>();
            button[0].onClick.AddListener(() => { HuntingStart(); button[0].onClick.RemoveAllListeners(); } );
            //button[1].onClick.AddListener(() => {HuntingStart(); button[1].onClick.RemoveAllListeners();});
        }
    }

    public void HuntingStart()
    {
        DungeonSystem.Instance.DungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
        DungeonSystem.Instance.DungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = DungeonSystem.Instance.DungeonSystemData;
        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);

        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);
        GameManager.Manager.LoadScene(GameScene.Hunt);
        Destroy(gameObject);
    }
}
