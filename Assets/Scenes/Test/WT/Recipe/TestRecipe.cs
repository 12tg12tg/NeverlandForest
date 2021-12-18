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
            Debug.Log($"{result} �������� ���Խ��ϴ�"); // ������ ������ �־ ������� ������� ����
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
                Debug.Log($"{i}���� ���� {array[i]}������"); //  ���� �־���, ��, ���̷� , ��ῡ ���� ���� ����
                sb.Append($"{i}���� ���� {array[i]}�� ������");
            }
            sb.Append($"��� : {result}�� ������");
            Debug.Log(sb);
        }
    }
}
