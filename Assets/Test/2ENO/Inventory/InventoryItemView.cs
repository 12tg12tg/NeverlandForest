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
        // 초기화    // 칸 합치기 기능 포함
        InitDivideItemList(this.itemDataList);

        SortItemList();
        SetAllItems(divideItemList);
    }


    //Init 초기화 느낌 1회성
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
                // 생성자에서 분할 작업 나눠가짐
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
            Debug.Log("아이템 삭제완료");
        }
    }

    public void GetItemExcute(List<DataAllItem> itemList)
    {
        getItemDataList = itemList;
        // 인벤토리 남은 칸 계산
        int space = MaxItemCount - divideItemList.Count;

        // 얻은 아이템데이터를 분할해서 리스트화
        for(int i = 0; i < getItemDataList.Count; i++)
        {
            var divideitem = DivideCreate(getItemDataList[i]);
            if (divideitem == null)
                break;
            getItemDataList.Add(divideitem);
        }
        // 남은 칸 만큼 채워넣고, 못채워넣은 데이터는 리스트에 남아있다
        int count = 0;
        while(space > 0)
        {
            var divideIndex = divideItemList.FindLastIndex(x => x.itemId == getItemDataList[0].itemId);
            var itemIndex = itemDataList.FindLastIndex(x => x.itemId == getItemDataList[0].itemId);
            if (divideIndex == -1)
            {
                // 두가지 새로운 아이템을 각 list에 넣는데 그냥 넣으면 같은 데이터 참조로 각 리스트 데이터 수정이 서로에게 적용됨
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
                    // 이부분은 남은공간을 쓰지 않는 부분이기때문에 space값에 변동없도록 처리
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
        // 남은공간 0일때, 만약 칸 아이템 빈 개수가 있을때 마저 채워넣기
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
            Debug.Log("못 담은 아이템이 아직 잇음");
        }
        else
        {
            Debug.Log("아이템 획득 완료");
        }

        SortItemList();
        SetAllItems(divideItemList);
    }


}
