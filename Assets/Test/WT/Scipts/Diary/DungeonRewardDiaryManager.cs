using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class DungeonRewardDiaryManager : MonoBehaviour
{
    private static DungeonRewardDiaryManager instance;
    public static DungeonRewardDiaryManager Instance => instance;
    [Header("태그관련")]
    public Image battleTagImage;
    public Image huntTagImage;
    public Image gatheringInDungeonTagImage;

    [Header("판넬관련")]
    public GameObject battleRewardPanel;
    public GameObject huntRewardPanel;
    public GameObject gatheringInDungeonPanel;
    public GameObject gatheringInDungeonRewardPanel;
    [Header("다이어리인벤토리")]
    public DiaryInventory gatheringInDungeonrewardInventory;

    [Header("보상")]
    public GameObject reward;
    public ReconfirmPanelManager popupPanel;
    public List<RewardObject> selectedItemList = new List<RewardObject>();

    [Header("리워드 다이어리 정보")]
    public BottomInfoUI info;

    public void Awake()
    {
        instance = this;
        gatheringInDungeonrewardInventory.ItemButtonInit();
        //Close();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void AllClose()
    {
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        gatheringInDungeonRewardPanel.SetActive(false);
    }
    public void OpenBattleReward()
    {
        AllClose();
        battleRewardPanel.SetActive(true);
    }
    public void OpenHuntReward()
    {
        AllClose();
        huntRewardPanel.SetActive(true);
    }
    public void OpenGatheringInDungeon()
    {
        AllClose();
        gatheringInDungeonPanel.SetActive(true);
    }
    public void OpenGatheringInDungeonReward()
    {
        gatheringInDungeonRewardPanel.SetActive(true);
        gatheringInDungeonrewardInventory.ItemButtonInit();
    }
   
    public void CheckRewardEmpty()
    {
        var items = reward.GetComponentsInChildren<RewardObject>();
        var isEmpty = false;
        for (int i = 0; i < items.Length; i++)
        {
            isEmpty = items[i].Item == null;
            if (!isEmpty)
                break;
        }

        if(isEmpty)
        {
            QuitContents();
            gameObject.SetActive(false);
        }
        else
        {
            PopupPanelOpne(false, true);
        }
    }

    public void GetAllItem()
    {
        var items = reward.GetComponentsInChildren<RewardObject>();
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i].Item != null)
            {
                var isInvenNotFull = Vars.UserData.AddItemData(items[i].Item);
                if (!isInvenNotFull)
                {
                    PopupPanelOpne(true);
                    return;
                }
                Vars.UserData.ExperienceListAdd(items[i].Item.itemId);
                items[i].InitItemSprite();
            }
        }
        gatheringInDungeonrewardInventory.ItemButtonInit();
    }

    public void GetSelectedItem()
    {
        if(selectedItemList.Count == 0)
        {
            Debug.Log("선택된 아이템이 없습니다");
            return;
        }
        selectedItemList.ForEach(x => {
            var isInvenNotFull = Vars.UserData.AddItemData(x.Item);
            if (!isInvenNotFull)
            {
                PopupPanelOpne(true);
                return;
            }
            Vars.UserData.ExperienceListAdd(x.Item.itemId);
            x.InitItemSprite();
        });
        gatheringInDungeonrewardInventory.ItemButtonInit();
    }

    private void PopupPanelOpne(bool isInvenFull = false, bool isrewardEmpty = false)
    {
        popupPanel.gameObject.SetActive(true);
        popupPanel.inventoryFullPopup.SetActive(isInvenFull);
        popupPanel.rewardNotEmptyPopup.SetActive(isrewardEmpty);
    }

    public void QuitContents() => GameManager.Manager.LoadScene(GameScene.Dungeon);
}
