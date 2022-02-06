using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class BottomUIManager : MonoBehaviour
{
    public enum ButtonState { Item, Skill }

    private static BottomUIManager instance;
    public static BottomUIManager Instance => instance;

    //Instance
    [HideInInspector] public BattleManager bm;
    [HideInInspector] public TileMaker tm;
    [Header("왼쪽 정보 창 연결")]
    public BottomInfoUI info;
    [Header("우측 12개 아이콘 연결")]
    public List<BottomSkillButtonUI> skillButtons;
    public List<BottomItemButtonUI> itemButtons;
    public BottomSkillButtonUI arrowInfo;
    public BottomSkillButtonUI oilInfo;

    [Header("우측 두 개 분류 태그 연결")]
    public List<Button> tags;

    //Vars
    [HideInInspector] public ButtonState buttonState;
    [HideInInspector] public BottomSkillButtonUI curSkillButton;

    public bool IsSkillLock { get; set; } = true;

    private bool isBoySkillDone;
    private bool isGirlSkillDone;

    //PopUpWindow
    [Header("팝업 On/Off 확인")] public bool isPopUp;
    [Header("팝업 관련 연결")]
    public RectTransform popUpWindow;
    public DataAllItem selectItem;
    public RectTransform selectedItemRect;
    public GameObject burnButton;

    [Header("아이템 선택 슬롯")]
    public int selectItemSlotNum;

    private void Awake()
    {
        instance = this;
        popUpWindow.gameObject.SetActive(false);
        burnButton.SetActive(false);
    }

    private void Start()
    {
        SkillButtonInit();

        bm = BattleManager.Instance;
        tm = TileMaker.Instance;
        switch (GameManager.Manager.State)
        {
            case GameState.None:
                break;
            case GameState.Battle:
                break;
            case GameState.Hunt:
                break;
            case GameState.Gathering:
                break;
            case GameState.Cook:
                break;
            case GameState.Camp:
                burnButton.SetActive(true);
                break;
            case GameState.Dungeon:
                break;
            default:
                break;
        }
    }

    // 스킬소모아이콘 업데이트
    public void UpdateCostInfo()
    {
        arrowInfo.UpdateCostInfo();
        oilInfo.UpdateCostInfo();
    }

    // 버튼 활성화 / 비활성화
    public void ButtonInteractive(bool interactive)
    {
        var list = GetComponentsInChildren<Button>();
        foreach (var button in list)
        {
            var skillbut =  button.transform.parent.GetComponent<BottomSkillButtonUI>();
            if (skillbut != null && skillbut.buttonType == SkillButtonType.Info)
                continue;
            button.interactable = interactive;
        }
    }

    // 스킬 버튼
    public void InteractiveSkillButton(PlayerType type, bool interactive)
    {
        if (type == PlayerType.Boy)
        {
            isBoySkillDone = !interactive;
        }
        else
        {
            isGirlSkillDone = !interactive;
        }
        UpdateSkillInteractive();
    }

    public void UpdateSkillInteractive()
    {
        skillButtons.ForEach(n =>
        {
            if (n.skill?.SkillTableElem.player == PlayerType.Boy || n.buttonType == SkillButtonType.Swap)
            {
                if (isBoySkillDone)
                    n.MakeUnclickable();
                else
                    n.MakeClickable();
            }
            else if (n.skill?.SkillTableElem.player == PlayerType.Girl)
            {
                if (isGirlSkillDone)
                    n.MakeUnclickable();
                else
                    n.MakeClickable();
            }
        });
    }

    public void IntoSkillState(BottomSkillButtonUI skillButton)
    {
        curSkillButton = skillButton;
        IsSkillLock = true;
        bm.directingLink.SetTimescaleAndShader(curSkillButton.skill.SkillTableElem);
        bm.tileLink.ReadyTileClick();
        bm.tileLink.DisplaySkillTile(curSkillButton.skill.SkillTableElem);
        skillButtons.ForEach(n => { if (n != curSkillButton) n.MakeUnclickable(); });
        tags.ForEach(n => n.interactable = false);
    }

    public void ExitSkillState(bool isButton)
    {
        IsSkillLock = !isButton;

        curSkillButton = null;
        bm.directingLink.ResetTimescaleAndShader();
        bm.tileLink.EndTileClick();
        bm.tileLink.UndisplayMonsterTile();
        UpdateSkillInteractive();
        tags.ForEach(n => n.interactable = true);
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
                    itemButtons.ForEach(n => n.IsSelect = false);
                    popUpWindow.gameObject.SetActive(false);
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
        var camera = GameManager.Manager.cm.uiCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(popUpWindow, pos, camera);
    }
    private bool IsContainItemRect(Vector2 pos)
    {
        var camera = GameManager.Manager.cm.uiCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(selectedItemRect, pos, camera);
    }

    public void SkillButtonInit() // 스킬아이콘 12개 세팅 : 태그 버튼 + 자동 활성화를 위한 함수
    {
        info.Init();
        popUpWindow.gameObject.SetActive(false);

        buttonState = ButtonState.Skill;
        itemButtons.ForEach((n) => n.gameObject.SetActive(false));
        skillButtons.ForEach((n) => n.gameObject.SetActive(true));

        var list = Vars.BoySkillList;
        int count = list.Count;

        skillButtons[0].Init();
        skillButtons[6].Init();

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

        UpdateSkillInteractive();
    }

    // 아이템 버튼
    public void ItemButtonsInteractive(bool interactive)
    {
        popUpWindow.gameObject.SetActive(false);
        isPopUp = false;
        itemButtons.ForEach(n => n.Interactive(interactive));
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

    public void ItemListInit()
    {
        itemButtons.ForEach(n => n.Init(null));
        // 이거 Create할때 임시리스트 생성해서 사용중이긴 한데 자주 호출됬을때 좀 찝찝할수도
        //var list = Vars.UserData.HaveAllItemList.ToList();
        var list1 = CreateDivideItemList(Vars.UserData.HaveAllItemList.ToList());
        SortItemList(list1);
        int count = list1.Count;

        for (int i = 0; i < count; i++)
        {
            itemButtons[i].Init(list1[i]);
        }
    }

    public void ItemUse() // 버튼 클릭 함수
    {
        if (selectItem == null)
            return;

        // 1. 배틀 상태
        if (GameManager.Manager.State == GameState.Battle)
        {
            popUpWindow.gameObject.SetActive(false); // use/dump 팝업 끄기
            isPopUp = false;
            if (selectItem.ItemTableElem.type == "INSTALLATION")
            {
                // 설치류 아이템일경우 동작
                bm.costLink.trapSelector.WaitUntilTrapTileSelect(selectItem);
            }
            else if (selectItem.ItemTableElem.stat_Hp > 0)
            {
                // 소비 아이템일경우 동작
                bm.costLink.CostForRecovery(selectItem); // 회복 코루틴 동작 후 알아서 배틀 커맨드 증가시킬것임.
            }
        }
        // 2. 그 밖의 상태
        else
        {
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
    }



    public void ItemDump() // 버튼 클릭 함수
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

    public void ItemBurn()
    {
        if (selectItem == null)
            return;
        if (selectItem.ItemTableElem.isBurn == true)
        {
            var allItem = new DataAllItem(selectItem);
            allItem.OwnCount = 1;
            var bonminute = Vars.UserData.uData.BonfireHour * 60;
            bonminute += selectItem.ItemTableElem.burn_recovery;
            Vars.UserData.uData.BonfireHour = bonminute / 60;

            if (Vars.UserData.RemoveItemData(allItem))
            {
                popUpWindow.gameObject.SetActive(false);
                selectItem = null;
                isPopUp = false;
            }
            CampManager.Instance.SetBonTime();
            ItemListInit();
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
