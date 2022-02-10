using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BottomItemButtonUI : MonoBehaviour
{
    private TileMaker tm => TileMaker.Instance;
    private BattleManager bm => BattleManager.Instance;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image selectedImg;
    [SerializeField] private Image type;
    [SerializeField] private Button ownButton;
    [SerializeField] private Image shadeCover;
    private int slotNum;

    private bool isSelect;
    public void SelectActive(bool isActive)
    {
        isSelect = isActive;
        if (isSelect)
        {
            selectedImg.color = Color.white;
        }
        else
        {
            selectedImg.color = Color.clear;
        }
    }



    private bool isInstallation;
    public bool IsInstallation
    {
        get => isInstallation;
        set
        {
            isInstallation = value;
            if(isInstallation)
                type.color = Color.blue;
            else
                type.color = Color.white;
        }
    }

    private bool isConsumable;
    public bool IsConsumable
    {
        //get
        //{
        //    if (dataItem != null)
        //    {
        //        if (dataItem.ItemTableElem.stat_Hp > 0)
        //        {
        //            isConsumable = true;
        //            type.color = Color.gray;
        //        }
        //        else
        //        {
        //            isConsumable = false;
        //        }
        //    }
        //    return isConsumable;
        //}
        get => isConsumable;
        set
        {
            isConsumable = value;
            if (isConsumable)
                type.color = Color.gray;
            else
                type.color = Color.white;
        }
    }
    private bool isBurn;
    public bool IsBurn
    {
        get => isBurn;
        set
        {
            isBurn = value;
            if (isBurn)
                type.color = Color.red;
            else
                type.color = Color.white;
        }
    }


    private DataAllItem dataItem;

    public DataAllItem DataItem { get => dataItem; }

    public void Init(DataAllItem data, int slot = -1)
    {
        var RandomEventUImanager = RandomEventUIManager.Instance;

        slotNum = slot;

        count.color = Color.black;
        if (data == null)
        {
            dataItem = null;
            icon.sprite = null;
            icon.color = Color.clear;
            count.text = string.Empty;
            SelectActive(false);
            IsConsumable = false;
            IsInstallation = false;
            return;
        }

        dataItem = data;
        AllItemTableElem elem = data.ItemTableElem;
        icon.sprite = elem.IconSprite;
        icon.color = Color.white;
        count.text = data.OwnCount.ToString();

        if(RandomEventUImanager != null &&
            RandomEventUImanager.isRaneomEventOn)
        {
            if (RandomEventUImanager.selectInvenItem != null &&
            RandomEventUImanager.selectInvenItem.itemId == dataItem.itemId
            && RandomEventUImanager.selectSlotNum == slotNum)
            {
                SelectActive(true);
            }
        }
        else
        {
            if (BottomUIManager.Instance.selectItem != null &&
            BottomUIManager.Instance.selectItem.itemId == dataItem.itemId
            && BottomUIManager.Instance.selectItemSlotNum == slotNum)
            {
                SelectActive(true);
            }
        }

        switch (GameManager.Manager.State)
        {
            case GameState.Battle:
                if (BattleManager.Instance.FSM.curState == BattleState.Start
                     && BattleManager.Instance.isPlayerFirst)
                {
                    if (elem.type == "INSTALLATION" && (elem.id != $"ITEM_{18}"))
                    {
                        IsInstallation = true;
                    }
                }
                else if(BattleManager.Instance.FSM.curState == BattleState.Player)
                {
                    if (elem.stat_Hp > 0)
                    {
                        IsConsumable = true;
                    }
                }
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
                break;
        }


    }

    public void Interactive(bool interactive)
    {
        ownButton.interactable = interactive;

        var color = shadeCover.color;
        color.a = interactive ? 0f : 0.3f;

        shadeCover.color = color;
    }
    public void DiaryItemButtonClick()
    {
        if (dataItem == null)
            return;

        if (tm != null && tm.IsWaitingToSelectTrapTile) // ��ġ �ڷ�ƾ ���� ��
            return;

        if (tm != null && tm.IsWaitingToHeal) // ���� �ڷ�ƾ ���� ��
            return;

        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;
        DungeonRewardDiaryManager.Instance.info.Init(dataItem);
        Debug.Log(GameManager.Manager.State);
        switch (GameManager.Manager.State)
        {
            case GameState.Battle:
                if (BattleManager.Instance.FSM.curState == BattleState.Start && IsInstallation
                    || BattleManager.Instance.FSM.curState == BattleState.Player && IsConsumable)
                {
                    BottomUIManager.Instance.selectItem = dataItem;
                    BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                    BottomUIManager.Instance.isPopUp = true;
                    BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                    uiVec = BottomUIManager.Instance.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    BottomUIManager.Instance.popUpWindow.position = newVector;
                    BottomUIManager.Instance.itemButtons.ForEach(n => n.SelectActive(false));
                    SelectActive(true);
                }
                break;
            case GameState.Hunt:
                BottomUIManager.Instance.selectItem = dataItem;
                BottomUIManager.Instance.selectItemSlotNum = slotNum;
                BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                BottomUIManager.Instance.isPopUp = true;
                BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                uiVec = BottomUIManager.Instance.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                BottomUIManager.Instance.popUpWindow.position = newVector;
                DungeonRewardDiaryManager.Instance.gatheringInDungeonrewardInventory.itemButtons.ForEach(n => n.SelectActive(false));
                SelectActive(true);
                break;
            case GameState.Gathering:
                break;
            case GameState.Cook:
                break;
            case GameState.Camp:
                BottomUIManager.Instance.selectItem = dataItem;
                BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                BottomUIManager.Instance.isPopUp = true;
                BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                uiVec = BottomUIManager.Instance.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                BottomUIManager.Instance.popUpWindow.position = newVector;
                BottomUIManager.Instance.itemButtons.ForEach(n => n.SelectActive(false));
                SelectActive(true);
                if (DiaryInventory.Instance != null)
                {
                    DiaryInventory.Instance.info.Init(dataItem);
                    // �����ʱ�ȭ
                    for (int i = 0; i < DiaryInventory.Instance.itemButtons.Count; i++)
                    {
                        DiaryInventory.Instance.itemButtons[i].SelectActive(false);
                    }
                }
                break;
            case GameState.Dungeon:
                if (RandomEventUIManager.Instance != null &&
            RandomEventUIManager.Instance.isRaneomEventOn)
                {
                    RandomEventUIManager.Instance.info.Init(dataItem);
                    RandomEventUIManager.Instance.info2page.Init(dataItem);

                    RandomEventUIManager.Instance.selectInvenItem = dataItem;
                    RandomEventUIManager.Instance.selectSlotNum = slotNum;
                    RandomEventUIManager.Instance.itemBox = gameObject.GetComponent<RectTransform>();
                    RandomEventUIManager.Instance.isPopUp = true;
                    uiVec = RandomEventUIManager.Instance.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    RandomEventUIManager.Instance.popUpWindow.position = newVector;
                    RandomEventUIManager.Instance.itemButtons.ForEach(n => n.SelectActive(false));
                    RandomEventUIManager.Instance.itemButtons2page.ForEach(n => n.SelectActive(false));
                    SelectActive(true);
                }
                else
                {
                    BottomUIManager.Instance.selectItem = dataItem;
                    BottomUIManager.Instance.selectItemSlotNum = slotNum;
                    BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                    BottomUIManager.Instance.isPopUp = true;
                    BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                    uiVec = BottomUIManager.Instance.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    BottomUIManager.Instance.popUpWindow.position = newVector;
                    BottomUIManager.Instance.itemButtons.ForEach(n => n.SelectActive(false));
                    SelectActive(true);
                }
                break;
        }
    }
    public void ItemButtonClick()
    {
        if (dataItem == null)
            return;

        if (tm != null && tm.IsWaitingToSelectTrapTile) // ��ġ �ڷ�ƾ ���� ��
            return;

        if (tm != null && tm.IsWaitingToHeal) // ���� �ڷ�ƾ ���� ��
            return;

        if (bm != null && bm.isTutorial && !bm.tutorial.tu_03_TrapClick1)
            bm.tutorial.tu_03_TrapClick1 = true;

        var battleManager = BattleManager.Instance;
        var bottomUImanager = BottomUIManager.Instance;
        var RandomEventUImanager = RandomEventUIManager.Instance;
        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;
        BottomUIManager.Instance.info.Init(dataItem);
        switch (GameManager.Manager.State)
        {
            case GameState.Battle:
                if (battleManager.FSM.curState == BattleState.Start && IsInstallation
                    || battleManager.FSM.curState == BattleState.Player && IsConsumable)
                {
                    bottomUImanager.selectItem = dataItem;
                    bottomUImanager.popUpWindow.gameObject.SetActive(true);
                    bottomUImanager.isPopUp = true;
                    bottomUImanager.selectedItemRect = gameObject.GetComponent<RectTransform>();
                    uiVec = bottomUImanager.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    bottomUImanager.popUpWindow.position = newVector;
                    bottomUImanager.itemButtons.ForEach(n => n.SelectActive(false));
                    SelectActive(true);
                }
                break;
            case GameState.Hunt:
                BottomUIManager.Instance.selectItem = dataItem;
                BottomUIManager.Instance.selectItemSlotNum = slotNum;
                BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                BottomUIManager.Instance.isPopUp = true;
                BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                uiVec = BottomUIManager.Instance.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                BottomUIManager.Instance.popUpWindow.position = newVector;
                BottomUIManager.Instance.itemButtons.ForEach(n => n.SelectActive(false));
                SelectActive(true);
                break;
            case GameState.Gathering:
                break;
            case GameState.Cook:
                break;
            case GameState.Camp:
                bottomUImanager.selectItem = dataItem;
                bottomUImanager.popUpWindow.gameObject.SetActive(true);
                bottomUImanager.isPopUp = true;
                bottomUImanager.selectedItemRect = gameObject.GetComponent<RectTransform>();
                uiVec = bottomUImanager.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                bottomUImanager.popUpWindow.position = newVector;
                bottomUImanager.itemButtons.ForEach(n => n.SelectActive(false));
                SelectActive(true);
                if (DiaryInventory.Instance != null)
                {
                    DiaryInventory.Instance.info.Init(dataItem);
                    // �����ʱ�ȭ
                    for (int i = 0; i < DiaryInventory.Instance.itemButtons.Count; i++)
                    {
                        DiaryInventory.Instance.itemButtons[i].SelectActive(false);
                    }
                }
                break;
            case GameState.Dungeon:
                if (RandomEventUImanager != null &&
            RandomEventUImanager.isRaneomEventOn)
                {
                    RandomEventUImanager.info.Init(dataItem);
                    RandomEventUImanager.info2page.Init(dataItem);

                    RandomEventUImanager.selectInvenItem = dataItem;
                    RandomEventUImanager.selectSlotNum = slotNum;
                    RandomEventUImanager.itemBox = gameObject.GetComponent<RectTransform>();
                    RandomEventUImanager.isPopUp = true;
                    uiVec = RandomEventUImanager.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    RandomEventUImanager.popUpWindow.position = newVector;
                    RandomEventUImanager.itemButtons.ForEach(n => n.SelectActive(false));
                    RandomEventUImanager.itemButtons2page.ForEach(n => n.SelectActive(false));
                    SelectActive(true);
                }
                else
                {
                    bottomUImanager.selectItem = dataItem;
                    bottomUImanager.selectItemSlotNum = slotNum;
                    bottomUImanager.popUpWindow.gameObject.SetActive(true);
                    bottomUImanager.isPopUp = true;
                    bottomUImanager.selectedItemRect = gameObject.GetComponent<RectTransform>();
                    uiVec = bottomUImanager.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    bottomUImanager.popUpWindow.position = newVector;
                    bottomUImanager.itemButtons.ForEach(n => n.SelectActive(false));
                    SelectActive(true);
                }
                break;
            case GameState.Tutorial:
                bottomUImanager.selectItem = dataItem;
                bottomUImanager.selectItemSlotNum = slotNum;
                bottomUImanager.popUpWindow.gameObject.SetActive(true);
                bottomUImanager.isPopUp = true;
                bottomUImanager.selectedItemRect = gameObject.GetComponent<RectTransform>();
                uiVec = bottomUImanager.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                bottomUImanager.popUpWindow.position = newVector;
                bottomUImanager.itemButtons.ForEach(n => n.SelectActive(false));
                SelectActive(true);
                break;
        }
    }
}

//    public void Init(DataConsumable data)
//{
//    dataItem = data;
//    ConsumableTableElem elem = data.ItemTableElem;

//    icon.sprite = elem.IconSprite;
//    count.text = data.OwnCount.ToString();
//    itemName.name = elem.name;
//}
//public void Init(DataMaterial data)
//{
//    dataItem = data;
//    AllItemTableElem elem = data.ItemTableElem;

//    icon.sprite = elem.IconSprite;
//    count.text = data.OwnCount.ToString();
//    itemName.name = elem.name;
//}