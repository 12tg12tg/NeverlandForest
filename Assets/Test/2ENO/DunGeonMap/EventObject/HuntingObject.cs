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
        // �̰� player�� ������� �� �����ʹ� �� �����Ǵ°��ΰ�? �׽�Ʈ �ʿ� -> �� �Ǵ°� ����
        dungeonSystem.DungeonSystemData.curPlayerData.SetUnitData(dungeonSystem.dungeonPlayer);
        Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystem.DungeonSystemData;

        dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
        dungeonSystem.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

        SceneManager.LoadScene("AS_Hunting");
        Destroy(gameObject);
    }
}
