using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BottomUIManager : MonoBehaviour
{
    public enum ButtonState { Item, Skill }

    private static BottomUIManager instance;
    public static BottomUIManager Instance => instance;

    //Instance
    public BottomInfoUI info;
    public List<BottomSkillButtonUI> skillButtons;
    public List<BottomItemButtonUI> itemButtons;

    //Vars
    [HideInInspector] public ButtonState buttonState;

    //PopUpWindow
    public bool isPopUp;
    public RectTransform popUpWindow;
    public DataItem selectItem;


    private void Awake()
    {
        instance = this;
        SkillButtonInit();
        popUpWindow.gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Set Battle State Toggle"))
        {
            if (GameManager.Manager.State == GameState.Battle)
                GameManager.Manager.State = GameState.None;
            else
                GameManager.Manager.State = GameState.Battle;
        }
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

        if(MultiTouch.Instance.TouchCount > 0)
            isPopUp = false;
    }
    private bool IsContainPos(Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(popUpWindow, pos);
    }

    public void SkillButtonInit()
    {
        info.Init();
        popUpWindow.gameObject.SetActive(false);

        buttonState = ButtonState.Skill;
        itemButtons.ForEach((n) => n.gameObject.SetActive(false));
        skillButtons.ForEach((n) => n.gameObject.SetActive(true));

        var list = Vars.BoySkillList;
        int count = list.Count;

        int buttonIndex = 1;
        for (int i = 0; i < count; i++)
        {
            skillButtons[buttonIndex++].Init(list[i]);
        }

        buttonIndex = 7;
        list = Vars.GirlSkillList;
        for (int i = 0; i < count; i++)
        {
            skillButtons[buttonIndex++].Init(list[i]);
        }
    }

    public void ItemButtonInit()
    {
        info.Init();
        popUpWindow.gameObject.SetActive(false);

        buttonState = ButtonState.Item;
        itemButtons.ForEach((n) => n.gameObject.SetActive(true));
        skillButtons.ForEach((n) => n.gameObject.SetActive(false));

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
