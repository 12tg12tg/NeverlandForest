using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class DungeonRewardDiaryManager : MonoBehaviour
{
    private static DungeonRewardDiaryManager instance;
    public static DungeonRewardDiaryManager Instacne => instance;
    [Header("�±װ���")]
    public Image battleTagImage;
    public Image huntTagImage;
    public Image gatheringInDungeonTagImage;

    [Header("�ǳڰ���")]
    public GameObject battleRewardPanel;
    public GameObject huntRewardPanel;
    public GameObject gatheringInDungeonPanel;
    public GameObject gatheringInDungeonRewardPanel;
    [Header("���̾�κ��丮")]
    public DiaryInventory gatheringInDungeonrewardInventory;

    [Header("����")]
    public GameObject reward;
    public ReconfirmPanelManager popupPanel;
    public List<RewardObject> selectedItemList = new List<RewardObject>();

    public void Awake()
    {
        instance = this;
        gatheringInDungeonrewardInventory.ItemButtonInit();
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
            popupPanel.gameObject.SetActive(true);
            popupPanel.rewardNotEmptyPopup.SetActive(true);
        }
    }

    public void GetAllItem()
    {
        var items = reward.GetComponentsInChildren<RewardObject>();
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i].Item != null)
            {
                Vars.UserData.AddItemData(items[i].Item);
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
            Debug.Log("���õ� �������� �����ϴ�");
            return;
        }
        selectedItemList.ForEach(x => {
            Vars.UserData.AddItemData(x.Item);
            Vars.UserData.ExperienceListAdd(x.Item.itemId);
            x.InitItemSprite();
        });
        gatheringInDungeonrewardInventory.ItemButtonInit();
    }

    public void QuitContents() => SceneManager.LoadScene("AS_RandomMap");
}
