using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 인벤토리 컨트롤러 제거, DataAllItem 속성들 다양해짐

public class RecipeIcon : MonoBehaviour
{
    private List<RecipeObj> itemGoList = new List<RecipeObj>();
    private int selectedSlot = -1;
    private const int MaxitemCount = 100;
    private RecipeDataTable table;
    private AllItemTableElem fireobj;
    private AllItemTableElem condimentobj;
    private AllItemTableElem materialobj;
    private string Time = null;
    private string result = string.Empty;
    private bool isfireok;
    private bool iscondimentok;
    private bool ismaterialok;
    private int fireNum;
    private int condimentNum;
    private int materialNum;
    private DataAllItem fireitem;
    private DataAllItem condimentitem;
    private DataAllItem materialitem;
    private DataAllItem resultitem;
    private AllItemDataTable allitemTable;

    public RecipeObj itemPrehab;
    public ScrollRect scrollRect;
    public TextMeshProUGUI makingTime;

    public Image fire;
    public Image condiment;
    public Image material;

    public bool Isfireok => isfireok;
    public bool Iscondimentok => iscondimentok;
    public bool Ismaterialok => ismaterialok;
  
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        table = DataTableManager.GetTable<RecipeDataTable>();
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
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Recipe);
        SetAllItems();
    }
    public void SetAllItems()
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }
        var itemList = Vars.UserData.HaveRecipeIDList;

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
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var fireid = $"ITEM_{(itemGoList[slot].Recipes[0])}";
        var condimentid = $"ITEM_{(itemGoList[slot].Recipes[1])}";
        var materialid = $"ITEM_{(itemGoList[slot].Recipes[2])}";

        fire.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
        condiment.sprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
        material.sprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
        result = itemGoList[slot].Result;

        fireobj = allitemTable.GetData<AllItemTableElem>(fireid);
        condimentobj = allitemTable.GetData<AllItemTableElem>(condimentid);
        materialobj = allitemTable.GetData<AllItemTableElem>(materialid);

        Time = itemGoList[slot].Time;
        makingTime.text = $"제작 시간은 {Time}분 입니다. ";
        CampManager.Instance.cookingText.text = "요리 하기";
    }
    public void MakeCooking()
    {
        var makeTime = int.Parse(Time);
        var list = Vars.UserData.HaveAllItemList;
        if (result != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemTableElem.id == fireobj.id)
                {
                    isfireok = true;
                    fireNum = i;
                }
                if (list[i].ItemTableElem.id == condimentobj.id)
                {
                    iscondimentok = true;
                    condimentNum = i;

                }
                if (list[i].ItemTableElem.id == materialobj.id)
                {
                    ismaterialok = true;
                    materialNum = i;
                }
            }
            if (!isfireok)
            {
                CampManager.Instance.cookingText.text = "재료가 부족합니다";
            }
            else if (!iscondimentok)
            {
                iscondimentok = true;
                var stringid = "ITEM_0";
                condimentitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringid));
            }
            else if (!ismaterialok)
            {
                ismaterialok = true;
                var stringid = "ITEM_0";
                materialitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringid));
            }

            if (isfireok && iscondimentok && ismaterialok)
            {

                fireitem = new DataAllItem(list[fireNum]);
                fireitem.OwnCount = 1;

                if (condimentitem ==null)
                {
                    condimentitem = new DataAllItem(list[condimentNum]);
                    condimentitem.OwnCount = 1;
                }
                if (materialitem == null)
                {
                    materialitem = new DataAllItem(list[materialNum]);
                    materialitem.OwnCount = 1;
                }

                var stringId = $"ITEM_{result}";
                resultitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringId));

                if (fireitem.itemId != "ITEM_0")
                {
                    Vars.UserData.RemoveItemData(fireitem);
                }
                if (condimentitem.itemId != "ITEM_0")
                {
                    Vars.UserData.RemoveItemData(condimentitem);
                }
                if (materialitem.itemId != "ITEM_0")
                {
                    Vars.UserData.RemoveItemData(materialitem);
                }

                ConsumeManager.TimeUp(makeTime);
                ConsumeManager.RecoveryHunger(20);

                isfireok = false;
                iscondimentok = false;
                ismaterialok = false;
                fire.sprite = null;
                condiment.sprite = null;
                material.sprite = null;
                result = string.Empty;
                makingTime.text = string.Empty;
                Debug.Log("요리 완료");
                DiaryManager.Instacne.OpenCookingReward();
            }
        }
    }
}
