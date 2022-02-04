using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class TestRecipe : MonoBehaviour
{
    public void Start()
    {
        Test();
    }
    public void Test()
    {
        var recipeTable = DataTableManager.GetTable<RecipeDataTable>();
        string result;
        var user = new UserData();
        var sb = new StringBuilder();
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        if (recipeTable.IsCombine("3","10", out result))
        {
            if (allitem.data.ContainsKey(result))
            {
                var item = allitem.GetData<AllItemTableElem>(result);
                Debug.Log($"{result}번째 아이템 {item.name} 가 나왔습니다");
            }
        }
        else
        {
            Debug.Log("해당조합은 레시피에 없습니다.");
        }

        if (result != null)
        {
            var array = recipeTable.GetCombination(result);
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append($"{i}번쨰 재료는 : " +
                    $"{array[i]}번 아이템 :{allitem.GetData<AllItemTableElem>(array[i]).name} ");
                sb.Append("\n");
            }
            sb.Append($"결과는:{result}번째 아이템 : {allitem.GetData<AllItemTableElem>(result).name} ");
            Debug.Log(sb);
        }
    }
}
