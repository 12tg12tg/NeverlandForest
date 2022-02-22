using System.Collections.Generic;

public class ItemListSaveData_0 : SaveDataBase
{
    public List<string> itemIdList = new List<string>();
    public List<int> itemOwnCountList = new List<int>();
    public List<int> itemRemainCountList = new List<int>();
}

public class DataAllItem
{
    public string itemId;

    // ���� ������ ������ ���̺��� �޾ƿ���
    public DataAllItem(AllItemTableElem itemElem)
    {
        itemId = itemElem.id;
        itemTableElem = itemElem;
    }
    // elem ��������� �ٲ�
    public DataAllItem(DataAllItem item)
    {
        OwnCount = item.OwnCount;
        ToolCount = item.itemTableElem.tool_remainCount;
        var elem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(item.itemTableElem.id);
        itemTableElem = elem;
        itemId = item.itemId;
    }

    public int OwnCount { get; set; }
    public int ToolCount { get; set; }

    private AllItemTableElem itemTableElem;
    public AllItemTableElem ItemTableElem
    {
        get => itemTableElem;
        set => itemTableElem = value;
    }
    public int InvenFullCount
    {
        get
        {
            if (OwnCount == 0 || ItemTableElem.limitCount == 0)
                return 0;
            else
            {
                return (OwnCount / ItemTableElem.limitCount);
            }
        }
    }
    public int InvenRemainCount
    {
        get
        {
            if (OwnCount == 0 || ItemTableElem.limitCount == 0)
                return 0;
            else
            {
                return (OwnCount % ItemTableElem.limitCount);
            }
        }
    }
}
