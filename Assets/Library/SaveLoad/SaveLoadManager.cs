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
    DungeonMapSaveData_0 dungeonMapData;
    WorldMapSaveData_0 worldMapSaveData;
    ConsumableSaveData_0 consumableSaveData;
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
            case SaveLoadSystem.SaveType.DungeonMap:
                SaveDungeonMap();
                break;
            case SaveLoadSystem.SaveType.WorldMap:
                SaveWorldMap();
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
            case SaveLoadSystem.SaveType.DungeonMap:
                LoadDungeonMap();
                break;
            case SaveLoadSystem.SaveType.WorldMap:
                LoadWorldMap();
                break;
        }
    }
    private void SavePlayer()
    {
    }
    private void SaveOption()
    {
    }
    private void SaveDungeonMap()
    {
        dungeonMapData = new DungeonMapSaveData_0();
        dungeonMapData.dungeonMap = Vars.UserData.dungeonMapData;
        SaveLoadSystem.Save(dungeonMapData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.DungeonMap);
    }
    private void SaveRecipe()
    {
        recipeData = new RecipeSaveData_0();
        recipeData.haveRecipe = Vars.UserData.HaveRecipeIDList;
        SaveLoadSystem.Save(recipeData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Recipe);
    }
    private void SaveTime()
    {
        //timeData = new TimeSaveData_0();
        //timeData.makeTime = Vars.UserData.MakeList;
        //SaveLoadSystem.Save(timeData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Time);
    }
    private void SaveWorldMap()
    {
        worldMapSaveData = new WorldMapSaveData_0();
        worldMapSaveData.MapNodeStruct = Vars.UserData.WorldMapNodeStruct;
        SaveLoadSystem.Save(worldMapSaveData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMap);
    }

    private void SaveConsumableData()
    {
        consumableSaveData = new ConsumableSaveData_0();
        consumableSaveData.curStamina = ConsumeManager.CurStamina;
        consumableSaveData.curStaminaState = ConsumeManager.CurStaminaState;
        consumableSaveData.curIngameHour = ConsumeManager.CurIngameHour;
        consumableSaveData.curIngameMinute = ConsumeManager.CurIngameMinute;
        consumableSaveData.curTimeState = ConsumeManager.CurTimeState;
        consumableSaveData.date = ConsumeManager.Date;
    }

    private void LoadPlayer()
    {
    }
    private void LoadOption()
    {
    }
    private void LoadRecipe()
    {
        recipeData = (RecipeSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Recipe);
        if (recipeData != null)
        {
            Vars.UserData.HaveRecipeIDList = recipeData.haveRecipe;
        }
    }
    private void LoadTime()
    {
        //timeData = (TimeSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Time);
        //if (timeData != null)
        //{
        //    Vars.UserData.MakeList = timeData.makeTime;
        //}
    }
    private void LoadDungeonMap()
    {
        dungeonMapData = (DungeonMapSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.DungeonMap);
        if (dungeonMapData != null)
        {
            Vars.UserData.dungeonMapData = dungeonMapData.dungeonMap;
        }
    }
    private void LoadWorldMap()
    {
        worldMapSaveData = (WorldMapSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMap);
        if (worldMapSaveData != null)
        {
            Vars.UserData.WorldMapNodeStruct = worldMapSaveData.MapNodeStruct;
        }
    }

    private void LoadConsumableData()
    {
        consumableSaveData = (ConsumableSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.ConsumableData);
        if (consumableSaveData != null)
        {
            ConsumeManager.CurStamina = consumableSaveData.curStamina;
            ConsumeManager.CurStaminaState = consumableSaveData.curStaminaState;
            ConsumeManager.CurIngameHour = consumableSaveData.curIngameHour;
            ConsumeManager.CurIngameMinute = consumableSaveData.curIngameMinute;
            ConsumeManager.CurTimeState = consumableSaveData.curTimeState;
            ConsumeManager.Date = consumableSaveData.date;
        }
    }
}