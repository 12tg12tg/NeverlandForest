using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class RecipeInfo : MonoBehaviour
{
    public RecipeObject itemprehab;
    public ScrollRect scrollRect;
    private List<RecipeObject> itemGoList = new List<RecipeObject>();
    private int selectedSlot = -1;
    public const int MaxItemCount = 100;
    private RecipeDataTable table;

    public void Awake()
    {

        table = DataTableManager.GetTable<RecipeDataTable>();
        for (int i = 0; i < MaxItemCount; i++)
        {
            var item = Instantiate(itemprehab, scrollRect.content);
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
        // 누른 레시피의 조합을 보여주자.
    }

}
