using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class DiaryRecipeIcon : MonoBehaviour
{
    public RecipeObj itemPrehab;
    public ScrollRect diaryscrollRect;

    private List<RecipeObj> itemGoList = new List<RecipeObj>();
    private int selectedSlot = -1;
    public const int MaxitemCount = 100;
    private RecipeDataTable table;

    public Image diary_fire;
    public Image diary_condiment;
    public Image diary_material;
    public Image diary_result;

    public void Awake()
    {
        table = DataTableManager.GetTable<RecipeDataTable>();
        for (int i = 0; i < MaxitemCount; i++)
        {
            var item = Instantiate(itemPrehab, diaryscrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.AddComponent<Button>();
            item.gameObject.SetActive(false);

            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnChangedDiarySelection(item.Slot));
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
    public void OnChangedDiarySelection(int slot)
    {
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        // 누른 레시피의 조합을 보여주자.
        diary_fire.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Recipes[0]).IconSprite;
        diary_condiment.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Recipes[1]).IconSprite;
        diary_material.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Recipes[2]).IconSprite;
        diary_result.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Result).IconSprite;
    }
}
