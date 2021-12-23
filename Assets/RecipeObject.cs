using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
public class RecipeObject : MonoBehaviour
{   
    public int Slot { get; set; }
    public TextMeshProUGUI RecipeText;
   
    private StringBuilder sb =new StringBuilder();
    public void Init(RecipeDataTable elem, string id)
    {
        var result = elem.GetData<RecipeTableElem>(id).result_ID;
        string[] recipe = elem.GetCombination(result);
        // fire+ condiment+ material ���ս��� ���� �ϳ����� ����ִ�.
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        sb.Clear();
        for (int i = 0; i < recipe.Length; i++)
        {
           sb.Append($"{recipe[i]}�� ������ :{allitem.GetData<AllItemTableElem>(recipe[i]).name} ");
        }
        Debug.Log(sb);
        RecipeText.text = sb.ToString();
    }
}
