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
        if (recipeTable.ISCombine("3","10", out result))
        {
            if (allitem.data.ContainsKey(result))
            {
                var item = allitem.GetData<AllItemTableElem>(result);
                Debug.Log($"{result}��° ������ {item.name} �� ���Խ��ϴ�");
            }
        }
        else
        {
            Debug.Log("�ش������� �����ǿ� �����ϴ�.");
        }

        if (result != null)
        {
            var array = recipeTable.GetCombination(result);
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append($"{i}���� ���� : " +
                    $"{array[i]}�� ������ :{allitem.GetData<AllItemTableElem>(array[i]).name} ");
                sb.Append("\n");
            }
            sb.Append($"�����:{result}��° ������ : {allitem.GetData<AllItemTableElem>(result).name} ");
            Debug.Log(sb);
        }
    }
}
