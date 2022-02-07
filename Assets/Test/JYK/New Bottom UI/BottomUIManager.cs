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
    [Header("���� ���� â ����")]
    public BottomInfoUI info;
    [Header("���� 12�� ������ ����")]
    public List<BottomSkillButtonUI> skillButtons;
    public List<BottomItemButtonUI> itemButtons;
    public BottomSkillButtonUI arrowInfo;
    public BottomSkillButtonUI oilInfo;

    [Header("���� �� �� �з� �±� ����")]
    public List<Button> tags;

    //Vars
    [HideInInspector] public ButtonState buttonState;
    [HideInInspector] public BottomSkillButtonUI curSkillButton;

    public bool IsSkillLock { get; set; } = true;

    private bool isBoySkillDone;
    private bool isGirlSkillDone;

    //PopUpWindow
    [Header("�˾� On/Off Ȯ��")] public bool isPopUp;
    [Header("�˾� ���� ����")]
    public RectTransform popUpWindow;
    public DataAllItem selectItem;
    public RectTransform selectedItemRect;
    public GameObject burnButton;

    [Header("������ ���� ����")]
    public int selectItemSlotNum;

    [Header("���� ���̾ �г�")]
    public GameObject dungeonDiaryPanel;

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

    // ��ų�Ҹ������ ������Ʈ
    public void UpdateCostInfo()
    {
        arrowInfo.UpdateCostInfo();
        oilInfo.UpdateCostInfo();
    }

    // ��ư Ȱ��ȭ / ��Ȱ��ȭ
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

    // ��ų ��ư
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

    public void SkillButtonInit() // ��ų������ 12�� ���� : �±� ��ư + �ڵ� Ȱ��ȭ�� ���� �Լ�
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

    // ������ ��ư
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
        // �̰� Create�Ҷ� �ӽø���Ʈ �����ؼ� ������̱� �ѵ� ���� ȣ������� �� �����Ҽ���
        //var list = Vars.UserData.HaveAllItemList.ToList();
        var list1 = CreateDivideItemList(Vars.UserData.HaveAllItemList.ToList());
        SortItemList(list1);
        int count = list1.Count;

        for (int i = 0; i < count; i++)
        {
            itemButtons[i].Init(list1[i]);
        }
    }

    public void ItemUse() // ��ư Ŭ�� �Լ�
    {
        if (selectItem == null)
            return;

        // 1. ��Ʋ ����
        if (GameManager.Manager.State == GameState.Battle)
        {
            popUpWindow.gameObject.SetActive(false); // use/dump �˾� ����
            isPopUp = false;
            if (selectItem.ItemTableElem.type == "INSTALLATION")
            {
                // ��ġ�� �������ϰ�� ����
                bm.costLink.trapSelector.WaitUntilTrapTileSelect(selectItem);
            }
            else if (selectItem.ItemTableElem.stat_Hp > 0)
            {
                // �Һ� �������ϰ�� ����
                bm.costLink.CostForRecovery(selectItem); // ȸ�� �ڷ�ƾ ���� �� �˾Ƽ� ��Ʋ Ŀ�ǵ� ������ų����.
            }
        }
        // 2. �� ���� ����
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

    public void OpenDungeonDiaryButton()
    {
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
                break;
            case GameState.Dungeon:
                gameObject.SetActive(false);
                dungeonDiaryPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

}
