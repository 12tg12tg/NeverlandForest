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
    private RecipeIcon recipeIcon;
    [SerializeField] private Button button;

    public void Init(RecipeDataTable elem, string id, RecipeIcon recipeIcon)
    {
        this.recipeIcon = recipeIcon;
        result = elem.GetData<RecipeTableElem>(id).result_ID;
        recipes = elem.GetCombination(result);
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        var stringid = $"ITEM_{result}";
        image.sprite = allitem.GetData<AllItemTableElem>(stringid).IconSprite;
        image.color = Color.white;
        time = elem.IsMakingTime(result);
        button.interactable = true;
    }
    public void ButtonOnClick()
    {
        recipeIcon.currentRecipe = this;
        recipeIcon.OnChangedSelection();
    }
    public void Clear()
    {
        image.sprite = null;
        image.color = Color.clear;
        button.interactable = false;
    }
}
