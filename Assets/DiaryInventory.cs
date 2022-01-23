using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiaryInventory : MonoBehaviour
{

    public enum ButtonState { Item, Skill }

    //Instance
    public BottomInfoUI info;
    public List<BottomItemButtonUI> itemButtons;
    private static DiaryInventory instance;
    public static DiaryInventory Instance => instance;

    //Vars
    [HideInInspector] public ButtonState buttonState;

    private void Awake()
    {
        ItemButtonInit();
        instance = this;
    }

    public void ItemButtonInit()
    {
        info.Init();
        buttonState = ButtonState.Item;
        itemButtons.ForEach((n) => n.gameObject.SetActive(true));
        ItemListInit();
    }

    private void ItemListInit()
    {
        itemButtons.ForEach(n => n.Init(null));
        // 이거 Create할때 임시리스트 생성해서 사용중이긴 한데 자주 호출됬을때 좀 찝찝할수도
        var list = CreateDivideItemList(Vars.UserData.HaveAllItemList.ToList());
        SortItemList(list);
        int count = list.Count;

        for (int i = 0; i < count; i++)
        {
            switch (list[i].dataType)
            {
                case DataType.Consume:
                    break;
                case DataType.AllItem:
                    itemButtons[i].Init(list[i] as DataAllItem);
                    break;
                case DataType.Material:
                    break;
            }
        }
    }

    private List<DataItem> CreateDivideItemList(List<DataItem> itemDataList)
    {
        var dataAllDivideItemList = new List<DataItem>();
        for (int i = 0; i < itemDataList.Count; i++)
        {
            var newItem = itemDataList[i];

            switch (newItem.dataType)
            {
                case DataType.Consume:
                    dataAllDivideItemList.Add(new DataConsumable(newItem));
                    break;
                case DataType.AllItem:
                    dataAllDivideItemList.Add(new DataAllItem(newItem));
                    break;
                case DataType.Material:
                    dataAllDivideItemList.Add(new DataMaterial(newItem));
                    break;
            }
        }

        for (int i = 0; i < dataAllDivideItemList.Count; i++)
        {
            if (dataAllDivideItemList[i].LimitCount < dataAllDivideItemList[i].OwnCount)
            {
                var divideItem = CreateDivideItem(dataAllDivideItemList[i]);
                if (divideItem != null)
                    dataAllDivideItemList.Add(divideItem);
            }
        }
        return dataAllDivideItemList;
    }
    private DataItem CreateDivideItem(DataItem item)
    {
        DataItem newItem = null;
        switch (item.dataType)
        {
            case DataType.Consume:
                newItem = new DataConsumable(item);
                break;
            case DataType.AllItem:
                newItem = new DataAllItem(item);
                break;
            case DataType.Material:
                newItem = new DataMaterial(item);
                break;
        }
        // 한도치 계산해서 분할 복사, 원본역시 분할된 만큼 소유개수 줄음
        if (item.OwnCount > item.LimitCount)
        {
            newItem.OwnCount = item.OwnCount - item.LimitCount;
            newItem.LimitCount = item.LimitCount;
            newItem.itemTableElem = item.itemTableElem;
            newItem.itemId = item.itemId;
            item.OwnCount = item.LimitCount;
        }
        else
        {
            return null;
        }
        return newItem;
    }

    private void SortItemList(List<DataItem> itemList)
    {
        itemList.Sort((lhs, rhs) => ItemCompare(lhs, rhs));
    }

    private int ItemCompare(DataItem lhs, DataItem rhs)
    {
        int result = 1;
        if (lhs.dataType < rhs.dataType)
            return result;
        else if (lhs.dataType > rhs.dataType)
            return -result;
        else
        {
            if (lhs.itemId < rhs.itemId)
                return result;
            else if (lhs.itemId > rhs.itemId)
                return -result;
            else
            {
                if (lhs.OwnCount < rhs.OwnCount)
                    return result;
                else if (lhs.OwnCount > rhs.OwnCount)
                    return -result;
            }
        }
        return 0;
    }
}
