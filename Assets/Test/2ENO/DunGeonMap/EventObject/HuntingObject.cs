using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HuntingObject : MonoBehaviour, IPointerClickHandler
{
    private DungeonSystem dungeonSystem;
    private EventData data;
    private int thisRoomIdx;
    public void Init(DungeonSystem system, EventData dt, int roomIdx)
    {
        this.dungeonSystem = system;
        data = dt;
        thisRoomIdx = roomIdx;

        var mesh = gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 이거 player가 사라져도 그 데이터는 잘 보존되는것인가? 테스트 필요 -> 잘 되는거 같다
        dungeonSystem.DungeonSystemData.curPlayerGirlData.SetUnitData(dungeonSystem.dungeonPlayerGirl);
        dungeonSystem.DungeonSystemData.curPlayerBoyData.SetUnitData(dungeonSystem.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystem.DungeonSystemData;

        dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
        dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

        SceneManager.LoadScene("AS_Hunting");
        Destroy(gameObject);
    }
}
