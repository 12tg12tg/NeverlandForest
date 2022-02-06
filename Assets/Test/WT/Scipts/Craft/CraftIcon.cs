using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftIcon : MonoBehaviour
{
    private List<CraftObj> itemGoList = new List<CraftObj>();
    private int selectedSlot = -1;
    private const int MaxitemCount = 100;
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

    public CraftObj itemPrehab;
    public ScrollRect scrollRect;

    public Image fire;
    public Image condiment;
    public Image material;

    public TextMeshProUGUI makingTime;
    public string result = string.Empty;
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

        for (int i = 0; i < MaxitemCount; i++)
        {
            var item = Instantiate(itemPrehab, scrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.AddComponent<Button>();
            item.gameObject.SetActive(false);

            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnChangedSelection(item.Slot));
        }
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Craft);
        SetAllItems();
    }
    public void SetAllItems()
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        var itemList = Vars.UserData.HaveCraftIDList;

        for (int i = 0; i < itemList.Count; i++)
        {
            itemGoList[i].Init(table, itemList[i]);
            itemGoList[i].gameObject.SetActive(true);
        }
        if (itemList.Count > 0)
        {
            selectedSlot = 0;
            EventSystem.current.SetSelectedGameObject(itemGoList[selectedSlot].gameObject);
        }
    }
    public void OnChangedSelection(int slot)
    {
        // 누른 레시피의 조합을 보여주자.
        var fireid = $"ITEM_{(itemGoList[slot].Crafts[0])}";
        var condimentid = $"ITEM_{(itemGoList[slot].Crafts[1])}";
        var materialid = $"ITEM_{(itemGoList[slot].Crafts[2])}";

        fire.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
        condiment.sprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
        material.sprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
        result = itemGoList[slot].Result;

        materialobj0 = allitemTable.GetData<AllItemTableElem>(fireid);
        materialobj1 = allitemTable.GetData<AllItemTableElem>(condimentid);
        materialobj2 = allitemTable.GetData<AllItemTableElem>(materialid);

        Time = itemGoList[slot].Time;
        makingTime.text = $"제작 시간은 {Time}분 입니다. ";
        CampManager.Instance.producingText.text = "제작 하기";
    }
    public void MakeProducing()
    {
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var makeTime = float.Parse(Time);
        var list = Vars.UserData.HaveAllItemList; //인벤토리
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

            if (is0ok && is1ok && is2ok)
            {

                fireitem = new DataAllItem(list[material0Num]);
                fireitem.OwnCount = 1;
                if (materialobj1.id != "ITEM_0")
                {
                    condimentitem = new DataAllItem(list[material1Num]);
                    condimentitem.OwnCount = 1;
                }
                if (materialobj2.id != "ITEM_0")
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
                    Debug.Log("제작 완료");
                    ConsumeManager.TimeUp(makeTime);
                    Vars.UserData.uData.BonfireHour -= makeTime / 60;
                    CampManager.Instance.SetBonTime();
                    Vars.UserData.RemoveItemData(fireitem);
                    if (materialobj1.id != "ITEM_0")
                    {
                        Vars.UserData.RemoveItemData(condimentitem);
                    }
                    if (materialobj2.id != "ITEM_0")
                    {
                        Vars.UserData.RemoveItemData(materialitem);
                    }
                    DiaryManager.Instacne.OpenProduceReward();
                }
                else
                {
                    CampManager.Instance.reconfirmPanelManager.gameObject.SetActive(true);
                    CampManager.Instance.reconfirmPanelManager.OpenBagReconfirm();
                    Debug.Log("가방이 가득찼다");
                }
            }
            else if (!is1ok)
            {
                if (materialobj1.id == "ITEM_0")
                {
                    is1ok = true;
                }
                else
                {
                    CampManager.Instance.producingText.text = "재료가 부족합니다";
                }
            }
            else if (!is2ok)
            {
                if (materialobj2.id == "ITEM_0")
                {
                    is2ok = true;
                }
                else
                {
                    CampManager.Instance.producingText.text = "재료가 부족합니다";
                }
            }
            if (!is0ok)
            {
                CampManager.Instance.producingText.text = "재료가 부족합니다";
            }
        }
    }

    public void FillterByTool()
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        var itemList = Vars.UserData.HaveCraftIDList;

        for (int i = 0; i < itemList.Count; i++)
        {
            var type = DataTableManager.GetTable<CraftDataTable>().GetData<CraftTableElem>(itemList[i]).type;
            if (type == 0 || type == 1)
            {
                itemGoList[i].Init(table, itemList[i]);
                itemGoList[i].gameObject.SetActive(true);
            }
        }
    }
    public void FillterByBattle()
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        var itemList = Vars.UserData.HaveCraftIDList;

        for (int i = 0; i < itemList.Count; i++)
        {
            var type = DataTableManager.GetTable<CraftDataTable>().GetData<CraftTableElem>(itemList[i]).type;
            if (type == 4)
            {
                itemGoList[i].Init(table, itemList[i]);
                itemGoList[i].gameObject.SetActive(true);
            }
        }
    }
    public void FillterByHerb()
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        var itemList = Vars.UserData.HaveCraftIDList;

        for (int i = 0; i < itemList.Count; i++)
        {
            var type = DataTableManager.GetTable<CraftDataTable>().GetData<CraftTableElem>(itemList[i]).type;
            if (type == 5)
            {
                itemGoList[i].Init(table, itemList[i]);
                itemGoList[i].gameObject.SetActive(true);
            }
        }
    }
}
