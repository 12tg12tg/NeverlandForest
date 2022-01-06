using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class InventoryItemView : MonoBehaviour
{
    public InventoryController invenCtrl;

    public InventoryItem itemPrefab;
    public ScrollRect scrollRect;

    public const int MaxItemCount = 21;
    private List<InventoryItem> itemGoList = new List<InventoryItem>();
    private List<DataAllItem> itemDataList = new List<DataAllItem>();
    private List<DataAllItem> divideItemList = new List<DataAllItem>();

    private List<DataAllItem> getItemDataList = new List<DataAllItem>();


    //private DataCharacter selectedCharacter;
    private int selectedSlot = -1;


    private void Awake()
    {
        for (var i = 0; i < MaxItemCount; ++i)
        {
            var item = Instantiate(itemPrefab, scrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.SetActive(false);

            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnItemClickEvent(item.Slot));
        }
    }

 
    public void Init(List<DataAllItem> itemDataList)
    {
        this.itemDataList = itemDataList;
        // �ʱ�ȭ    // ĭ ��ġ�� ��� ����
        InitDivideItemList(this.itemDataList);

        SortItemList();
        SetAllItems(divideItemList);
    }


    //Init �ʱ�ȭ ���� 1ȸ��
    public void InitDivideItemList(List<DataAllItem> itemDataList)
    {
        var dataAllDivideItemList = new List<DataAllItem>();
        for (int i = 0; i < itemDataList.Count; i++)
        {
            var newItem = itemDataList[i];
            dataAllDivideItemList.Add(new DataAllItem(newItem));
        }

        for (int i = 0; i < dataAllDivideItemList.Count; i++)
        {
            if(dataAllDivideItemList[i].LimitCount < dataAllDivideItemList[i].OwnCount)
            {
                // �����ڿ��� ���� �۾� ��������
                var divideItem = DivideCreate(dataAllDivideItemList[i]);
                if (divideItem == null)
                    break;
                dataAllDivideItemList.Add(divideItem);
            }
        }

        divideItemList = dataAllDivideItemList;
    }

    public DataAllItem DivideCreate(DataAllItem item)
    {
        var newItem = new DataAllItem();
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

    public void SetAllItems(List<DataAllItem> itemList)
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        for(var i = 0; i< itemList.Count; i++)
        {
            itemGoList[i].gameObject.SetActive(true);
            itemGoList[i].Init(itemList[i]);
        }

        if (this.itemDataList.Count > 0)
        {
            selectedSlot = 0;
            EventSystem.current.SetSelectedGameObject(itemGoList[selectedSlot].gameObject);
        }

        invenCtrl.UserDataUpdate(itemDataList);
    }

    private void OnItemClickEvent(int slot)
    {
        var item = itemGoList[slot].DataItem as DataAllItem;
        invenCtrl.OpenMessageWindow(item);
    }

    public void SortItemList()
    {
        divideItemList.Sort((lhs, rhs) => lhs.itemId.CompareTo(rhs.itemId));
    }

    public void DeleteItemExcute(DataAllItem item)
    {
        var index = itemDataList.FindLastIndex(x => x.itemId == item.itemId);

        if(index != -1)
        {
            itemDataList[index].OwnCount -= item.OwnCount;

            if(itemDataList[index].OwnCount <= 0)
            {
                itemDataList.RemoveAt(index);
                invenCtrl.UserDataItemRemove(item);
            }

            InitDivideItemList(itemDataList);
            SortItemList();
            SetAllItems(divideItemList);
            Debug.Log("������ �����Ϸ�");
        }
    }

    public void GetItemExcute(List<DataAllItem> itemList)
    {
        getItemDataList = itemList;
        // �κ��丮 ���� ĭ ���
        int space = MaxItemCount - divideItemList.Count;

        // ���� �����۵����͸� �����ؼ� ����Ʈȭ
        for(int i = 0; i < getItemDataList.Count; i++)
        {
            var divideitem = DivideCreate(getItemDataList[i]);
            if (divideitem == null)
                break;
            getItemDataList.Add(divideitem);
        }
        // ���� ĭ ��ŭ ä���ְ�, ��ä������ �����ʹ� ����Ʈ�� �����ִ�
        int count = 0;
        while(space > 0)
        {
            var divideIndex = divideItemList.FindLastIndex(x => x.itemId == getItemDataList[0].itemId);
            var itemIndex = itemDataList.FindLastIndex(x => x.itemId == getItemDataList[0].itemId);
            if (divideIndex == -1)
            {
                // �ΰ��� ���ο� �������� �� list�� �ִµ� �׳� ������ ���� ������ ������ �� ����Ʈ ������ ������ ���ο��� �����
                var newItem = new DataAllItem(getItemDataList[0]);
                var newItem2 = new DataAllItem(getItemDataList[0]);
                divideItemList.Add(newItem);
                itemDataList.Add(newItem2);
            }
            else if (divideItemList[divideIndex].LimitCount == divideItemList[divideIndex].OwnCount)
            {
                divideItemList.Add(getItemDataList[0]);
                itemDataList[itemIndex].OwnCount += getItemDataList[0].OwnCount;
            }
            else
            {
                itemDataList[itemIndex].OwnCount += getItemDataList[0].OwnCount;
                var spaceCount = (divideItemList[divideIndex].LimitCount - divideItemList[divideIndex].OwnCount);
                if (spaceCount > getItemDataList[0].OwnCount)
                {
                    divideItemList[divideIndex].OwnCount += getItemDataList[0].OwnCount;
                    // �̺κ��� ���������� ���� �ʴ� �κ��̱⶧���� space���� ���������� ó��
                    space++;
                }
                else
                {
                    divideItemList[divideIndex].OwnCount = divideItemList[divideIndex].LimitCount;
                    getItemDataList[0].OwnCount -= spaceCount;
                    divideItemList.Add(getItemDataList[0]);
                }
            }
            space--;
            getItemDataList.Remove(getItemDataList[0]);
            if (getItemDataList.Count <= 0)
                break;
            count++;
        }
        // �������� 0�϶�, ���� ĭ ������ �� ������ ������ ���� ä���ֱ�
        if(space == 0)
        {
            var divideIndex = divideItemList.FindLastIndex(x => x.itemId == getItemDataList[0].itemId);
            if(divideIndex != -1)
            {
                var spaceCount = (divideItemList[divideIndex].LimitCount - divideItemList[divideIndex].OwnCount);
                if (spaceCount > getItemDataList[0].OwnCount)
                {
                    divideItemList[divideIndex].OwnCount += getItemDataList[0].OwnCount;
                    getItemDataList.Remove(getItemDataList[0]);
                }
                else
                {
                    divideItemList[divideIndex].OwnCount = divideItemList[divideIndex].LimitCount;
                    getItemDataList[0].OwnCount -= spaceCount;
                }
            }
        }

        if(getItemDataList.Count > 0)
        {
            Debug.Log("�� ���� �������� ���� ����");
        }
        else
        {
            Debug.Log("������ ȹ�� �Ϸ�");
        }

        SortItemList();
        SetAllItems(divideItemList);
    }


}
