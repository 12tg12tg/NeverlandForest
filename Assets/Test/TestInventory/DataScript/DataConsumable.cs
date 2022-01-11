using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataConsumable : DataItem
{
    public DataConsumable() : base() { }
    public DataConsumable(DataConsumable item) : base(item) { }
    public DataConsumable(DataItem item) : base(item) { }

    public ConsumableTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as ConsumableTableElem;
        }
    }
}

