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
    [SerializeField] private Image disableImg;
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

    private bool isDisable;
    public void DisableItem(bool disable)
    {
        isDisable = disable;
        if(isDisable)
        {
            disableImg.color = Color.white;
        }
        else
        {
            disableImg.color = Color.clear;
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
                disableImg.color = Color.red;
            else
                disableImg.color = Color.white;
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
            DisableItem(false);
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
                if(BattleManager.Instance.isBluemoonSet)
                {
                    // 블루문일 경우
                    if (!(elem.type == "INSTALLATION"))
                    {
                        DisableItem(true);
                    }
                }
                else
                {
                    // 블루문이 아닐경우
                    if (BattleManager.Instance.FSM.curState == BattleState.Start
                        && BattleManager.Instance.isPlayerFirst)
                    {
                        if (!(elem.type == "INSTALLATION" && (elem.id != $"ITEM_{18}")))
                        {
                            DisableItem(true);
                        }

                    }
                    else if (BattleManager.Instance.FSM.curState == BattleState.Player)
                    {
                        if (elem.stat_Hp <= 0)
                        {
                            DisableItem(true);
                        }
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

    public void GatheringInvenButtonClick()
    {
        if (dataItem == null)
            return;

        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;

        var gatheringInven = DiaryInventory.Instance;
        gatheringInven.info.Init(dataItem);

        gatheringInven.isPopUp = true;
        gatheringInven.popUpWindow.gameObject.SetActive(true);
        uiVec = gatheringInven.popUpWindow.position;
        newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
        gatheringInven.popUpWindow.position = newVector;

        gatheringInven.selectItem = dataItem;
        gatheringInven.selectItemSlotNum = slotNum;
        gatheringInven.selectedItemRect = gameObject.GetComponent<RectTransform>();
        gatheringInven.itemButtons.ForEach(n => n.SelectActive(false));
        SelectActive(true);
    }

    public void DiaryItemButtonClick()
    {
        if (dataItem == null)
            return;

        if (tm != null && tm.IsWaitingToSelectTrapTile) // 설치 코루틴 동작 중
            return;

        if (tm != null && tm.IsWaitingToHeal) // 포션 코루틴 동작 중
            return;

        Vector3 uiVec = Vector3.zero;
        Vector3 uiVec2 = Vector3.zero;
        Vector3 newVector = Vector3.zero;
        Vector3 newVector2 = Vector3.zero;
        DungeonRewardDiaryManager.Instance.info.Init(dataItem);
        Debug.Log(GameManager.Manager.State);
        switch (GameManager.Manager.State)
        {
            case GameState.Battle:
                if (BattleManager.Instance.FSM.curState == BattleState.Start && !isDisable
                    || BattleManager.Instance.FSM.curState == BattleState.Player && !isDisable)
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
                    // 선택초기화
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
                    BottomUIManager.Instance.isPopUp = true;
                    BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
                    uiVec = BottomUIManager.Instance.popUpWindow.position;
                    newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
                    BottomUIManager.Instance.selectItem = dataItem;
                    BottomUIManager.Instance.selectItemSlotNum = slotNum;
                    BottomUIManager.Instance.selectedItemRect = gameObject.GetComponent<RectTransform>();
                    newVector2 = new Vector3(transform.position.x, uiVec2.y, uiVec2.z);
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

        if (tm != null && tm.IsWaitingToSelectTrapTile) // 설치 코루틴 동작 중
            return;

        if (tm != null && tm.IsWaitingToHeal) // 포션 코루틴 동작 중
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
                if ((battleManager.FSM.curState == BattleState.Start
                    || battleManager.FSM.curState == BattleState.Player) && !isDisable)
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
                    // 선택초기화
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