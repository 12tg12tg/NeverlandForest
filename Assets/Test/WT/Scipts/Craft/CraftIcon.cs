using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftIcon : MonoBehaviour
{
    public enum CraftButtonType { Tool, Battle, Herb };

    [SerializeField] private List<CraftObj> itemGoList = new List<CraftObj>();
    private CraftDataTable table;
    private AllItemTableElem material0obj;
    private AllItemTableElem material1obj;
    private AllItemTableElem material2obj;
    private bool ismaterial0have;
    private bool ismaterial1have;
    private bool ismaterial2have;
    private int material0Num;
    private int material1Num;
    private int material2Num;
    private DataAllItem fireitem;
    private DataAllItem condimentitem;
    private DataAllItem materialitem;
    private string Time = null;
    private AllItemDataTable ItemTable;

    private int page = 1;
    private CraftButtonType currentButtonType = CraftButtonType.Tool;
    [HideInInspector] public CraftObj currentCraft;
    private int maxPage;
    public Image fire;
    public Image condiment;
    public Image material;

    public TextMeshProUGUI makingTime;
    public string result = string.Empty;

    public Image resultItemIcon;
    public TextMeshProUGUI resultItemName;
    public TextMeshProUGUI resultItemDesc;

    public Image rewardresultItemIcon;
    public TextMeshProUGUI rewardresultItemName;
    public TextMeshProUGUI rewardresultItemDesc;


    [SerializeField] private Button previewButton;
    [SerializeField] private Button nextButton;

    public bool Ismaterial0haveitem
    {
        get => ismaterial0have;
        set
        { ismaterial0have = value; }
    }
    public bool Ismaterial1haveitem
    {
        get => ismaterial1have;
        set { ismaterial1have = value; }
    }
    public bool Ismaterial2haveitem
    {
        get => ismaterial2have;
        set { ismaterial2have = value; }
    }
    public void Start()
    {
        currentButtonType = CraftButtonType.Tool;
        Init();
    }
    public void Init()
    {
        table = DataTableManager.GetTable<CraftDataTable>();
        ItemTable = DataTableManager.GetTable<AllItemDataTable>();
        if (GameManager.Manager.State ==GameState.Tutorial)
        {
            currentButtonType = CraftButtonType.Herb;
        }
       
        var craftList = Vars.UserData.HaveCraftIDList;

        for (int i = 0; i < craftList.Count; i++)
        {
            Debug.Log($"{i}��° ������ Ÿ���� { table.GetData<CraftTableElem>(craftList[i]).type}");
        }

        int type;
        var fillterList = craftList.Where(n =>
        {
            switch (currentButtonType)
            {
                case CraftButtonType.Tool:
                    type = table.GetData<CraftTableElem>(n).type;
                    return (type == 0 || type == 1);
                case CraftButtonType.Battle:
                    type = table.GetData<CraftTableElem>(n).type;
                    return (type == 4);
                case CraftButtonType.Herb:
                    type = table.GetData<CraftTableElem>(n).type;
                    return (type == 5);
                default:
                    return false;
            }
        }).ToList();

        maxPage = fillterList.Count / 5 + 1;

        for (int i = 0; i < 5; i++)
        {
            var index = i + 5 * (page - 1);
            if (index < fillterList.Count)
            {
                itemGoList[i].Init(table, fillterList[index], this);
            }
            else
            {
                itemGoList[i].Clear();
            }
        }
        SetPageButton();
    }
    public void PreviewPageOpen() //��ư�Լ�
    {
        if (page > 1)
        {
            page--;
            SetPageButton();
        }
    }
    public void NextPageOpen() //��ư�Լ�
    {
        if (page < maxPage)
        {
            page++;
            SetPageButton();
        }
    }
    private void SetPageButton()
    {
        if (page == 1)
        {
            previewButton.interactable = false;
        }
        else if (page == maxPage)
        {
            nextButton.interactable = false;
        }
        else
        {
            previewButton.interactable = true;
            nextButton.interactable = true;
        }

    }
    public void OnChangedSelection()
    {
        var fireid = $"ITEM_{(currentCraft.Crafts[0])}";
        var condimentid = $"ITEM_{(currentCraft.Crafts[1])}";
        var materialid = $"ITEM_{(currentCraft.Crafts[2])}";
        var userItemList = Vars.UserData.HaveAllItemList;
        var zeroId = "ITEM_0";

        fire.sprite = ItemTable.GetData<AllItemTableElem>(fireid).IconSprite;
        if (fire.sprite != null)
            fire.color = Color.white;
        else
            fire.color = Color.clear;
        condiment.sprite = ItemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
        if (condiment.sprite != null)
            condiment.color = Color.white;
        else
            condiment.color = Color.clear;
        material.sprite = ItemTable.GetData<AllItemTableElem>(materialid).IconSprite;
        if (material.sprite != null)
            material.color = Color.white;
        else
            material.color = Color.clear;
        material0obj = ItemTable.GetData<AllItemTableElem>(fireid);
        material1obj = ItemTable.GetData<AllItemTableElem>(condimentid);
        material2obj = ItemTable.GetData<AllItemTableElem>(materialid);

        fireitem = new DataAllItem(material0obj);
        fireitem.OwnCount = 1;
        condimentitem = new DataAllItem(material1obj);
        condimentitem.OwnCount = 1;
        materialitem = new DataAllItem(material2obj);
        materialitem.OwnCount = 1;

        if (material1obj.id == zeroId)
        {
            ismaterial1have = true;
        }
        else
        {
            ismaterial1have = false;
        }
        if (material2obj.id == zeroId)
        {
            ismaterial2have = true;
        }
        else
        {
            ismaterial2have = false;
        }

        result = currentCraft.Result;
        var resultid = $"ITEM_{result}";
       
        resultItemIcon.sprite = ItemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        resultItemIcon.color = Color.white;
        resultItemName.text = ItemTable.GetData<AllItemTableElem>(resultid).name;
        resultItemDesc.text = ItemTable.GetData<AllItemTableElem>(resultid).desc;

        rewardresultItemIcon.sprite = ItemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        rewardresultItemIcon.color = Color.white;
        rewardresultItemName.text = ItemTable.GetData<AllItemTableElem>(resultid).name;
        rewardresultItemDesc.text = ItemTable.GetData<AllItemTableElem>(resultid).desc;

        Time = currentCraft.Time;
        makingTime.text = $"���� �ð��� {Time}�� �Դϴ�. ";

        for (int i = 0; i < userItemList.Count; i++)
        {
            if (userItemList[i].ItemTableElem.id == material0obj.id)
            {
                ismaterial0have = true;
                material0Num = i;
            }
        }

        CampManager.Instance.producingText.text = "���� �ϱ�";
    }
    public void MakeProducing()
    {
        ItemTable = DataTableManager.GetTable<AllItemDataTable>();
        var list = Vars.UserData.HaveAllItemList; //�κ��丮
        var zeroId = "ITEM_0";
        if (result != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemTableElem.id == material1obj.id)
                {
                    material1Num = i;
                    ismaterial1have = true;
                }
                if (list[i].ItemTableElem.id == material2obj.id)
                {
                    material2Num = i;
                    ismaterial2have = true;
                }
            }
            if (!ismaterial0have || !ismaterial1have || !ismaterial2have)
            {
                CampManager.Instance.producingText.text = "��ᰡ �����մϴ�";
            }

            var resultid = $"ITEM_{result}";
            var consumeTime = ItemTable.GetData<AllItemTableElem>(resultid).duration;
            var haveBonfireTime = Vars.UserData.uData.BonfireHour * 60;
            int overlap = 0;
            var inventory = Vars.UserData.HaveAllItemList;

            if (currentCraft.Crafts[0] == currentCraft.Crafts[1] && currentCraft.Crafts[1] == currentCraft.Crafts[2])
            {
                overlap = 3;
            }
            else if (currentCraft.Crafts[1] == currentCraft.Crafts[2])
            {    
                // ��� ������ �� 
                overlap = 0;
            }
            else if(currentCraft.Crafts[0] == currentCraft.Crafts[1])
            {
                // ����Ʈ�� �κ�Ʈ��
                overlap = 2;
            }

           
            if (ismaterial0have && ismaterial1have && ismaterial2have && haveBonfireTime >=consumeTime)
            {
                DiaryManager.Instacne.produceInventory.ItemButtonInit();
                DiaryManager.Instacne.CraftResultItem = new DataAllItem(ItemTable.GetData<AllItemTableElem>(resultid));
                if (resultid == "ITEM_2" || resultid == "ITEM_20" || resultid == "ITEM_21")
                {
                    DiaryManager.Instacne.CraftResultItem.OwnCount = 3;
                }
                else
                {
                    DiaryManager.Instacne.CraftResultItem.OwnCount = 1;
                }
                DiaryManager.Instacne.craftResultItemImage.sprite = DiaryManager.Instacne.CraftResultItem.ItemTableElem.IconSprite;

                if (overlap !=0 && inventory[material0Num].OwnCount ==overlap)
                {
                    if (Vars.UserData.AddItemData(DiaryManager.Instacne.CraftResultItem) != false)
                    {
                        Vars.UserData.ExperienceListAdd(DiaryManager.Instacne.CraftResultItem.itemId);
                        ConsumeManager.TimeUp(consumeTime);
                        haveBonfireTime -= consumeTime;
                        Vars.UserData.uData.BonfireHour = haveBonfireTime / 60;
                        if (Vars.UserData.uData.BonfireHour <= 0)
                        {
                            Vars.UserData.uData.BonfireHour = 0;
                        }
                        ConsumeManager.SaveConsumableData();
                        CampManager.Instance.SetBonTime();
                        Vars.UserData.RemoveItemData(fireitem);
                        if (material1obj.id != zeroId)
                        {
                            Vars.UserData.RemoveItemData(condimentitem);
                        }
                        if (material2obj.id != zeroId)
                        {
                            Vars.UserData.RemoveItemData(materialitem);
                        }
                        DiaryManager.Instacne.OpenProduceReward();
                    }
                    else
                    {
                        CampManager.Instance.reconfirmPanelManager.gameObject.SetActive(true);
                        CampManager.Instance.reconfirmPanelManager.inventoryFullPopup.SetActive(true);
                        CampManager.Instance.reconfirmPanelManager.bonfireTimeRemainPopup.SetActive(false);
                    }
                }
                else if (overlap ==0)
                {
                    if (Vars.UserData.AddItemData(DiaryManager.Instacne.CraftResultItem) != false)
                    {
                        Vars.UserData.ExperienceListAdd(DiaryManager.Instacne.CraftResultItem.itemId);
                        ConsumeManager.TimeUp(consumeTime);
                        haveBonfireTime -= consumeTime;
                        Vars.UserData.uData.BonfireHour = haveBonfireTime / 60;
                        if (Vars.UserData.uData.BonfireHour <= 0)
                        {
                            Vars.UserData.uData.BonfireHour = 0;
                        }
                        ConsumeManager.SaveConsumableData();
                        CampManager.Instance.SetBonTime();
                        Vars.UserData.RemoveItemData(fireitem);
                        if (material1obj.id != zeroId)
                        {
                            Vars.UserData.RemoveItemData(condimentitem);
                        }
                        if (material2obj.id != zeroId)
                        {
                            Vars.UserData.RemoveItemData(materialitem);
                        }
                        DiaryManager.Instacne.OpenProduceReward();
                    }
                    else
                    {
                        CampManager.Instance.reconfirmPanelManager.gameObject.SetActive(true);
                        CampManager.Instance.reconfirmPanelManager.inventoryFullPopup.SetActive(true);
                        CampManager.Instance.reconfirmPanelManager.bonfireTimeRemainPopup.SetActive(false);
                    }
                }
            }
        }
        CampManager.Instance.camptutorial.TutorialCraftStartbuttonClick = true;
        CraftReset();

    }

    public void FillterByTool() //��ư�Լ�
    {
        currentButtonType = CraftButtonType.Tool;
        page = 1;
        Init();
    }
    public void FillterByBattle()//��ư�Լ�
    {
        currentButtonType = CraftButtonType.Battle;
        page = 1;
        Init();
    }
    public void FillterByHerb()//��ư�Լ�
    {
        currentButtonType = CraftButtonType.Herb;
        page = 1;
        Init();
    }

    public void CraftReset()
    {
        ismaterial0have = false;
        ismaterial1have = false;
        ismaterial2have = false;

        fire.sprite = null;
        fire.color = Color.clear;
        condiment.sprite = null;
        condiment.color = Color.clear;

        material.sprite = null;
        material.color = Color.clear;

        result = string.Empty;
        makingTime.text = string.Empty;
    }

}
