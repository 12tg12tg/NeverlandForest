using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataAllItem : DataItem
{
    public DataAllItem(): base() { }
    public DataAllItem(DataAllItem item) : base(item)
    {
        //this.OwnCount = item.OwnCount;
        //this.LimitCount = item.LimitCount;
        //this.itemTableElem = item.itemTableElem;
        //this.itemId = item.itemId;
    }
    public DataAllItem(DataItem item) : base(item) { }

    public AllItemTableElem ItemTableElem
    {
        get => itemTableElem as AllItemTableElem;
    }
}
