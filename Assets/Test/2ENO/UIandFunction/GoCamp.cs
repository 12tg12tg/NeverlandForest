using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GoCamp : MonoBehaviour
{
    private bool haveWoodChip;
    private bool haveTreeBranch;
    private AllItemDataTable allitem;

    [Header("재확인")]
    public ReconfirmPanelManager reconfirmPanel;
    public GameObject godungeonReconfirm;
    [Header("이미지 관련")]
    public Image woodchipImage;
    public Image treeBranchImage;
    [Header("움직임 관련")]
    public MoveTest moveTest;

    public void Start()
    {
        allitem = DataTableManager.GetTable<AllItemDataTable>();
        haveWoodChip = false;
        haveTreeBranch = false;
        var woodchipStringId = $"ITEM_{1}";
        var treebranchStringId = $"ITEM_{2}";
        woodchipImage.sprite = allitem.GetData<AllItemTableElem>(woodchipStringId).IconSprite;
        treeBranchImage.sprite = allitem.GetData<AllItemTableElem>(treebranchStringId).IconSprite;
    }
    public void OpenCampScene()
    {
        reconfirmPanel.gameObject.SetActive(true);
        godungeonReconfirm.SetActive(true);
        moveTest.gameObject.SetActive(false);
        var list = Vars.UserData.HaveAllItemList;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemId == "ITEM_1") //나무토막
            {
                if (list[i].OwnCount >= 3)
                {
                    Debug.Log("나무토막 3개이상있음");
                    haveWoodChip = true;
                }
                else
                {
                    Debug.Log("나무토막은 있는데 3개까지는 없음");
                    Debug.Log("재료가 부족합니다");
                }
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemId == "ITEM_2") //나무가지
            {
                if (list[i].OwnCount >= 6)
                {
                    Debug.Log("나무가지 6개이상있음");
                    haveTreeBranch = true;
                }
                else
                {
                    Debug.Log("나무가지은 있는데 6개까지는 없음");
                    Debug.Log("재료가 부족합니다");
                }
            }
        }
    }

    public void UseWoodChips()
    {
        var list = Vars.UserData.HaveAllItemList;

        if (haveWoodChip)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].itemId == "ITEM_1")
                {
                    var item = new DataAllItem(list[i]);
                    item.OwnCount = 3;
                    Vars.UserData.RemoveItemData(item);

                    if (BottomUIManager.Instance != null)
                    {
                        BottomUIManager.Instance.ItemButtonInit();
                    }
                    GoToCamp();
                }
            }
        }
    }
    public void UseTreeBranch()
    {
        var list = Vars.UserData.HaveAllItemList;

        if (haveTreeBranch)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].itemId == "ITEM_2")
                {
                    list[i].OwnCount = 6;
                    Vars.UserData.RemoveItemData(list[i]);
                    if (BottomUIManager.Instance != null)
                    {
                        BottomUIManager.Instance.ItemButtonInit();
                    }
                    GoToCamp();
                }
            }
        }
    }

    public void GoToCamp()
    {
        DungeonSystem.Instance.DungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
        DungeonSystem.Instance.DungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = DungeonSystem.Instance.DungeonSystemData;

        SceneManager.LoadScene("JYK_Test_Main");
    }

}
