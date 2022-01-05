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
    // ���� ���̺� �����ؾߵ� ���������� �ϴ� ���⿡ ����
    public int LimitCount { get; set; }
    public AllItemTableElem ItemTableElem
    {
        get => itemTableElem as AllItemTableElem;
    }
}
