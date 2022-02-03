using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTestMaker : MonoBehaviour
{
    private string result = string.Empty;
    private List<string> recipeResultList = new List<string>();
    private List<string> craftResultList = new List<string>();
    private RecipeDataTable recipeTable;
    private CraftDataTable craftTable;
    void Start()
    {
        recipeTable = DataTableManager.GetTable<RecipeDataTable>();
        craftTable = DataTableManager.GetTable<CraftDataTable>();
       /* SetRecipeResultList();
        SetCraftResultList();
        GetRecipeList();
        GetCraftList();*/
    }

  
    public void SetRecipeResultList()
    {
        // 23~35
        for (int i = 23; i < 35; i++)
        {
            recipeResultList.Add(i.ToString());
        }
    }
    public void SetCraftResultList()
    {
        //2,12,13,14,16,17,19,20,21,22,23
        craftResultList.Add(2.ToString());
        for (int i = 12; i < 14; i++)
        {
            craftResultList.Add(i.ToString());
        }
        for (int i = 16; i < 17; i++)
        {
            craftResultList.Add(i.ToString());
        }
        for (int i = 19; i < 23; i++)
        {
            craftResultList.Add(i.ToString());
        }
    }
    public void GetRecipeList()
    {
        var userRecipeData = Vars.UserData.HaveRecipeIDList;
        for (int i = 0; i < recipeResultList.Count; i++)
        {
            var recipeid = recipeTable.GetRecipeId(recipeResultList[i]);
            if (!userRecipeData.Contains(recipeid))
            {
                userRecipeData.Add(recipeid);
                SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);
            }
        }


/*
        랜덤한 숫자를 뽑는데 레시피정보를 전부 들고있는 List에서 
        뽑아서 보상으로 주고 해당 레시피가 기존이 있던게 아니라면 더해주고 
        List에서 뺀다. 기존에 있던거면 꽝

        var userRecipeData = Vars.UserData.HaveRecipeIDList;
        var rand = Random.Range(0, userRecipeData.Count-1);
        var recipeid = recipeTable.GetRecipeId(recipeResultList[rand]);
        if (!userRecipeData.Contains(recipeid))
        {
            userRecipeData.Add(recipeid);
            recipeResultList.RemoveAt(rand);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);
        }
        else
        {
            Debug.log("꽝");
        }
*/




    }
    public void GetCraftList()
    {
        var userCraftData = Vars.UserData.HaveCraftIDList;
        for (int i = 0; i < craftResultList.Count; i++)
        {
            var craftid = craftTable.GetCraftId(craftResultList[i]);
            if (!userCraftData.Contains(craftid))
            {
                userCraftData.Add(craftid);
                SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
            }
        }

    }
}
