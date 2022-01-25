using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMaterial : DataItem
{
    public DataMaterial() : base() { }
    public DataMaterial(DataMaterial item)
    {
        //this.OwnCount = item.OwnCount;
        //this.LimitCount = item.LimitCount;
        //this.itemTableElem = item.itemTableElem;
        //this.itemId = item.itemId;
    }
    public AllItemTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as AllItemTableElem;
        }
    }
}
