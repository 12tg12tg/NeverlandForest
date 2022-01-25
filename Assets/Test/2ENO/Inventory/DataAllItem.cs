using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataAllItem
{
    public string itemId;

    public DataAllItem(AllItemTableElem itemElem)
    {
        itemId = itemElem.id;
        itemTableElem = itemElem;
    }
    public DataAllItem(DataAllItem item)
    {
        OwnCount = item.OwnCount;
        itemTableElem = item.itemTableElem;
        itemId = item.itemId;
    }

    public int OwnCount { get; set; }
    public int GetCount { get; set; }

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
//public DataAllItem(DataItem item) : base(item) { }
