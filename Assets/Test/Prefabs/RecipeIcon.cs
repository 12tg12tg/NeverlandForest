using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
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
        // ���� �������� ������ ��������.
        fire.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Recipes[0]).IconSprite;
        condiment.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Recipes[1]).IconSprite;
        material.sprite = allitem.GetData<AllItemTableElem>(itemGoList[slot].Recipes[2]).IconSprite;

        var Time = itemGoList[slot].Time;
        makingTime.text = $"���� �ð��� {Time[0]}:{Time[1]}:{Time[2]} �Դϴ�. ";
    }

}
