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
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        rand = Random.Range(0, 4);
        var stringId = $"ITEM_{rand}";
        var newItem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringId));
        newItem.OwnCount = Random.Range(1, 3);
        item = newItem;
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
