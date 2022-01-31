using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GoCamp : MonoBehaviour
{
    private bool haveWoodChip;
    private bool haveTreeBranch;

    public ReconfirmPanelManager reconfirmPanel;
    public GameObject godungeonReconfirm;

    public Image woodchipImage;
    public Image treeBranchImage;
    public MoveTest moveTest;
    AllItemDataTable allitem;

    public void Awake()
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
            if (list[i].itemId == "ITEM_1") //�����丷
            {
                if (list[i].OwnCount >= 3)
                {
                    Debug.Log("�����丷 3���̻�����");
                    haveWoodChip = true;
                }
                else
                {
                    Debug.Log("�����丷�� �ִµ� 3�������� ����");
                    Debug.Log("��ᰡ �����մϴ�");
                }
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemId == "ITEM_2") //��������
            {
                if (list[i].OwnCount >= 6)
                {
                    Debug.Log("�������� 6���̻�����");
                    haveTreeBranch = true;
                }
                else
                {
                    Debug.Log("���������� �ִµ� 6�������� ����");
                    Debug.Log("��ᰡ �����մϴ�");
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
