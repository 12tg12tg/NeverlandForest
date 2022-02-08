using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
public class RecipeObject : MonoBehaviour
{
    private string[] recipes;
    private string time;
    private string result;
    public Image image;
    public string[] Recipes => recipes;
    public string Time => time;
    public string Result => result;
    private DiaryRecipe diaryRecipe;
    [SerializeField] private Button button;
    public void Init(RecipeDataTable elem, string id,DiaryRecipe diaryrecipe)
    {
        this.diaryRecipe = diaryrecipe;
        result = elem.GetData<RecipeTableElem>(id).result_ID;
        recipes = elem.GetCombination(result);
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        var stringid = $"ITEM_{result}";
        image.sprite = allitem.GetData<AllItemTableElem>(stringid).IconSprite;
        time = elem.IsMakingTime(result);
        button.interactable = true;
    }
    public void ButtonOnClick()
    {
        diaryRecipe.currentRecipe = this;
        diaryRecipe.OnChangedSelection();
    }
    public void Clear()
    {
        button.interactable = false;
    }
}
