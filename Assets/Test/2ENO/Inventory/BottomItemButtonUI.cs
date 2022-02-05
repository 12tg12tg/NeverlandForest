using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BottomItemButtonUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image selectedImg;
    [SerializeField] private Image type;
    [SerializeField] private Button ownButton;
    [SerializeField] private Image shadeCover;
    private int slotNum;

    private bool isSelect;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
                selectedImg.color = Color.red;
            else
                selectedImg.color = Color.green;
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
        slotNum = slot;

        count.color = Color.black;
        if (data == null)
        {
            dataItem = null;
            icon.sprite = null;
            count.text = string.Empty;
            IsSelect = false;
            IsConsumable = false;
            IsInstallation = false;
            return;
        }

        dataItem = data;
        AllItemTableElem elem = data.ItemTableElem;
        icon.sprite = elem.IconSprite;
        count.text = data.OwnCount.ToString();

        if(RandomEventUIManager.Instance != null &&
            RandomEventUIManager.Instance.isRaneomEventOn)
        {
            if (RandomEventUIManager.Instance.selectInvenItem != null &&
            RandomEventUIManager.Instance.selectInvenItem.itemId == dataItem.itemId
            && RandomEventUIManager.Instance.selectSlotNum == slotNum)
            {
                IsSelect = true;
            }
        }
        else
        {
            if (BottomUIManager.Instance.selectItem != null &&
            BottomUIManager.Instance.selectItem.itemId == dataItem.itemId
            && BottomUIManager.Instance.selectItemSlotNum == slotNum)
            {
                IsSelect = true;
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

    public void ItemButtonClick()
    {
        if (dataItem == null)
            return;

        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;
        BottomUIManager.Instance.info.Init(dataItem);
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
                    BottomUIManager.Instance.itemButtons.ForEach(n => n.isSelect = false);
                    IsSelect = true;
                }
                break;
            case GameState.Hunt:
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
                BottomUIManager.Instance.itemButtons.ForEach(n => n.isSelect = false);
                IsSelect = true;
                if (DiaryInventory.Instance != null)
                {
                    DiaryInventory.Instance.info.Init(dataItem);
                    // 선택초기화
                    for (int i = 0; i < DiaryInventory.Instance.itemButtons.Count; i++)
                    {
                        DiaryInventory.Instance.itemButtons[i].IsSelect = false;
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
                    RandomEventUIManager.Instance.itemButtons.ForEach(n => n.isSelect = false);
                    RandomEventUIManager.Instance.itemButtons2page.ForEach(n => n.isSelect = false);
                    IsSelect = true;
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
                    BottomUIManager.Instance.itemButtons.ForEach(n => n.isSelect = false);
                    IsSelect = true;
                }
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