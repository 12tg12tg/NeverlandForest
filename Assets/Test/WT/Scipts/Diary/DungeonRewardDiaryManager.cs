using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Awake()
    {
        instance = this;
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
            popupPanel.rewardNotEmptyPopup.SetActive(true);
        }
    }

    public void QuitContents()
    {

    }

}
