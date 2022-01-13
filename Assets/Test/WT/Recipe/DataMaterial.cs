using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMaterial : DataItem
{
    public DataMaterial() : base() { }
    public DataMaterial(DataMaterial item) : base(item)
    {
        //this.OwnCount = item.OwnCount;
        //this.LimitCount = item.LimitCount;
        //this.itemTableElem = item.itemTableElem;
        //this.itemId = item.itemId;
    }
    public DataMaterial(DataItem item) : base(item) { }
   
    public AllItemTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as AllItemTableElem;
        }
    }
}
