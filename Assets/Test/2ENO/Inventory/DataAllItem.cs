using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataAllItem : DataItem
{
    public DataAllItem() { }
    public DataAllItem(DataAllItem item)
    {
        this.OwnCount = item.OwnCount;
        this.LimitCount = item.LimitCount;
        this.itemTableElem = item.itemTableElem;
        this.itemId = item.itemId;
    }

    public int OwnCount { get; set; }
    // 엑셀 테이블에 정의해야될 내용이지만 일단 여기에 구현
    public int LimitCount { get; set; }
    public AllItemTableElem ItemTableElem
    {
        get => itemTableElem as AllItemTableElem;
    }
}
