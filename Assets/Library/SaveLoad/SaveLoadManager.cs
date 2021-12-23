using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    PlayerSaveData_1 playerDate;
    OptionSaveData_0 optionDate;
    RecipeSaveData_0 recipeData;
    TimeSaveData_0 timeData;

    private void Start()
    {
        //SaveLoadSystem.Init();
    }

    public void Save(SaveLoadSystem.SaveType saveType)
    {
        switch (saveType)
        {
            case SaveLoadSystem.SaveType.Player:
                SavePlayer();
                break;
            case SaveLoadSystem.SaveType.Option:
                SaveOption();
                break;
            case SaveLoadSystem.SaveType.Recipe:
                SaveRecipe();
                break;
            case SaveLoadSystem.SaveType.Time:
                SaveTime();
                break;
        }
    }

    public void Load(SaveLoadSystem.SaveType saveType)
    {
        switch (saveType)
        {
            case SaveLoadSystem.SaveType.Player:
                LoadPlayer();
                break;
            case SaveLoadSystem.SaveType.Option:
                LoadOption();
                break;
            case SaveLoadSystem.SaveType.Recipe:
                LoadRecipe();
                break;
            case SaveLoadSystem.SaveType.Time:
                LoadTime();
                break;
        }
    }

    private void SavePlayer()
    {

    }
    private void SaveOption()
    {

    }
    private void SaveRecipe()
    {
        recipeData = new RecipeSaveData_0();
        recipeData.haveRecipe = Vars.UserData.HaveRecipeIDList;
        SaveLoadSystem.Save(recipeData, SaveLoadSystem.Modes.Text,SaveLoadSystem.SaveType.Recipe);
    }
    private void SaveTime()
    {
        timeData = new TimeSaveData_0();
        timeData.makeTime = Vars.UserData.MakeList;
        SaveLoadSystem.Save(timeData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Time);
    }
   
    private void LoadPlayer()
    {

    }
    private void LoadOption()
    {

    }
    private void LoadRecipe()
    {
        recipeData =(RecipeSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Recipe);

        if (recipeData !=null)
        {
            Vars.UserData.HaveRecipeIDList = recipeData.haveRecipe;

        }
    }
    private void LoadTime()
    {
        timeData = (TimeSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Time);
        if (timeData !=null)
        {
            Vars.UserData.MakeList = timeData.makeTime;
        }
    }
}
