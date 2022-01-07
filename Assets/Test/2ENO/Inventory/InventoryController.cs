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
    public void Start()
    {
        SetInventoryType();
        itemMessageUI.Close();
    }

    public void SetInventoryType()
    {

        List<DataAllItem> list = new List<DataAllItem>();
        //list = Vars.UserData.HaveAllItemList2.Cast<DataItem>().ToList();

        foreach (var item in Vars.UserData.HaveAllItemList2)
        {
            list.Add(item.Value);
        }

        itemViewUI.Init(list);
    }

    public void OpenMessageWindow(DataAllItem item)
    {
        itemMessageUI.Close();
        itemMessageUI.Open();
        //manager.Open(1, true);
        itemMessageUI.Init(item);
    }

    public void GetItem(List<DataAllItem> items)
    {
        itemViewUI.GetItemExcute(items);
    }

    public void DeleteItem(DataAllItem item)
    {
        itemViewUI.DeleteItemExcute(item);
    }

    public void UserDataUpdate(List<DataAllItem> dataItem)
    {
        for (int i = 0; i < dataItem.Count; i++)
        {
            if (!Vars.UserData.HaveAllItemList2.ContainsKey(dataItem[i].ItemTableElem.name))
            {
                Vars.UserData.HaveAllItemList2.Add(dataItem[i].ItemTableElem.name, dataItem[i]);
            }
            else
            {
                Vars.UserData.HaveAllItemList2[dataItem[i].ItemTableElem.name].OwnCount = dataItem[i].OwnCount;
            }
        }
    }
    public void UserDataItemRemove(DataAllItem item)
    {
        if (Vars.UserData.HaveAllItemList2.ContainsKey(item.ItemTableElem.name))
        {
            Vars.UserData.HaveAllItemList2.Remove(item.ItemTableElem.name);
        }
    }

    private void OnGUI()
    {
        
        if (GUILayout.Button("ItemGet"))
        {
            var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
            var stringId = "7";
            var newItem = new DataAllItem();
            newItem.itemTableElem = allItemTable.GetData<AllItemTableElem>(stringId);
            newItem.itemId = int.Parse(stringId);
            newItem.OwnCount = 13;
            newItem.LimitCount = 5;

            var itemList = new List<DataAllItem>();
            itemList.Add(newItem);

            GetItem(itemList);
            // È¹µæ °¡´ÉÇÑ Ã¢ ¶ç¿ì±â
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

            DeleteItem(newItem);
        }
    }

    
    
    
}
