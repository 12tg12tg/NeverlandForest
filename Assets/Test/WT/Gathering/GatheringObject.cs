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
    public EventData data;
    public int roomIndex;
    private AllItemDataTable allitemTable;

    public GatheringObjectType objectType;
    public AllItemDataTable AllItemTable
    {
        get
        {
            return allitemTable;
        }
    }
    public DataAllItem item;
    private List<DataAllItem> allitemlist;
    private Vector3 weedPos;
   
    public GatheringSystem gathering;
   /* private int index;
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }*/
    private string id;
    private int rand;
    public int objId; //������ id
    public void OnPointerClick(PointerEventData eventData)
    {
        gathering.GoGatheringObject(gameObject.transform.position);
        gathering.curSelectedObj = this;

        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[roomIndex].UseEvent(DunGeonEvent.Gathering);
        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[roomIndex].eventObjDataList.Remove(data);
        DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[roomIndex].gatheringCount--;
        //Destroy(gameObject);
    }
    public void Init()
    {
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var newItem = new DataAllItem();
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        switch (objectType)
        {
            case GatheringObjectType.Tree:
                rand = 0;
                break;
            case GatheringObjectType.Pit:
                rand = 1;
                break;
            case GatheringObjectType.Herbs:
                rand = 2;
                break;
            case GatheringObjectType.Mushroom:
                rand = 3;
                break;
            default:
                break;
        }
        newItem.itemId = rand;
        newItem.LimitCount = 5;
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{rand}";
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);
        item = newItem;
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
        var newItem = new DataAllItem();
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        switch (objectType)
        {
            case GatheringObjectType.Tree:
                rand = 0;
                break;
            case GatheringObjectType.Pit:
                rand = 1;
                break;
            case GatheringObjectType.Herbs:
                rand = 2;
                break;
            case GatheringObjectType.Mushroom:
                rand = 3;
                break;
            default:
                break;
        }
        newItem.itemId = rand;
        newItem.LimitCount = 5;
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{rand}";
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);
        item = newItem;
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
