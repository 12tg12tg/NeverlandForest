using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCunsumable : DataItem
{
    public int count;

    public ConsumableTableElemInho ItemTableElem
    {
        get
        {
            return itemTableElem as ConsumableTableElemInho;
        }
    }
}

