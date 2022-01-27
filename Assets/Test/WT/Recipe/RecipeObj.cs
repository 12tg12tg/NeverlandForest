using UnityEngine;
using UnityEngine.UI;
public class RecipeObj : MonoBehaviour
{
    public int Slot { get; set; }
    public Image image;
    private string[] recipes;
    public string[] Recipes => recipes;
    private string[] time;
    public string[] Time => time;
    private string result;
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
