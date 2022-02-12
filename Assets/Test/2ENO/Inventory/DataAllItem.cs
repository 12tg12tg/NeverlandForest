using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemListSaveData_0
{
    public List<string> itemIdList = new List<string>();
    public List<int> itemOwnCountList = new List<int>();
}

public class DataAllItem
{
    public string itemId;

    // 사용시 무조건 데이터 테이블에서 받아오기
    public DataAllItem(AllItemTableElem itemElem)
    {
        itemId = itemElem.id;
        itemTableElem = itemElem;
    }
    // elem 깊은복사로 바꿈
    public DataAllItem(DataAllItem item)
    {
        OwnCount = item.OwnCount;
        var elem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(item.itemTableElem.id);
        itemTableElem = elem;
        itemId = item.itemId;
    }

    public int OwnCount { get; set; }

    private AllItemTableElem itemTableElem;
    public AllItemTableElem ItemTableElem
    {
        get => itemTableElem;
        set => itemTableElem = value;
    }
    public int InvenFullCount
    {
        get
        {
            if (OwnCount == 0 || ItemTableElem.limitCount == 0)
                return 0;
            else
            {
                return (OwnCount / ItemTableElem.limitCount);
            }
        }
    }
    public int InvenRemainCount
    {
        get
        {
            if (OwnCount == 0 || ItemTableElem.limitCount == 0)
                return 0;
            else
            {
                return (OwnCount % ItemTableElem.limitCount);
            }
        }
    }
}
