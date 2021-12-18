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

        //user.RecipeList.Add(result)
        if (recipeTable.ISCombine("0", "2", "10", out result))
        {
            Debug.Log($"{result} 아이템이 나왔습니다"); // 각각의 재료들을 넣어서 만들어진 결과물이 나옴
        }
        else
        {
            Debug.Log("fail to combine");
        }
        if (result != null)
        {
            var array = recipeTable.GetCombination(result);
            for (int i = 0; i < array.Length; i++)
            {
                Debug.Log($"{i}번쨰 재료는 {array[i]}아이템"); //  재료로 넣었던, 불, 조미료 , 재료에 대한 값이 리턴
                sb.Append($"{i}번쨰 재료는 {array[i]}번 아이템");
            }
            sb.Append($"결과 : {result}번 아이템");
            Debug.Log(sb);
        }
    }
}
