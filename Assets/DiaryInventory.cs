using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiaryInventory : MonoBehaviour
{
    public enum ButtonState { Item, Skill }
    [Header("�ν��Ͻ�")]
    public BottomInfoUI info;
    public List<BottomItemButtonUI> itemButtons;
    private static DiaryInventory instance;
    public static DiaryInventory Instance => instance;

    [Header("�˾� On/Off Ȯ��")] public bool isPopUp;
    [Header("�˾� ���� ����")]
    public RectTransform popUpWindow;
    public DataAllItem selectItem;
    public RectTransform selectedItemRect;

    [Header("������ ���� ����")]
    public int selectItemSlotNum;

    private void Start()
    {
        instance = this;
        if (popUpWindow !=null)
        {
            popUpWindow.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        var touchPos = MultiTouch.Instance.TouchPos;
        if (!isPopUp)
        {
            if (MultiTouch.Instance.TouchCount > 0)
            {
                if (!IsContainPos(touchPos))
                {
                    itemButtons.ForEach(n => n.SelectActive(false));
                    if (popUpWindow != null)
                    {
                        popUpWindow.gameObject.SetActive(false);
                    }
                    isPopUp = false;
                }
            }
        }
        if (MultiTouch.Instance.TouchCount > 0 && !IsContainItemRect(touchPos))
        {
            isPopUp = false;
        }
    }
    private bool IsContainPos(Vector2 pos)
    {
        var camera = GameManager.Manager.CamManager.uiCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(popUpWindow, pos, camera);
    }
    private bool IsContainItemRect(Vector2 pos)
    {
        var camera = GameManager.Manager.CamManager.uiCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(selectedItemRect, pos, camera);
    }

    public void ItemButtonInit()
    {
        info.Init();
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
            itemButtons[i].Init(list[i]);
        }
    }

    private List<DataAllItem> CreateDivideItemList(List<DataAllItem> itemDataList)
    {
        var dataAllDivideItemList = new List<DataAllItem>();
        for (int i = 0; i < itemDataList.Count; i++)
        {
            var newItem = itemDataList[i];
            dataAllDivideItemList.Add(new DataAllItem(newItem));
        }

        for (int i = 0; i < dataAllDivideItemList.Count; i++)
        {
            if (dataAllDivideItemList[i].ItemTableElem.limitCount < dataAllDivideItemList[i].OwnCount)
            {
                var divideItem = CreateDivideItem(dataAllDivideItemList[i]);
                if (divideItem != null)
                    dataAllDivideItemList.Add(divideItem);
            }
        }
        return dataAllDivideItemList;
    }
    private DataAllItem CreateDivideItem(DataAllItem item)
    {
        DataAllItem newItem = new DataAllItem(item);

        // �ѵ�ġ ����ؼ� ���� ����, �������� ���ҵ� ��ŭ �������� ����
        if (item.OwnCount > item.ItemTableElem.limitCount)
        {
            newItem.OwnCount = item.OwnCount - item.ItemTableElem.limitCount;
            item.OwnCount = item.ItemTableElem.limitCount;
        }
        else
            return null;
        return newItem;
    }

    private void SortItemList(List<DataAllItem> itemList)
    {
        itemList.Sort((lhs, rhs) => ItemCompare(lhs, rhs));
    }

    private int ItemCompare(DataAllItem lhs, DataAllItem rhs)
    {
        int result = 1;
        if (lhs.ItemTableElem.type.CompareTo(rhs.ItemTableElem.type) != 0)
            return lhs.ItemTableElem.type.CompareTo(rhs.ItemTableElem.type);
        else
        {
            if (lhs.ItemTableElem.id.CompareTo(rhs.ItemTableElem.id) != 0)
                return lhs.ItemTableElem.id.CompareTo(rhs.ItemTableElem.id);
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

    public void ItemUse() // ��ư Ŭ�� �Լ�
    {
        if (selectItem == null)
            return;
        var allItem = new DataAllItem(selectItem);
        allItem.OwnCount = 1;

        if (allItem.ItemTableElem.isEat)
        {
            ConsumeManager.RecoveryHunger(allItem.ItemTableElem.stat_str);
            ConsumeManager.RecoverHp(allItem.ItemTableElem.stat_Hp);
        }
        else if (allItem.itemId == "ITEM_19")
        {
            // ���� 1ĭ ä��� �ǳ�?
            ConsumeManager.FullingLantern(1);
        }
        else
            return;

        if (Vars.UserData.RemoveItemData(allItem))
        {
            popUpWindow.gameObject.SetActive(false);
            selectItem = null;
            isPopUp = false;
        }
        ItemListInit();
        BottomUIManager.Instance.ItemListInit();
    }
    public void ItemDump() // ��ư Ŭ�� �Լ�
    {
        if (selectItem == null)
            return;

        var allItem = new DataAllItem(selectItem);
        allItem.OwnCount = 1;
        if (Vars.UserData.RemoveItemData(allItem))
        {
            popUpWindow.gameObject.SetActive(false);
            selectItem = null;
            isPopUp = false;
        }
        ItemListInit();
        BottomUIManager.Instance.ItemListInit();

    }
}
