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
        var dungeonSystemData = DungeonSystem.Instance.DungeonSystemData;
        dungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
        dungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);

        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystemData;

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);

        dungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
        dungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

        SoundManager.Instance.Play(SoundType.Se_Button);
        GameManager.Manager.LoadScene(GameScene.Hunt);
        Destroy(gameObject);
    }
}
