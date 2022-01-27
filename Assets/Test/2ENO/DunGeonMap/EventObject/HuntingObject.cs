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
        // 이거 player가 사라져도 그 데이터는 잘 보존되는것인가? 테스트 필요 -> 잘 되는거 같다
        DungeonSystem.Instance.DungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
        DungeonSystem.Instance.DungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = DungeonSystem.Instance.DungeonSystemData;

        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

        SceneManager.LoadScene("AS_Hunting");
        Destroy(gameObject);
    }
}
