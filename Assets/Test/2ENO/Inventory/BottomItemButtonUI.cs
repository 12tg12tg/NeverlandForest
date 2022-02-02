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

    public void Init(DataAllItem data)
    {
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

        if (BottomUIManager.Instance.selectItem != null &&
        BottomUIManager.Instance.selectItem.itemId == dataItem.itemId)
        {
            IsSelect = true;
        }
        switch (GameManager.Manager.State)
        {
            case GameState.Battle:
                if (BattleManager.Instance.FSM.curState == BattleState.Start)
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

    public void ItemButtonClick()
    {
        if (dataItem == null)
            return;

        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;
        switch (GameManager.Manager.State)
        {
            case GameState.Battle:
                if(BattleManager.Instance.FSM.curState == BattleState.Start && IsInstallation
                    || BattleManager.Instance.FSM.curState == BattleState.Player && IsConsumable)
                {
                    BottomUIManager.Instance.info.Init(dataItem);
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
                BottomUIManager.Instance.info.Init(dataItem);
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
                    // �����ʱ�ȭ
                    for (int i = 0; i < DiaryInventory.Instance.itemButtons.Count; i++)
                    {
                        DiaryInventory.Instance.itemButtons[i].IsSelect = false;
                    }
                }
                break;
            case GameState.Dungeon:
                BottomUIManager.Instance.info.Init(dataItem);
                BottomUIManager.Instance.selectItem = dataItem;
                BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                BottomUIManager.Instance.isPopUp = true;
                BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                uiVec = BottomUIManager.Instance.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                BottomUIManager.Instance.popUpWindow.position = newVector;
                BottomUIManager.Instance.itemButtons.ForEach(n => n.isSelect = false);
                IsSelect = true;

                RandomEventUIManager.Instance.info.Init(dataItem);
                RandomEventUIManager.Instance.info2page.Init(dataItem);
                RandomEventUIManager.Instance.selectInvenItem = dataItem;
                RandomEventUIManager.Instance.itemBox = gameObject.GetComponent<RectTransform>();
                RandomEventUIManager.Instance.isPopUp = true;
                uiVec = RandomEventUIManager.Instance.popUpWindow.position;
                newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                RandomEventUIManager.Instance.popUpWindow.position = newVector;
                RandomEventUIManager.Instance.itemButtons.ForEach(n => n.isSelect = false);
                RandomEventUIManager.Instance.itemButtons2page.ForEach(n => n.isSelect = false);
                IsSelect = true;
                break;
        }

       




        // TODO: �ӽ�, ����� �س�����
        if (RandomEventManager.Instance != null && GameManager.Manager.State == GameState.Dungeon)
        {
            //RandomEventUIManager.Instance.info.Init(dataItem);
            //RandomEventUIManager.Instance.info2page.Init(dataItem);
            //RandomEventUIManager.Instance.selectInvenItem = dataItem;
            ////RandomEventUIManager.Instance.popUpWindow.gameObject.SetActive(true);
            //RandomEventUIManager.Instance.itemBox = gameObject.GetComponent<RectTransform>();
            //RandomEventUIManager.Instance.isPopUp = true;

            //var uiVec = RandomEventUIManager.Instance.popUpWindow.position;
            //var newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
            //RandomEventUIManager.Instance.popUpWindow.position = newVector;
            //// �����ʱ�ȭ
            //for (int i = 0; i < RandomEventUIManager.Instance.itemButtons.Count; i++)
            //{
            //    RandomEventUIManager.Instance.itemButtons[i].IsSelect = false;
            //    RandomEventUIManager.Instance.itemButtons2page[i].IsSelect = false;
            //}
            //IsSelect = true;
        }

        else
        {
            //BottomUIManager.Instance.info.Init(dataItem);
            //BottomUIManager.Instance.selectItem = dataItem;
            //BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
            //BottomUIManager.Instance.isPopUp = true;
            //BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();

            //var uiVec = BottomUIManager.Instance.popUpWindow.position;
            //var newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
            //BottomUIManager.Instance.popUpWindow.position = newVector;
            //// �����ʱ�ȭ
            //for (int i = 0; i < BottomUIManager.Instance.itemButtons.Count; i++)
            //{
            //    BottomUIManager.Instance.itemButtons[i].IsSelect = false;
            //}
            //IsSelect = true;
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