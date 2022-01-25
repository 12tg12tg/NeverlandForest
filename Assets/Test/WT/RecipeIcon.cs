using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 인벤토리 컨트롤러 제거, DataAllItem 속성들 다양해짐

public class RecipeIcon : MonoBehaviour
{
    public RecipeObj itemPrehab;
    public ScrollRect scrollRect;

    private List<RecipeObj> itemGoList = new List<RecipeObj>();
    private int selectedSlot = -1;
    public const int MaxitemCount = 100;
    private RecipeDataTable table;

    public Image fire;
    public Image condiment;
    public Image material;

    public TextMeshProUGUI makingTime;
    string[] Time = null;
    string result = string.Empty;
    private AllItemTableElem fireobj;
    private AllItemTableElem condimentobj;
    private AllItemTableElem materialobj;

    private bool isfireok;
    private bool iscondimentok;
    private bool ismaterialok;
    private int fireNum ;
    private int condimentNum ;
    private int materialNum ;

    public void Awake()
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
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        // 누른 레시피의 조합을 보여주자.
        var fireid = $"ITEM_{(itemGoList[slot].Recipes[0])}";
        var condimentid = $"ITEM_{(itemGoList[slot].Recipes[1])}";
        var materialid = $"ITEM_{(itemGoList[slot].Recipes[2])}";
        fire.sprite = allitem.GetData<AllItemTableElem>(fireid).IconSprite;
        condiment.sprite = allitem.GetData<AllItemTableElem>(condimentid).IconSprite;
        material.sprite = allitem.GetData<AllItemTableElem>(materialid).IconSprite;
        result = itemGoList[slot].Result;

        fireobj = allitem.GetData<AllItemTableElem>(fireid);
        condimentobj = allitem.GetData<AllItemTableElem>(condimentid);
        materialobj = allitem.GetData<AllItemTableElem>(materialid);

        Time = itemGoList[slot].Time;
        makingTime.text = $"제작 시간은 {Time[0]}:{Time[1]}:{Time[2]} 입니다. ";
    }
    public void MakeCooking()
    {
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        var makeTime = int.Parse(Time[0]) * 60 + int.Parse(Time[1]);
        var list = Vars.UserData.HaveAllItemList;
        if (result!=null)
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

            if (isfireok && iscondimentok && ismaterialok)
            {
                ConsumeManager.TimeUp(makeTime);
                ConsumeManager.RecoveryHunger(20);
                
                var fireitem = new DataAllItem(list[fireNum]);
                fireitem.OwnCount = 1;

                var condimentitem = new DataAllItem(list[condimentNum]);
                condimentitem.OwnCount = 1;

                var materialitem = new DataAllItem(list[materialNum]);
                materialitem.OwnCount = 1;

                Vars.UserData.RemoveItemData(fireitem);
                Vars.UserData.RemoveItemData(condimentitem);
                Vars.UserData.RemoveItemData(materialitem);

                isfireok = false;
                iscondimentok = false;
                ismaterialok = false;
                fire.sprite = null;
                condiment.sprite = null;
                material.sprite = null;
                result = string.Empty;
                makingTime.text = string.Empty;
                Debug.Log("요리 완료");
            }
            else
            {
                Debug.Log("재료가 부족합니다");
            }
        }
    }
}
