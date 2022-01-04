using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GatheringObject : MonoBehaviour, IPointerClickHandler
{
    private ConsumableTable consumableTable;
    public ConsumableTable ConsumableTable
    {
        get
        {
            return consumableTable;
        }
    }
    public DataConsumable item;
    private List<DataConsumable> userconsume;
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
    public int objId; //고유의 id
    public void OnPointerClick(PointerEventData eventData)
    {
        gathering.GoGatheringObject(gameObject.transform.position);
        gathering.curSelectedObj = this;
    }
    public void Init()
    {
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

        consumableTable = DataTableManager.GetTable<ConsumableTable>();
        userconsume = Vars.UserData.ConsumableItemList;

        //처음 수집씬에 들어오면 랜덤으로 아이템 하나를 들고 시작함
        rand = Random.Range(1, 4);
        id = $"CON_000{rand}";
        item = new DataConsumable();
        item.itemId = rand;
        item.dataType = DataType.Consume;
        item.itemTableElem = consumableTable.GetData<ConsumableTableElem>(id);
       
    }
    private void Awake()
    {
       

    }
    // Start is called before the first frame update
    void Start()
    {
      
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
