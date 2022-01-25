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


    private void Awake()
    {
        instance = this;
        popUpWindow.gameObject.SetActive(false);
        SkillButtonInit();
    }

    // ���α׷���
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

    // ��ų ��ư
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

    public void SkillButtonInit() // ��ų������ 12�� ���� : �±� ��ư + �ڵ� Ȱ��ȭ�� ���� �Լ�
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

    // ������ ��ư
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
