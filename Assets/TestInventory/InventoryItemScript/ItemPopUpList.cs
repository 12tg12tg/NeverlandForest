using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopUpList : MonoBehaviour
{
    public UIItemInventory itemInventory;

    public ItemObject itemPrefab;
    private List<ItemObject> itemGoList = new List<ItemObject>();

    public ScrollRect scrollRect;

    public bool isDelete = false;

    private void Awake()
    {
        for (var i = 0; i < UIInventoryItemList.MaxItemCount; ++i)
        {
            var item = Instantiate(itemPrefab, scrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.SetActive(false);
            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickAddItem(item.DataItem));
        }
    }

    public void init()
    {
        var weaponTable = DataTableMgr.GetTable<WeaponTable>();
        var consumableTable = DataTableMgr.GetTable<ItemTable>();

        List<DataWeapon> weaponList = new List<DataWeapon>();
        List<DataCunsumable> consumList = new List<DataCunsumable>();
        for (int i = 1; i <= weaponTable.Data.Count; i++)
        {
            var newWeapon = new DataWeapon();
            newWeapon.itemId = i;
            var randId = $"WEA_000{i}";
            newWeapon.itemTableElem = weaponTable.GetData<WeaponTableElem>(randId);
            weaponList.Add(newWeapon);
        }

        for (int i = 1; i <= consumableTable.Data.Count; i++)
        {
            var newItem = new DataCunsumable();
            newItem.itemId = i;
            var randId = $"CON_000{i}";
            newItem.itemTableElem = consumableTable.GetData<ItemTableElem>(randId);
            consumList.Add(newItem);
        }

        foreach(var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        for(var i = 0; i < weaponList.Count; i++)
        {
            itemGoList[i].gameObject.SetActive(true);
            itemGoList[i].Init(weaponList[i]);
        }
        for(var i = weaponList.Count; i < weaponList.Count + consumList.Count; i++)
        {
            itemGoList[i].gameObject.SetActive(true);
            itemGoList[i].Init(consumList[i- weaponList.Count]);
        }
    }

    // 삭제 추가 동시?
    public void OnClickAddItem(DataItem item)
    {
        if (!isDelete)
        {
            if (item as DataWeapon != null)
            {
                Vars.UserData.weaponItemList.Add(item as DataWeapon);
            }
            else if (item as DataCunsumable != null)
            {
                Vars.UserData.consumableItemList.Add(item as DataCunsumable);
            }
        }
        else
        {
            if (item as DataWeapon != null)
            {
                var tempItem = item as DataWeapon;
                var idx = Vars.UserData.weaponItemList.FindLastIndex( x => x.itemTableElem.id == tempItem.itemTableElem.id);
                Vars.UserData.weaponItemList.RemoveAt(idx);
            }
            else if (item as DataCunsumable != null)
            {
                var tempItem = item as DataCunsumable;
                var idx = Vars.UserData.consumableItemList.FindLastIndex(x => x.itemTableElem.id == tempItem.itemTableElem.id);
                Vars.UserData.consumableItemList.RemoveAt(idx);
            }
        }

        itemInventory.SetInventoryType(itemInventory.CurrentInventory);
        gameObject.SetActive(false);
    }
}
