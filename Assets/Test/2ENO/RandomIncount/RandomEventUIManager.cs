using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// �� UI�� ������ ���� �⺻ ������ ������ �۵� �ߴ�
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
    public bool isPopUp;
    public RectTransform popUpWindow;
    public DataItem selectItem;

    // RandomEventData
    public DataRandomEvent randomEventData;

    // RandomEventText
    public List<GameObject> selectButtons;
    public TextMeshProUGUI eventDesc;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        popUpWindow.gameObject.SetActive(false);
        ItemButtonInit();
    }
    private void Update()
    {
        if (!isPopUp)
        {
            if (MultiTouch.Instance.TouchCount > 0)
            {
                var touchPos = MultiTouch.Instance.TouchPos;
                if (!IsContainPos(touchPos))
                {
                    popUpWindow.gameObject.SetActive(false);
                    isPopUp = false;
                }
            }
        }

        if (MultiTouch.Instance.TouchCount > 0)
            isPopUp = false;
    }
    private bool IsContainPos(Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(popUpWindow, pos);
    }

    public void EventInit(DataRandomEvent data)
    {
        randomEventData = data;
        gameObject.SetActive(true);
        eventDesc.text = randomEventData.eventDesc;
        SelectInit();
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

            // ���� ���� �̺�Ʈ �������� �ǵ�� �Լ� add��
            // ���� UI�� ��ȯ�ϴ� �Լ� add
            int selectNum = i+1;
            button.onClick.AddListener(() => randomEventData.SelectFeedBack(selectNum));

            // �̺�Ʈ �������� i �ε��� selectString���� �ʱ�ȭ
            nameText.text = randomEventData.selectName[i];
            // �̺�Ʈ �����Ϳ��� i �ε��� is���ð� �� true �� �ʱ�ȭ �ƴϸ� ����� defaultText�� ǥ��
            infoText.text = randomEventData.SelectInfos[i];
        }
    }

    public void ItemButtonInit()
    {
        info.Init();
        popUpWindow.gameObject.SetActive(false);

        buttonState = ButtonState.Item;
        itemButtons.ForEach((n) => n.gameObject.SetActive(true));

        ItemListInit();
    }

    private void ItemListInit()
    {
        itemButtons.ForEach(n => n.Init(null));
        // �̰� Create�Ҷ� �ӽø���Ʈ �����ؼ� ������̱� �ѵ� ���� ȣ������� �� �����Ҽ���
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
                    popUpWindow.gameObject.SetActive(false);
                    selectItem = null;
                    isPopUp = false;
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
                    popUpWindow.gameObject.SetActive(false);
                    selectItem = null;
                    isPopUp = false;
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
        // �ѵ�ġ ����ؼ� ���� ����, �������� ���ҵ� ��ŭ �������� ����
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
