public enum DataType
{
    Default,
    Consume,
    AllItem,
}
public abstract class DataItem
{
    public int itemId = 0;
    public DataType dataType;
    public DataTableElemBase itemTableElem;
    public int OwnCount { get; set; }
    public int LimitCount { get; set; }

    public DataItem() { }
    public DataItem(DataItem item)
    {
        this.itemId = item.itemId;
        this.dataType = item.dataType;
        this.itemTableElem = item.itemTableElem;
        this.OwnCount = item.OwnCount;
        this.LimitCount = item.LimitCount;
    }

    public int InvenFullCount => (OwnCount / LimitCount);

    public int InvenRemainCount => (OwnCount % LimitCount);



    //public abstract DataTableElemBase ItemTableElem { get; }
}
