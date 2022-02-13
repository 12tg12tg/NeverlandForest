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
    private AllItemTableElem materialobj0;
    private AllItemTableElem materialobj1;
    private AllItemTableElem materialobj2;
    private bool is0ok;
    private bool is1ok;
    private bool is2ok;
    private int material0Num;
    private int material1Num;
    private int material2Num;
    private DataAllItem fireitem;
    private DataAllItem condimentitem;
    private DataAllItem materialitem;
    private string Time = null;
    private AllItemDataTable allitemTable;

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

    public bool Is0ok
    {
        get => is0ok;
        set
        { is0ok = value; }
    }
    public bool Is1ok
    {
        get => is1ok;
        set { is1ok = value; }
    }
    public bool Is2ok
    {
        get => is2ok;
        set { is2ok = value; }
    }
    public void Start()
    {
        Init();
    }
    public void Init()
    {
        table = DataTableManager.GetTable<CraftDataTable>();
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var itemList = Vars.UserData.HaveCraftIDList;
        int type;
        var fillterList = itemList.Where(n =>
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
    public void PreviewPageOpen()
    {
        if (page > 1)
        {
            page--;
            SetPageButton();
        }
    }
    public void NextPageOpen()
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
        // ���� �������� ������ ��������.
        var fireid = $"ITEM_{(currentCraft.Crafts[0])}";
        var condimentid = $"ITEM_{(currentCraft.Crafts[1])}";
        var materialid = $"ITEM_{(currentCraft.Crafts[2])}";

        fire.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
        condiment.sprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
        material.sprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
        result = currentCraft.Result;
        var resultid = $"ITEM_{result}";
        materialobj0 = allitemTable.GetData<AllItemTableElem>(fireid);
        materialobj1 = allitemTable.GetData<AllItemTableElem>(condimentid);
        materialobj2 = allitemTable.GetData<AllItemTableElem>(materialid);

        resultItemIcon.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        resultItemIcon.color = Color.white;
        resultItemName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
        resultItemDesc.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;

        rewardresultItemIcon.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        rewardresultItemIcon.color = Color.white;
        rewardresultItemName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
        rewardresultItemDesc.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;


        Time = currentCraft.Time;
        makingTime.text = $"���� �ð��� {Time}�� �Դϴ�. ";
        CampManager.Instance.producingText.text = "���� �ϱ�";
    }
    public void MakeProducing()
    {
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var makeTime = float.Parse(Time);
        var list = Vars.UserData.HaveAllItemList; //�κ��丮
        if (result != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemTableElem.id == materialobj0.id)
                {
                    is0ok = true;
                    material0Num = i;
                }
                if (list[i].ItemTableElem.id == materialobj1.id)
                {
                    is1ok = true;
                    material1Num = i;

                }
                if (list[i].ItemTableElem.id == materialobj2.id)
                {
                    is2ok = true;
                    material2Num = i;
                }
            }

            if (!is0ok)
            {
                CampManager.Instance.producingText.text = "��ᰡ �����մϴ�";
            }
            var zeroId = "ITEM_0";


            if (materialobj1.id == zeroId)
            {
                is1ok = true;
            }
            if (materialobj2.id == zeroId)
            {
                is2ok = true;
            }

            if (is0ok && is1ok && is2ok)
            {
                fireitem = new DataAllItem(list[material0Num]);
                fireitem.OwnCount = 1;
                if (materialobj1.id != zeroId)
                {
                    condimentitem = new DataAllItem(list[material1Num]);
                    condimentitem.OwnCount = 1;
                }
                if (materialobj2.id != zeroId)
                {
                    materialitem = new DataAllItem(list[material2Num]);
                    materialitem.OwnCount = 1;
                }

                DiaryManager.Instacne.produceInventory.ItemButtonInit();
                var stringId = $"ITEM_{result}";
                DiaryManager.Instacne.CraftResultItem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringId));
                DiaryManager.Instacne.CraftResultItem.OwnCount = 1;
                DiaryManager.Instacne.craftResultItemImage.sprite = DiaryManager.Instacne.CraftResultItem.ItemTableElem.IconSprite;

                if (Vars.UserData.AddItemData(DiaryManager.Instacne.CraftResultItem) != false)
                {
                    Vars.UserData.ExperienceListAdd(DiaryManager.Instacne.CraftResultItem.itemId);
                    ConsumeManager.TimeUp(makeTime);
                    Vars.UserData.uData.BonfireHour -= makeTime / 60;
                    ConsumeManager.SaveConsumableData();
                    CampManager.Instance.SetBonTime();
                    Vars.UserData.RemoveItemData(fireitem);
                    if (materialobj1.id != zeroId)
                    {
                        Vars.UserData.RemoveItemData(condimentitem);
                    }
                    if (materialobj2.id != zeroId)
                    {
                        Vars.UserData.RemoveItemData(materialitem);
                    }
                    DiaryManager.Instacne.OpenProduceReward();
                }
                else
                {
                    CampManager.Instance.reconfirmPanelManager.gameObject.SetActive(true);
                    CampManager.Instance.reconfirmPanelManager.inventoryFullPopup.SetActive(true);
                }
            }
        }
    }

    public void FillterByTool()
    {
        currentButtonType = CraftButtonType.Tool;
        page = 1;
        Init();
    }
    public void FillterByBattle()
    {
        currentButtonType = CraftButtonType.Battle;
        page = 1;
        Init();
    }
    public void FillterByHerb()
    {
        currentButtonType = CraftButtonType.Herb;
        page = 1;
        Init();
    }
}
