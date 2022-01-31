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
    public List<Button> tags;
    public GameObject progress;
    [SerializeField] private List<Image> progressImg;
    public List<BottomItemButtonUI> itemButtons;

    //Vars
    [HideInInspector] public ButtonState buttonState;
    [HideInInspector] public BottomSkillButtonUI curSkillButton;
    [HideInInspector] public ObstacleType curObstacleType;

    private bool isBoySkillDone;
    private bool isGirlSkillDone;

    //PopUpWindow
    public bool isPopUp;
    public RectTransform popUpWindow;
    public DataAllItem selectItem;
    public RectTransform selectedItemRect;

    private void Awake()
    {
        instance = this;
        popUpWindow.gameObject.SetActive(false);
        SkillButtonInit();
    }

    // 프로그래스
    public void UpdateProgress()
    {
        int prog = BattleManager.Instance.Progress;
        if (prog == 0)
        {
            progressImg[0].enabled = false;
            progressImg[1].enabled = false;
        }
        else if(prog == 1)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = false;
        }
        else if(prog == 2)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = true;
        }
    }

    // 스킬 버튼
    public void InteractiveSkillButton(PlayerType type, bool interactive)
    {
        if(type == PlayerType.Boy)
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
            if (n.skill?.SkillTableElem.player == PlayerType.Boy)
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
        //Time.timeScale = 0.2f;
        this.curSkillButton = skillButton;
        BattleManager.Instance.ReadyTileClick();
        BattleManager.Instance.DisplayMonsterTile(curSkillButton.skill.SkillTableElem.range);
        skillButtons.ForEach(n => { if (n != curSkillButton) n.MakeUnclickable(); });
        tags.ForEach(n => n.interactable = false);
    }

    public void ExitSkillState()
    {
        //Time.timeScale = 1f;
        curSkillButton = null;
        BattleManager.Instance.EndTileClick();
        BattleManager.Instance.UndisplayMonsterTile();
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
        return RectTransformUtility.RectangleContainsScreenPoint(popUpWindow, pos);
    }
    private bool IsContainItemRect(Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(selectedItemRect, pos);
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

    // 아이템 버튼
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

    public void ItemBurn()
    {
        if (selectItem.ItemTableElem.isBurn ==true)
        {
            if (selectItem == null)
                return;

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
