using UnityEngine;

public class Combination : MonoBehaviour
{
    public UICookInventoryList inventory;
    private string fire;
    private string condiment;
    private string material;
    private RecipeDataTable recipeTableForCheck;
    public RecipeObject recipe;
    public RecipeInfo info;

    public void Start()
    {
        recipeTableForCheck = DataTableManager.GetTable<RecipeDataTable>();

        Debug.Log(Vars.UserData.HaveRecipeIDList.Count);
    }
    void OnGUI()
    {
        if (GUILayout.Button("Start Cooking"))
        {
            fire = inventory.FireObject.DataItem.ItemTableElem.id;
            condiment = inventory.CondimentObject.DataItem.ItemTableElem.id;
            material = inventory.MaterialObject.DataItem.ItemTableElem.id;
            var recipeTable = DataTableManager.GetTable<RecipeDataTable>();
            var result = "";
            recipeTable.ISCombine(condiment, material, out result, fire);

            var allitem = DataTableManager.GetTable<AllItemDataTable>();
            if (recipeTable.ISCombine(condiment, material, out result, fire))
            {
                var item = allitem.GetData<AllItemTableElem>(result);
                Debug.Log($"{result}번째 아이템 {item.name} 가 나왔습니다");
                inventory.Result.sprite = item.IconSprite;
                inventory.ResultObject = inventory.itemGoList[int.Parse(result)];
                var RecipeId = recipeTable.GetRecipeId(result);
                var userData = Vars.UserData.HaveRecipeIDList;
                if (!userData.Contains(RecipeId))
                {
                    userData.Add(RecipeId);
                    info.Init();
                    SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);
                }
                else
                {
                    Debug.Log("이미 해당 레시피가 존재함");
                }
            }
            else
            {
                Debug.Log("해당 조합법에 맞는 아이템이 없습니다");
            }
        }
    }
}
