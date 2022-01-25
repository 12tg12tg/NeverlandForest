using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 이 UI가 켜지는 순간 기본 던전맵 동작은 작동 중단
public class RandomEventUIManager : MonoBehaviour
{
    public enum ButtonState { Item, Skill }

    private static RandomEventUIManager instance;
    public static RandomEventUIManager Instance => instance;

    //Instance
    public BottomInfoUI info;
    public List<BottomItemButtonUI> itemButtons;
    public List<BottomItemButtonUI> itemButtons2page;

    public List<RandomEventItem> rewardItemButtons;
    public List<GameObject> windows;
    public List<GameObject> rewardOrCheck;

    //Vars
    [HideInInspector] public ButtonState buttonState;

    //PopUpWindow
    public bool isPopUp;
    public RectTransform popUpWindow;
    public DataAllItem selectItem;
    public RectTransform confirmPanel;

    //closeBtn
    public RectTransform closeBtn;

    // RandomEventData
    public DataRandomEvent randomEventData;

    // RandomEventText
    public List<GameObject> selectButtons;
    public TextMeshProUGUI eventDesc;
    public TextMeshProUGUI selectName;
    public TextMeshProUGUI selectDesc;
    public TextMeshProUGUI resultDesc;
    public GameObject itemInfo;

    private List<DataAllItem> rewardItemList = new();
    // 선택 아이템을 인벤토리쪽과 보상쪽 둘다 하나로 쓸수 있는지 고민?
    public DataAllItem curSelectItem;

    public bool isEventOn;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        popUpWindow.gameObject.SetActive(false);
        windows[0].SetActive(true);
        windows[1].SetActive(false);
        closeBtn.gameObject.SetActive(false);
        ItemButtonInit();
    }

    private void OnEnable()
    {
        isEventOn = true;
    }
    private void OnDisable()
    {
        isEventOn = false;
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
                    selectItem = null;

                    for (int i = 0; i < itemButtons.Count; i++)
                    {
                        itemButtons[i].IsSelect = false;
                        itemButtons2page[i].IsSelect = false;
                    }
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
        windows[0].SetActive(true);
        windows[1].SetActive(false);
        confirmPanel.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        eventDesc.text = randomEventData.eventDesc;
        SelectInit();
    }

    private void SelectInit()
    {
        int j = selectButtons.Count - 1;
        for (int i = 0; i < selectButtons.Count; i++)
        {
            if (randomEventData.selectCount < i + 1)
            {
                selectButtons[j].GetComponent<Image>().enabled = false;
                selectButtons[j].GetComponent<Button>().enabled = false;
                j--;
                continue;
            }
            var btnObj = selectButtons[i];

            var button = btnObj.GetComponent<Button>();
            var image = btnObj.GetComponent<Image>();

            var texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            var nameText = texts[0];
            var infoText = texts[1];

            // 현재 방의 이벤트 데이터의 피드백 함수 add함
            // 다음 UI로 전환하는 함수 add
            int selectNum = i+1;
            button.onClick.AddListener(() => randomEventData.SelectFeedBack(selectNum));
            button.onClick.AddListener(() => NextPage());
            // 이벤트 데이터의 i 인덱스 selectString으로 초기화
            nameText.text = randomEventData.selectName[i];
            // 이벤트 데이터에서 i 인덱스 is선택값 이 true 면 초기화 아니면 비공개 defaultText로 표시
            infoText.text = randomEventData.SelectInfos[i];
        }
    }

    private void NextPage()
    {
        windows[0].SetActive(false);
        windows[1].SetActive(true);
        closeBtn.gameObject.SetActive(true);

        resultDesc.text = randomEventData.resultInfo;
        selectName.text = randomEventData.selectName[randomEventData.curSelectNum - 1];
        selectDesc.text = randomEventData.selectResult;
        
        if(randomEventData.rewardItems.Count > 0)
        {
            rewardItemList.AddRange(randomEventData.rewardItems);

            resultDesc.enabled = false;
            itemInfo.SetActive(true);
            rewardOrCheck[0].SetActive(true);
            rewardOrCheck[1].SetActive(false);
            RewardItemLIstInit(rewardItemList);
        }
        else
        {
            rewardOrCheck[0].SetActive(false);
            rewardOrCheck[1].SetActive(true);
            resultDesc.enabled = true;
            itemInfo.SetActive(false);
        }
    }

    public void ExitEvent()
    {
        if (rewardItemList.Count > 0)
            confirmPanel.gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
    public void RewardItemLIstInit(List<DataAllItem> itemList)
    {
        rewardItemButtons.ForEach(n => n.Init(null));

        var count = itemList.Count;

        for (int i = 0; i < count; i++)
        {
            if(i >= rewardItemButtons.Count)
            {
                Debug.LogError("수치 이상!");
            }

            rewardItemButtons[i].Init(itemList[i] as DataAllItem);
        }
    }

    public void GetSelectItem()
    {
        if (rewardItemList.Count <= 0)
            return;

        Vars.UserData.AddItemData(curSelectItem);
        ItemListInit();
        if (curSelectItem.OwnCount <= 0)
        {
            var index = rewardItemList.FindIndex(x => x.itemId == curSelectItem.itemId);
            rewardItemList.RemoveAt(index);
        }
        RewardItemLIstInit(rewardItemList);
    }

    public void GetAllItems()
    {
        if (rewardItemList.Count <= 0)
            return;

        var removeList = new List<string>();
        for (int i = 0; i < rewardItemList.Count; i++)
        {
            Vars.UserData.AddItemData(rewardItemList[i]);
            ItemListInit();
            if(rewardItemList[i].OwnCount <= 0)
            {
                removeList.Add(rewardItemList[i].itemId);
            }
        }
        // 리스트를 반복문으로 지워야할떄는 항상 주의하기
        foreach(var id in removeList)
        {
            var index = rewardItemList.FindIndex(x => x.itemId == id);
            if (index != -1)
                rewardItemList.RemoveAt(index);
        }
        RewardItemLIstInit(rewardItemList);
    }

    public void ItemButtonInit()
    {
        info.Init();
        popUpWindow.gameObject.SetActive(false);

        buttonState = ButtonState.Item;
        itemButtons.ForEach((n) => n.gameObject.SetActive(true));
        itemButtons2page.ForEach((n) => n.gameObject.SetActive(true));
        ItemListInit();
    }

    private void ItemListInit()
    {
        itemButtons.ForEach(n => n.Init(null));
        itemButtons2page.ForEach(n => n.Init(null));

        // 이거 Create할때 임시리스트 생성해서 사용중이긴 한데 자주 호출됬을때 좀 찝찝할수도
        var list = Vars.UserData.HaveAllItemList.ToList();
        var list1 = CreateDivideItemList(list);
        SortItemList(list1);
        int count = list1.Count;
        for (int i = 0; i < count; i++)
        {
            itemButtons[i].Init(list1[i]);
            itemButtons2page[i].Init(list1[i]);
        }
    }

    public void ItemUse()
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
    }
    public void ItemDump()
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

        // 한도치 계산해서 분할 복사, 원본역시 분할된 만큼 소유개수 줄음
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
}
