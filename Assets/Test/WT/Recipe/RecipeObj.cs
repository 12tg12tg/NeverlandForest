using UnityEngine;
using UnityEngine.UI;
public class RecipeObj : MonoBehaviour
{
    private string[] recipes;
    private string time;
    private string result;

    public int Slot { get; set; }
    public Image image;
    public string[] Recipes => recipes;
    public string Time => time;
    public string Result => result;
    public void Init(RecipeDataTable elem, string id)
    {   
        result = elem.GetData<RecipeTableElem>(id).result_ID;
        recipes = elem.GetCombination(result);
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        var stringid = $"ITEM_{result}";
        image.sprite = allitem.GetData<AllItemTableElem>(stringid).IconSprite;
        time = elem.IsMakingTime(result);
    }

}
