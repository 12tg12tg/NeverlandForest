using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DataConsumable : DataItem
{
    public int count;
    
    public ConsumableTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as ConsumableTableElem;
        }
    }
}

