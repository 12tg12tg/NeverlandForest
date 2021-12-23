using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMaterial : DataItem
{
    public int count;
    public AllItemTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as AllItemTableElem;
        }
    }
}
