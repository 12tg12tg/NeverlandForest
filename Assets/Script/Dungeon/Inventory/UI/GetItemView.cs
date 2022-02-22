using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//public class GetItemView : GenericWindow
//{
//    [Header("ETC")]
//    [Space(3)]
//    public InventoryController invenCtrl;

//    public InventoryItem itemPrefab;
//    public ScrollRect scrollRect;

//    private List<InventoryItem> itemGoList = new List<InventoryItem>();
//    private List<DataItem> itemDataList = new List<DataItem>();

//    private int selectedSlot = -1;
//    private int MaxItemCount = 20;

//    protected override void Awake()
//    {
//        for (var i = 0; i < MaxItemCount; ++i)
//        {
//            var item = Instantiate(itemPrefab, scrollRect.content);
//            item.Slot = i;
//            itemGoList.Add(item);
//            item.gameObject.SetActive(false);

//            var button = item.GetComponent<Button>();
//            button.onClick.AddListener(() => OnItemClickEvent(item.Slot));
//        }
//    }

//    public void Init(List<DataItem> itemList)
//    {
//        itemDataList = itemList;
//        List<int> zeroItemIndexList = new List<int>();

//        for (int i = 0; i < itemDataList.Count; i++)
//        {
//            if(itemDataList[i].OwnCount <= 0)
//                zeroItemIndexList.Add(i);
//        }
//        for (int i = zeroItemIndexList.Count - 1; i >= 0; i--)
//        {
//            itemDataList.RemoveAt(zeroItemIndexList[i]);
//        }

//        SetAllItems(itemDataList);
//    }

//    public void SetAllItems(List<DataItem> itemList)
//    {
//        foreach (var item in itemGoList)
//        {
//            item.gameObject.SetActive(false);
//        }

//        for (var i = 0; i < itemList.Count; i++)
//        {
//            itemGoList[i].gameObject.SetActive(true);
//            switch (itemList[i].dataType)
//            {
//                case DataType.Default:
//                    break;
//                case DataType.Consume:
//                    var conItem = new DataConsumable(itemList[i]);
//                    itemGoList[i].Init(conItem);
//                    break;
//                case DataType.AllItem:
//                    var allItem = new DataAllItem(itemList[i]);
//                    itemGoList[i].Init(allItem);
//                    break;
//                case DataType.Material:
//                    var materialitem = new DataAllItem(itemList[i]);
//                    itemGoList[i].Init(materialitem);
//                    break;
//            }
//        }

//        if (this.itemDataList.Count > 0)
//        {
//            selectedSlot = 0;
//            EventSystem.current.SetSelectedGameObject(itemGoList[selectedSlot].gameObject);
//        }
//    }

//    public void AllItemGet()
//    {
//        int overItemCount = 0;
//        foreach(var item in itemDataList)
//        {
//            if(!Vars.UserData.AddItemData(item))
//            {
//                overItemCount++;
//            }
//        }
//        invenCtrl.Init();
//        Init(itemDataList);

//        if (overItemCount > 0)
//            Debug.Log("¿œ∫Œ æ∆¿Ã≈€ »πµÊ πÃøœ∑·");
//        else
//            Debug.Log("∏µÁ æ∆¿Ã≈€ »πµÊ øœ∑·");
//    }

//    private void OnItemClickEvent(int slot)
//    {
//        var slotItem = itemDataList.Find(x => (x.itemId == itemGoList[slot].DataItem.itemId) && (x.dataType == itemGoList[slot].DataItem.dataType));

//        if (!Vars.UserData.AddItemData(slotItem))
//            Debug.Log("¿œ∫Œ æ∆¿Ã≈€ »πµÊ πÃøœ∑·");
//        invenCtrl.Init();
//        Init(itemDataList);
//    }
//}

//switch (itemGoList[slot].DataItem.dataType)
//{
//    case DataType.Default:
//        break;
//    case DataType.Consume:
//       
//        break;
//    case DataType.AllItem:
//        var allItem = itemGoList[slot].DataItem as DataAllItem;
//        //invenCtrl.OpenClickMessageWindow(item);
//        break;
//}