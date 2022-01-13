using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GatheringObject : MonoBehaviour, IPointerClickHandler
{
    public EventData data;
    public int roomIndex;
    public DungeonSystem dungeonSystem;
    private AllItemDataTable allitemTable;

    public enum GatheringObjectType
    {
        Tree,
        Pit, //������
        Herbs, //����
        Mushroom, //����
    }
    public GatheringObjectType objectType = GatheringObjectType.Tree; //������ excel���������� �޾ƿ;��Ѵ�.
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

        dungeonSystem.DungeonSystemData.dungeonRoomArray[roomIndex].UseEvent(DunGeonEvent.Gathering);
        dungeonSystem.DungeonSystemData.dungeonRoomArray[roomIndex].eventObjDataList.Remove(data);
        dungeonSystem.DungeonSystemData.dungeonRoomArray[roomIndex].gatheringCount--;
        //Destroy(gameObject);
    }

    // TODO: ����� �ڵ� 2�� �ϳ��� ����
    public void Init()
    {
        var mesh = gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.green;
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        var newItem = new DataAllItem();
        rand = Random.Range(1, 4);
        newItem.itemId = rand;
        newItem.LimitCount = 5;
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{rand}";
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);

        item = newItem;
    }
    public void Init(GatheringSystem system, EventData dt, DungeonSystem dgSystem, int thisRoomIdx)
    {
        dungeonSystem = dgSystem;
        data = dt;
        roomIndex = thisRoomIdx;

        var mesh = gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.green;
        gathering = system;
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        var newItem = new DataAllItem();
        rand = Random.Range(1, 4);
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
