using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiaryRecipe : MonoBehaviour
{
    private List<RecipeObj> itemGoList = new List<RecipeObj>();
    private int selectedSlot = -1;
    private const int MaxitemCount = 48;
    private RecipeDataTable table;
    private AllItemDataTable allitemTable;
    private string Time = null;
    private string result = string.Empty;
    [Header("프리햅 셋팅")]
    public RecipeObj itemPrehab;
    public ScrollRect scrollRect;
    [Header("텍스트 셋팅")]
    public TextMeshProUGUI makingTime;
    public TextMeshProUGUI resultName;
    public TextMeshProUGUI cookeffect;
    public TextMeshProUGUI description;
    [Header("이미지 셋팅")]
    public Image recipecookImage;
    public Image material0;
    public Image material1;
    public Image material2;

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
            item.gameObject.SetActive(true);
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
        var fireid = string.Empty;
        var condimentid = string.Empty;
        var materialid = string.Empty;
        if (itemGoList[slot].Recipes != null)
        {
            if (itemGoList[slot].Recipes[0] != null)
            {
                fireid = $"ITEM_{(itemGoList[slot].Recipes[0])}";
                material0.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
            }
            if (itemGoList[slot].Recipes[1] != null)
            {
                condimentid = $"ITEM_{(itemGoList[slot].Recipes[1])}";
                material1.sprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
            }
            if (itemGoList[slot].Recipes[2] != null)
            {
                materialid = $"ITEM_{(itemGoList[slot].Recipes[2])}";
                material2.sprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
            }
            if (itemGoList[slot].Result != null)
            {
                result = itemGoList[slot].Result;
                var resultid = $"ITEM_{result}";

                recipecookImage.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
                Time = itemGoList[slot].Time;
                makingTime.text = $"제작 시간은 {Time}분 입니다. ";
                resultName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
                cookeffect.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;
                description.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;
            }

        }


    }
}
