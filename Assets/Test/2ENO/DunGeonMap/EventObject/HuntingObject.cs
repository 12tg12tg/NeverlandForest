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

        var mesh = gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // �̰� player�� ������� �� �����ʹ� �� �����Ǵ°��ΰ�? �׽�Ʈ �ʿ� -> �� �Ǵ°� ����
        DungeonSystem.Instance.DungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
        DungeonSystem.Instance.DungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = DungeonSystem.Instance.DungeonSystemData;

        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].UseEvent(data.eventType);
        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[thisRoomIdx].eventObjDataList.Remove(data);

        SceneManager.LoadScene("AS_Hunting");
        Destroy(gameObject);
    }
}
