using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomEventUIManager : MonoBehaviour
{
    public enum ButtonState { Item, Skill }

    private static RandomEventUIManager instance;
    public static RandomEventUIManager Instance => instance;

    //Instance
    public BottomInfoUI info;
    public List<BottomItemButtonUI> itemButtons;

    //Vars
    [HideInInspector] public ButtonState buttonState;

    //PopUpWindow
    public RectTransform popUpWindow;
    public DataItem selectItem;

    // RandomEventText
    public List<GameObject> selectButtons;

    private void Awake()
    {
        instance = this;
        //popUpWindow.SetActive(false);
    }

    private void SelectInit()
    {
        for (int i = 0; i < selectButtons.Count; i++)
        {
            var btnObj = selectButtons[i];

            var button = btnObj.GetComponent<Button>();
            var image = btnObj.GetComponent<Image>();

            var texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            var nameText = texts[0];
            var infoText = texts[1];

            // 현재 방의 이벤트 데이터의 피드백 함수 add함
            // 다음 UI로 전환하는 함수 add
            //button.onClick.AddListener()

            // 이벤트 데이터의 i 인덱스 selectString으로 초기화
            //nameText.text = 
            // 이벤트 데이터에서 i 인덱스 is선택값 이 true 면 초기화 아니면 비공개 defaultText로 표시
            //infoText.text = 
        }
    }

    public void ItemButtonInit()
    {
        info.Init();
        //popUpWindow.SetActive(false);

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

    public void ItemUse()
    {
        if (selectItem == null)
            return;
        switch (selectItem.dataType)
        {
            case DataType.Consume:
                break;
            case DataType.AllItem:
                var allItem = new DataAllItem(selectItem);
                allItem.OwnCount = 1;
                if (Vars.UserData.RemoveItemData(allItem))
                {
                    //popUpWindow.SetActive(false);
                    selectItem = null;
                }
                break;
        }
        ItemListInit();
    }
    public void ItemDump()
    {
        if (selectItem == null)
            return;
        switch (selectItem.dataType)
        {
            case DataType.Consume:
                break;
            case DataType.AllItem:
                var allItem = new DataAllItem(selectItem);
                allItem.OwnCount = 1;
                if (Vars.UserData.RemoveItemData(allItem))
                {
                    //popUpWindow.SetActive(false);
                    selectItem = null;
                }
                break;
        }
        ItemListInit();
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
