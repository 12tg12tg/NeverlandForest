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
    public ReconfirmPanelManager popupPanel;
    public List<RewardObject> selectedItemList = new List<RewardObject>();

    [Header("������ ���̾ ����")]
    public BottomInfoUI info;

    [Header("������ 6ĭ ����")]
    [SerializeField] private List<RewardObject> rewardSlots;

    public void Awake()
    {
        instance = this;
        gatheringInDungeonrewardInventory.ItemButtonInit();

        //if (GameManager.Manager.State == GameState.Dungeon)
        //{
        //    Close();
        //}
    }
    public void Close()
    {
        gameObject.SetActive(false);
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        SoundManager.Instance.Play(SoundType.Se_Diary);
    }
    public void OpenHuntReward()
    {
        AllClose();
        huntRewardPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

    }
    public void OpenGatheringInDungeon()
    {
        AllClose();
        gatheringInDungeonPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);
    }
    public void OpenGatheringInDungeonReward()
    {
        gatheringInDungeonRewardPanel.SetActive(true);
        gatheringInDungeonrewardInventory.ItemButtonInit();
        SoundManager.Instance.Play(SoundType.Se_Diary);
    }

    public void CheckRewardEmpty()
    {
        var isEmpty = false;
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            isEmpty = rewardSlots[i].Item == null;
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
            PopupPanelOpen(false, true);
        }
    }
    public void GetAllItem()
    {
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            if(rewardSlots[i].Item != null)
            {
                var isInvenNotFull = Vars.UserData.AddItemData(rewardSlots[i].Item);
                if (!isInvenNotFull)
                {
                    PopupPanelOpen(true);
                    return;
                }
                Vars.UserData.ExperienceListAdd(rewardSlots[i].Item.itemId);
                rewardSlots[i].InitItemSprite();
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
            var isInvenNotFull = Vars.UserData.AddItemData(x.Item);
            if (!isInvenNotFull)
            {
                PopupPanelOpen(true);
                return;
            }
            Vars.UserData.ExperienceListAdd(x.Item.itemId);
            x.InitItemSprite();
        });
        gatheringInDungeonrewardInventory.ItemButtonInit();
    }

    private void PopupPanelOpen(bool isInvenFull = false, bool isrewardEmpty = false)
    {
        popupPanel.gameObject.SetActive(true);
        popupPanel.inventoryFullPopup.SetActive(isInvenFull);
        popupPanel.rewardNotEmptyPopup.SetActive(isrewardEmpty);
    }

    public void OpenRewardsPopup(List<DataAllItem> rewards)
    {
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            if (i < rewards.Count)
                rewardSlots[i].Init(rewards[i]);
            else
            {
                rewardSlots[i].Init(null);
            }
        }
        SoundManager.Instance?.Play(SoundType.Se_Diary);
    }

    public void QuitContents() => GameManager.Manager.LoadScene(GameScene.Dungeon);
}
