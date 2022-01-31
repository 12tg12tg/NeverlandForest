using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftIcon : MonoBehaviour
{
    public CraftObj itemPrehab;
    public ScrollRect scrollRect;

    private List<CraftObj> itemGoList = new List<CraftObj>();
    private int selectedSlot = -1;
    public const int MaxitemCount = 100;
    private CraftDataTable table;

    public Image fire;
    public Image condiment;
    public Image material;

    public TextMeshProUGUI makingTime;
    string Time = null;
    string result = string.Empty;
    private AllItemTableElem materialobj0;
    private AllItemTableElem materialobj1;
    private AllItemTableElem materialobj2;

    private bool is0ok;
    public bool Is0ok => is0ok;
    private bool is1ok;
    public bool Is1ok => is1ok;
    private bool is2ok;
    public bool Is2ok => is2ok;

    private int material0Num;
    private int material1Num;
    private int material2Num;

    public void Awake()
    {
        table = DataTableManager.GetTable<CraftDataTable>();
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
        Init();
    }
    public void Init()
    {
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
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        // 누른 레시피의 조합을 보여주자.
        var fireid = $"ITEM_{(itemGoList[slot].Crafts[0])}";
        var condimentid = $"ITEM_{(itemGoList[slot].Crafts[1])}";
        var materialid = $"ITEM_{(itemGoList[slot].Crafts[2])}";
        fire.sprite = allitem.GetData<AllItemTableElem>(fireid).IconSprite;
        condiment.sprite = allitem.GetData<AllItemTableElem>(condimentid).IconSprite;
        material.sprite = allitem.GetData<AllItemTableElem>(materialid).IconSprite;
        result = itemGoList[slot].Result;

        materialobj0 = allitem.GetData<AllItemTableElem>(fireid);
        materialobj1 = allitem.GetData<AllItemTableElem>(condimentid);
        materialobj2 = allitem.GetData<AllItemTableElem>(materialid);

        Time = itemGoList[slot].Time;
        makingTime.text = $"제작 시간은 {Time}분 입니다. ";
    }
    public void MakeProducing()
    {
        var allitem= DataTableManager.GetTable<AllItemDataTable>();
        var makeTime = int.Parse(Time);
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

            if (is0ok && is1ok && is2ok )
            {
                ConsumeManager.TimeUp(makeTime);

                var fireitem = new DataAllItem(list[material0Num]);
                fireitem.OwnCount = 1;
                Vars.UserData.RemoveItemData(fireitem);
                if (materialobj1.id != "ITEM_0")
                {
                    var condimentitem = new DataAllItem(list[material1Num]);
                    condimentitem.OwnCount = 1;
                    Vars.UserData.RemoveItemData(condimentitem);
                }
                if (materialobj2.id != "ITEM_0")
                {
                    var materialitem = new DataAllItem(list[material2Num]);
                    materialitem.OwnCount = 1;
                    Vars.UserData.RemoveItemData(materialitem);
                }
                DiaryManager.Instacne.produceInventory.ItemButtonInit();
                var ProducedItemId = DataTableManager.GetTable<CraftDataTable>().GetCraftId(result);
                var table = DataTableManager.GetTable<AllItemDataTable>();
                DiaryManager.Instacne.CraftResultItem = new DataAllItem(table.GetData<AllItemTableElem>(ProducedItemId));
                DiaryManager.Instacne.CraftResultItem.OwnCount = 1;

                DiaryManager.Instacne.craftResultItemImage.sprite = DiaryManager.Instacne.CraftResultItem.ItemTableElem.IconSprite;

                is0ok = false;
                is1ok = false;
                is2ok = false;
                fire.sprite = null;
                condiment.sprite = null;
                material.sprite = null;
                result = string.Empty;
                makingTime.text = string.Empty;
                Debug.Log("제작 완료");


                DiaryManager.Instacne.OpenProduceReward();
            }
            if (!is1ok || !is2ok)
            {
                if (materialobj1.id== "ITEM_0")
                {
                    is1ok = true;

                }
                if (materialobj2.id == "ITEM_0")
                {
                    is2ok = true;
                }
            }
            else
            {
                CampManager.Instance.producingText.text = "재료가 부족합니다";
            }
        }

      

    }
    
}
