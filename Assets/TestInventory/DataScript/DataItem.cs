using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DataType
{
    Default,
    Weapon,
    Consume,
    Armor,
}
public abstract class DataItem
{
    public int itemId;
    public DataType dataType;
    public DataTableElemBase itemTableElem;

    //public abstract DataTableElemBase ItemTableElem { get; }
}
