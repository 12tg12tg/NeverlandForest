using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemPopUpList : MonoBehaviour
{
    public UIItemInventory itemInventory;

    public ItemObject itemPrefab;
    private List<ItemObject> itemGoList = new List<ItemObject>();
    public List<DataItem> itemList = new List<DataItem>();

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
            button.onClick.AddListener(() => OnClickSetItem(item.DataItem, button));
            //button.onClick.AddListener(() => OnClickAddDeleteItem(item.DataItem));
        }
    }

    // 유저 데이터에 있는 모든 아이템을 init
    public void init2(List<DataItem> list)
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        for (var i = 0; i < list.Count; i++)
        {
            itemGoList[i].gameObject.SetActive(true);

            switch (list[i].dataType)
            {
                case DataType.Default:
                    break;
                case DataType.Weapon:
                    itemGoList[i].Init(list[i] as DataWeapon);
                    break;
                case DataType.Consume:
                    itemGoList[i].Init(list[i] as DataCunsumable);
                    break;
                case DataType.Armor:
                    break;
            }
        }

        itemList.Clear();
    }

    // 아이템 종류별 1개씩 init
    public void init(List<DataItem> list)
    {
        foreach(var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        for(var i = 0; i< list.Count; i++)
        {
            itemGoList[i].gameObject.SetActive(true);

            switch (list[i].dataType)
            {
                case DataType.Default:
                    break;
                case DataType.Weapon:
                    itemGoList[i].Init(list[i] as DataWeapon);
                    break;
                case DataType.Consume:
                    itemGoList[i].Init(list[i] as DataCunsumable);
                    break;
                case DataType.Armor:
                    break;
            }
        }
    }

    // 버튼 선택중 표시 방법 모르겠다
    public void OnClickSetItem(DataItem item, Button btn)
    {
        var idx = itemList.FindIndex(x => x.itemId == item.itemId);
        if ( idx != -1)
        {
            itemList.RemoveAt(idx);
        }
        else
        {
            itemList.Add(item);
        }
    }

    public void OnClickConfirm()
    {
        if (isDelete)
        {
            foreach (var item in itemList)
            {
                switch (item.dataType)
                {
                    case DataType.Default:
                        break;
                    case DataType.Weapon:
                        var tempItem = item as DataWeapon;
                        var idx = Vars.UserData.weaponItemList.FindLastIndex(x => x.itemId == tempItem.itemId);
                        Vars.UserData.weaponItemList.RemoveAt(idx);
                        break;
                    case DataType.Consume:
                        var tempItem2 = item as DataCunsumable;
                        var idx2 = Vars.UserData.consumableItemList.FindLastIndex(x => x.itemId == tempItem2.itemId);
                        Vars.UserData.consumableItemList.RemoveAt(idx2);
                        break;
                    case DataType.Armor:
                        break;
                }
            }
        }
        else
        {
            foreach (var item in itemList)
            {
                switch (item.dataType)
                {
                    case DataType.Default:
                        break;
                    case DataType.Weapon:
                        var tempItem = item as DataWeapon;
                        tempItem.itemId = Vars.UserData.weaponItemList[Vars.UserData.weaponItemList.Count - 1].itemId + 1;
                        Vars.UserData.weaponItemList.Add(tempItem);
                        break;
                    case DataType.Consume:
                        var tempItem2 = item as DataCunsumable;
                        tempItem2.itemId = Vars.UserData.consumableItemList[Vars.UserData.consumableItemList.Count - 1].itemId + 1;
                        Vars.UserData.consumableItemList.Add(item as DataCunsumable);
                        break;
                    case DataType.Armor:
                        break;
                }
            }
        }
        itemInventory.SetInventoryType(itemInventory.CurrentInventory);
        gameObject.SetActive(false);
    }

    // 삭제 추가 동시?
    // 아이템 정보를 Inventory에 넘겨주고 직접 추가, 삭제는 그쪽에서 하도록 수정
    // 잠시 대기
    public void OnClickAddDeleteItem(DataItem item)
    {
        if (!isDelete)
        {
            switch (item.dataType)
            {
                case DataType.Default:
                    break;
                case DataType.Weapon:
                    Vars.UserData.weaponItemList.Add(item as DataWeapon);
                    break;
                case DataType.Consume:

                    Vars.UserData.consumableItemList.Add(item as DataCunsumable);
                    break;
                case DataType.Armor:
                    break;
            }
        }
        else
        {
            switch (item.dataType)
            {
                case DataType.Default:
                    break;
                case DataType.Weapon:
                    var tempItem = item as DataWeapon;
                    var idx = Vars.UserData.weaponItemList.FindLastIndex(x => x.itemTableElem.id == tempItem.itemTableElem.id);
                    Vars.UserData.weaponItemList.RemoveAt(idx);
                    break;
                case DataType.Consume:
                    var tempItem2 = item as DataCunsumable;
                    var idx2 = Vars.UserData.consumableItemList.FindLastIndex(x => x.itemTableElem.id == tempItem2.itemTableElem.id);
                    Vars.UserData.consumableItemList.RemoveAt(idx2);
                    break;
                case DataType.Armor:
                    break;
            }
        }

        itemInventory.SetInventoryType(itemInventory.CurrentInventory);
        gameObject.SetActive(false);
    }
}
