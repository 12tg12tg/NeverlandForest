using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum GatheringObjectType
{
    Tree,
    Pit, //������
    Herbs, //����
    Mushroom, //����
}
public class GatheringObject : MonoBehaviour, IPointerClickHandler
{
    private AllItemDataTable allitemTable;
    private List<DataAllItem> allitemlist;
    private Vector3 weedPos;
    public EventData data;
    [Header("������")]
    public int roomIndex;
    [Header("�����۰���")]
    public GatheringObjectType objectType;
    public DataAllItem item;
    public DataAllItem subitem;
    public GatheringSystem gathering;
    public int objId; //������ id
    public AllItemDataTable AllItemTable
    {
        get
        {
            return allitemTable;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (GameManager.Manager.State)
        {
            case GameState.Dungeon:
                if (DungeonSystem.Instance.DungeonSystemData.curDungeonRoomData.roomIdx == roomIndex)
                {
                    gathering.GoGatheringObject(gameObject.transform.position);
                    gathering.curSelectedObj = this;

                    DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[roomIndex].UseEvent(DunGeonEvent.Gathering);
                    DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[roomIndex].eventObjDataList.Remove(data);
                    DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[roomIndex].gatheringCount--;
                }
                break;
            case GameState.Tutorial:
                DungeonSystem.Instance.gatherTutorial.TutorialStep++;
                gathering.curSelectedObj = this;
                break;
            default:
                break;
        }
    }
    public void Init()
    {
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        switch (objectType)
        {
            case GatheringObjectType.Tree:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{1}"));
                item.OwnCount = Random.Range(1, 3);
                subitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{2}"));
                subitem.OwnCount = Random.Range(1, 3);
                break;
            case GatheringObjectType.Pit:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{3}"));
                item.OwnCount = Random.Range(3, 5);
                subitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{3}"));
                subitem.OwnCount = Random.Range(1, 3);
                break;
            case GatheringObjectType.Herbs:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{4}"));
                item.OwnCount = Random.Range(1, 3);
                subitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{5}"));
                subitem.OwnCount = Random.Range(1, 3);
                break;
            case GatheringObjectType.Mushroom:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{6}"));
                item.OwnCount = Random.Range(1, 3);
                break;
            default:
                break;
        }
       
    }
    public void Init(GatheringSystem system, EventData dt, int thisRoomIdx)
    {
        data = dt;
        roomIndex = thisRoomIdx;

        gathering = system;
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        switch (objectType)
        {
            case GatheringObjectType.Tree:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{1}"));
                item.OwnCount = Random.Range(1, 3);
                subitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{2}"));
                subitem.OwnCount = Random.Range(1, 3);
                break;
            case GatheringObjectType.Pit:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{3}"));
                item.OwnCount = Random.Range(3, 5);
                subitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{3}"));
                subitem.OwnCount = Random.Range(1, 3);
                break;
            case GatheringObjectType.Herbs:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{4}"));
                item.OwnCount = Random.Range(1, 3);
                subitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{5}"));
                subitem.OwnCount = Random.Range(1, 3);
                break;
            case GatheringObjectType.Mushroom:
                item = new DataAllItem(allitemTable.GetData<AllItemTableElem>($"ITEM_{6}"));
                item.OwnCount = Random.Range(1, 3);
                break;
            default:
                break;
        }
    }
    public void Appear()
    {
        gameObject.SetActive(true);
    }
    public void Disappear()
    {
        gameObject.SetActive(false);
    }
}
