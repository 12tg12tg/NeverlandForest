using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GatheringObjs : MonoBehaviour, IPointerClickHandler
{
   

    private AllItemDataTable allitemTable;
    public AllItemDataTable AllItemTable
    {
        get
        {
            return allitemTable;
        }
    }
    public DataAllItem item;
    private List<DataAllItem> allitemlist;
    public GatheringObjectType curobjectType;
    private int rand;
    public void OnPointerClick(PointerEventData eventData)
    {
      
    }
    public void Init()
    {
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        //처음 수집씬에 들어오면 랜덤으로 아이템 하나를 들고 시작함
        var newItem = new DataAllItem();
        rand = Random.Range(0, 4);
        newItem.itemId = rand;
        newItem.LimitCount = 5;
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{rand}";
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);
        item = newItem;
        GameObject obj;
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
