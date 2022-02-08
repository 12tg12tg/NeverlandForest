using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class RecipeInfo : MonoBehaviour
{
    private List<RecipeObject> itemGoList = new List<RecipeObject>();
    private RecipeDataTable table;
    private int selectedSlot = -1;
    private const int MaxItemCount = 100;

    public RecipeObject itemprehab;
    public ScrollRect scrollRect;

    public void Awake()
    {

        table = DataTableManager.GetTable<RecipeDataTable>();
        for (int i = 0; i < MaxItemCount; i++)
        {
            var item = Instantiate(itemprehab, scrollRect.content);
            itemGoList.Add(item);
            item.gameObject.AddComponent<Button>();
            item.gameObject.SetActive(false);

            var button = item.GetComponent<Button>();
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
