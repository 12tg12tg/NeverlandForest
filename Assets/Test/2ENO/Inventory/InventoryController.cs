using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryController : GenericWindow
{
    [Header("InventoryClass")]
    [Space(3)]
    public InventoryItemView itemViewUI;
    public ItemClickMessage itemMessageUI;
    public GetItemView itemGetUI;

    public void Start()
    {
        Debug.Log("Awake");
        Init();
    }
    public override void Open()
    {
        base.Open();
        itemMessageUI.Close();
        itemGetUI.Close();
    }
    public override void Close()
    {
        base.Close();
        itemMessageUI.Close();
        itemGetUI.Close();
    }

    public void Init()
    {
        SetInventoryType();
    }

    public void SetInventoryType()
    { 
        var list = Vars.UserData.HaveAllItemList.ToList();
        itemViewUI.Init(list);
    }

    public void OpenClickMessageWindow(DataItem item)
    {
        Open();
        itemMessageUI.Open();
        itemMessageUI.Init(item);
        //manager.Open(1, true);
    }

    public void OpenChoiceMessageWindow(List<DataItem> itemList)
    {
        Open();
        itemGetUI.Open();
        itemGetUI.Init(itemList);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("ItemGet"))
        {
            var getItemList = new List<DataItem>();
            var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
            var stringId = "7";
            var newItem = new DataAllItem();
            newItem.itemTableElem = allItemTable.GetData<AllItemTableElem>(stringId);
            newItem.itemId = int.Parse(stringId);
            newItem.OwnCount = 13;
            newItem.LimitCount = 5;
            newItem.dataType = DataType.AllItem;

            var stringId2 = "6";
            var newItem2 = new DataAllItem();
            newItem2.itemTableElem = allItemTable.GetData<AllItemTableElem>(stringId2);
            newItem2.itemId = int.Parse(stringId2);
            newItem2.OwnCount = 7;
            newItem2.LimitCount = 5;
            newItem2.dataType = DataType.AllItem;

            getItemList.Add(newItem);
            getItemList.Add(newItem2);

            OpenChoiceMessageWindow(getItemList);
        }

        if(GUILayout.Button("DeleteItem"))
        {
            var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
            var stringId = "7";
            var newItem = new DataAllItem();
            newItem.itemTableElem = allItemTable.GetData<AllItemTableElem>(stringId);
            newItem.itemId = int.Parse(stringId);
            newItem.OwnCount = 6;
            newItem.LimitCount = 5;
            newItem.dataType = DataType.AllItem;

            Vars.UserData.RemoveItemData(newItem);

            var list = Vars.UserData.HaveAllItemList.ToList();
            itemViewUI.Init(list);
        }
    }
}
//List<DataAllItem> list = new List<DataAllItem>();
//foreach (var item in Vars.UserData.HaveAllItemList)
//{
//    list.Add(item);
//}


//public void GetItem(List<DataAllItem> items)
//{
//    itemViewUI.GetItemExcute(items);
//}

//public void DeleteItem(DataAllItem item)
//{
//    itemViewUI.DeleteItemExcute(item);
//}

//public void UserDataUpdate(List<DataAllItem> dataItem)
//{
//    for (int i = 0; i < dataItem.Count; i++)
//    {
//        if (!Vars.UserData.HaveAllItemList2.ContainsKey(dataItem[i].ItemTableElem.name))
//        {
//            Vars.UserData.HaveAllItemList2.Add(dataItem[i].ItemTableElem.name, dataItem[i]);
//        }
//        else
//        {
//            Vars.UserData.HaveAllItemList2[dataItem[i].ItemTableElem.name].OwnCount = dataItem[i].OwnCount;
//        }
//    }
//}

//public void UserDataItemRemove(DataAllItem item)
//{
//    if (Vars.UserData.HaveAllItemList2.ContainsKey(item.ItemTableElem.name))
//    {
//        Vars.UserData.HaveAllItemList2.Remove(item.ItemTableElem.name);
//    }
//}