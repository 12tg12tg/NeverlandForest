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
    [Header("������ư(�̹���)��, ����â")]
    public BottomInfoUI info;
    public BottomInfoUI info2page;
    public List<BottomItemButtonUI> itemButtons;
    public List<BottomItemButtonUI> itemButtons2page;
    public List<RandomEventItem> rewardItemButtons;
    public List<GameObject> windows;

    public GameObject itemInfo;
    public List<GameObject> rewardOrCheck;
    public RectTransform closeBtn;

    //Vars
    [HideInInspector] public ButtonState buttonState;

    //PopUpWindow
    [Header("�˾�â")]
    public bool isPopUp;
    public GameObject randomEventUI;
    public RectTransform popUpWindow;
    public RectTransform confirmPanel;
    public RectTransform inventoryFullPanel;
    public RectTransform needMoreVal;

    /*[HideInInspector]*/ public RectTransform itemBox;

    // RandomEventData
    [HideInInspector] public DataRandomEvent randomEventData;

    // RandomEventText
    [Header("�ؽ�Ʈ")]
    public List<GameObject> selectButtons;
    public TextMeshProUGUI eventDesc;
    public TextMeshProUGUI selectName;
    public TextMeshProUGUI selectDesc;
    public GameObject resultLayout;
    public TextMeshProUGUI resultDesc;

    [Header("�ܺ� UI �� ��� ����")]
    public GameObject inventory;

    private List<DataAllItem> rewardItemList = new();

    [HideInInspector] public DataAllItem selectInvenItem;
    [HideInInspector] public List<DataAllItem> selectRewardItems = new List<DataAllItem>();
    [HideInInspector] public int curOperatorFeedback = -1;
    [HideInInspector] public bool isRaneomEventOn;
    [HideInInspector] public int selectSlotNum;
    [HideInInspector] public bool isBattleStart;

    private void Awake()
    {
        //instance = this;
        //gameObject.SetActive(false);
        //popUpWindow.gameObject.SetActive(false);
        //windows[0].SetActive(true);
        //windows[1].SetActive(false);
        //closeBtn.gameObject.SetActive(false);
        //ItemButtonInit();
    }

    public void Init()
    {
        instance = this;
        randomEventUI.SetActive(false);
        popUpWindow.gameObject.SetActive(false);
        windows[0].SetActive(true);
        windows[1].SetActive(false);
        closeBtn.gameObject.SetActive(false);
        ItemButtonInit();
    }

    private void OnEnable()
    {
        inventory.SetActive(false);
        isRaneomEventOn = true;
    }
    private void OnDisable()
    {
        inventory.SetActive(true);
        isRaneomEventOn = false;
    }
    private void Update()
    {
        var touchPos = GameManager.Manager.MultiTouch;
        if (touchPos.TouchCount > 0)
        {
            if (!IsContainPos(touchPos.PrimaryStartPos) && !ISContainItemBox(touchPos.PrimaryStartPos) && isPopUp == true)
                isPopUp = false;
        }

        if (isPopUp)
        {
            if (!popUpWindow.gameObject.activeSelf)
            {
                popUpWindow.gameObject.SetActive(true);
                SoundManager.Instance.PlayWalkSound(false);
            }
        }
        else
        {
            if (popUpWindow.gameObject.activeSelf)
            {
                popUpWindow.gameObject.SetActive(false);
                selectInvenItem = null;
                itemButtons.ForEach(n => n.SelectActive(false));
                itemButtons2page.ForEach(n => n.SelectActive(false));
            }
        }
    }
    private bool IsContainPos(Vector2 pos)
    {
        var camera = GameManager.Manager.CamManager.uiCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(popUpWindow, pos, camera);
    }

    private bool ISContainItemBox(Vector2 pos)
    {
        var camera = GameManager.Manager.CamManager.uiCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(itemBox, pos, camera);
    }

    public void EventInit(DataRandomEvent data)
    {
        randomEventData = data;
        randomEventUI.SetActive(true);
        windows[0].SetActive(true);
        windows[1].SetActive(false);
        confirmPanel.gameObject.SetActive(false);
        inventoryFullPanel.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(true);
        eventDesc.text = randomEventData.eventDesc;
        SelectInit();

        if (isBattleStart)
        {
            isBattleStart = false;
            GameManager.Manager.LoadScene(GameScene.Battle);
        }
    }

    private void SelectInit()
    {
        curOperatorFeedback = -1;
        int j = selectButtons.Count - 1;
        for (int i = 0; i < selectButtons.Count; i++)
        {
            if (randomEventData.selectBtnCount < i + 1)
            {
                selectButtons[j].transform.GetChild(0).gameObject.SetActive(false);
                j--;
                continue;
            }
            var btnObj = selectButtons[i].transform.GetChild(0);

            var button = btnObj.GetComponent<Button>();

            var texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            var nameText = texts[0];
            var infoText = texts[1];

            // ���� ���� �̺�Ʈ �������� �ǵ�� �Լ� add��
            // ���� UI�� ��ȯ�ϴ� �Լ� add
            int selectNum = i+1;
            button.onClick.AddListener(() => randomEventData.SelectFeedBack(selectNum));
            //button.onClick.AddListener(() => NextPage());
            // �̺�Ʈ �������� i �ε��� selectString���� �ʱ�ȭ
            nameText.text = randomEventData.selectName[i];
            // �̺�Ʈ �����Ϳ��� i �ε��� is���ð� �� true �� �ʱ�ȭ �ƴϸ� ����� defaultText�� ǥ��
            infoText.text = randomEventData.selectInfos[i];
        }
    }

    public void NextPage()
    {
        windows[0].SetActive(false);
        windows[1].SetActive(true);

        resultDesc.text = randomEventData.resultInfo;
        selectName.text = randomEventData.selectName[randomEventData.curSelectNum - 1];
        selectDesc.text = randomEventData.selectResultDesc;
        
        if(randomEventData.rewardItems.Count > 0)
        {
            rewardItemList.AddRange(randomEventData.rewardItems);

            resultLayout.SetActive(false);
            itemInfo.SetActive(true);
            rewardOrCheck[0].SetActive(true);
            rewardOrCheck[1].SetActive(false);
            RewardItemLIstInit(rewardItemList);
        }
        else
        {
            rewardOrCheck[0].SetActive(false);
            rewardOrCheck[1].SetActive(true);
            resultLayout.SetActive(true);
            itemInfo.SetActive(false);
        }
    }

    public void ExitEvent()
    {
        if (rewardItemList.Count > 0)
        {
            SoundManager.Instance.Play(SoundType.Se_Button);
            confirmPanel.gameObject.SetActive(true);
        }
        else
            EventExitInit();
    }
    public void EventExitInit()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);
        // �̺�Ʈ ������ ���� �����Ǹ� �ȵǴ� �͵� ��� �ʱ�ȭ

        randomEventData.DataDefaultEventExit();
        rewardItemList.Clear();
        for (int i = 0; i < selectButtons.Count; i++)
        {
            var button = selectButtons[i].transform.GetChild(0).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
        }
        randomEventUI.SetActive(false);
        BottomUIManager.Instance.ItemListInit();

        Vars.UserData.randomEventDatas = RandomEventManager.Instance.allDataList;

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.RandomEvent);
    }
    public void RewardItemLIstInit(List<DataAllItem> itemList)
    {
        rewardItemButtons.ForEach(n => n.Init(null));
        rewardItemButtons.ForEach(n => n.IsSelect = false);

        var count = itemList.Count;

        for (int i = 0; i < count; i++)
        {
            if(i >= rewardItemButtons.Count)
            {
                Debug.LogError("��ġ �̻�!");
            }
            rewardItemButtons[i].Init(itemList[i]);
        }
    }

    public void GetSelectItem()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);
        if (rewardItemList.Count <= 0 || selectRewardItems.Count == 0)
            return;

        for (int i = 0; i < selectRewardItems.Count; i++)
        {
            if(!Vars.UserData.AddItemData(selectRewardItems[i]))
            {
                inventoryFullPanel.gameObject.SetActive(true);
            }
            Vars.UserData.ExperienceListAdd(selectRewardItems[i].itemId);
            ItemListInit();
            if(selectRewardItems[i].OwnCount <= 0)
            {
                var index = rewardItemList.FindIndex(x => x.itemId == selectRewardItems[i].itemId);
                rewardItemList.RemoveAt(index);
                info.Init();
                info2page.Init();
            }
            RewardItemLIstInit(rewardItemList);
        }

        selectRewardItems.Clear();
    }

    public void GetAllItems()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);
        if (rewardItemList.Count <= 0)
            return;

        selectRewardItems.Clear();
        var removeList = new List<string>();
        for (int i = 0; i < rewardItemList.Count; i++)
        {
            Vars.UserData.AddItemData(rewardItemList[i]);
            Vars.UserData.ExperienceListAdd(rewardItemList[i].itemId);
            ItemListInit();
            if(rewardItemList[i].OwnCount <= 0)
            {
                removeList.Add(rewardItemList[i].itemId);
            }
        }
        // ����Ʈ�� �ݺ������� �������ҋ��� �׻� �����ϱ�
        foreach(var id in removeList)
        {
            var index = rewardItemList.FindIndex(x => x.itemId == id);
            if (index != -1)
                rewardItemList.RemoveAt(index);
        }
        RewardItemLIstInit(rewardItemList);
        info.Init();
        info2page.Init();

        if(rewardItemList.Count > 0)
            inventoryFullPanel.gameObject.SetActive(true);
    }

    public void ItemButtonInit()
    {
        info.Init();
        info2page.Init();
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

        // �̰� Create�Ҷ� �ӽø���Ʈ �����ؼ� ������̱� �ѵ� ���� ȣ������� �� �����Ҽ���
        var list = Vars.UserData.HaveAllItemList.ToList();
        var list1 = CreateDivideItemList(list);
        SortItemList(list1);
        int count = list1.Count;
        for (int i = 0; i < count; i++)
        {
            itemButtons[i].Init(list1[i], i);
            itemButtons2page[i].Init(list1[i], i);
        }
    }

    public void ItemUse()
    {
        if (selectInvenItem == null)
            return;
        var allItem = new DataAllItem(selectInvenItem);
        allItem.OwnCount = 1;
        if (Vars.UserData.RemoveItemData(allItem))
        {
            selectInvenItem = null;
            isPopUp = false;
            itemButtons.ForEach(n => n.SelectActive(false));
            itemButtons2page.ForEach(n => n.SelectActive(false));
            info.Init();
            info2page.Init();
        }
        ItemListInit();
    }
    public void ItemDump()
    {
        if (selectInvenItem == null)
            return;

        var allItem = new DataAllItem(selectInvenItem);
        allItem.OwnCount = 1;
        if (Vars.UserData.RemoveItemData(allItem))
        {
            itemBox.GetComponent<BottomItemButtonUI>().SelectActive(false);
            selectInvenItem = null;
            isPopUp = false;
            itemButtons.ForEach(n => n.SelectActive(false));
            itemButtons2page.ForEach(n => n.SelectActive(false));
            info.Init();
            info2page.Init();
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
}
//var npos = camera.ScreenToViewportPoint(itemBox.position);
//var wpos = camera.WorldToViewportPoint(itemBox.position);
//var wposS = camera.WorldToScreenPoint(itemBox.position);
//var pp = itemBox.position;
//Debug.Log($"{itemBox.rect.position}, {itemBox.anchoredPosition}, {itemBox.position}, {npos}, {pos}, {wpos}, {wposS}");
//Debug.Log($"{itemBox.rect.position} {pos}");
//return RectTransformUtility.ScreenPointToLocalPointInRectangle(itemBox, pos);
//return RectTransformUtility.ScreenPointToWorldPointInRectangle(itemBox, pos, camera,out _);







