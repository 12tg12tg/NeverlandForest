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
    WorldMapData_0 worldMapSaveData;
    WorldMapPlayerData_0 worldMapPlayerData;
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
            case SaveLoadSystem.SaveType.WorldMapData:
                SaveWorldMapData();
                break;
            case SaveLoadSystem.SaveType.WorldMapPlayerData:
                SaveWorldMapPlayer();
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
            case SaveLoadSystem.SaveType.WorldMapData:
                LoadWorldMapNode();
                break;
            case SaveLoadSystem.SaveType.WorldMapPlayerData:
                LoadWorldMapPlayer();
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
        //var list = new List<DungeonRoom>();
        //var rList = new List<DungeonRoom[]>();
        var pList = new List<PlayerDungeonUnitData>();
        var pList2 = new List<PlayerDungeonUnitData>();
        var iList = new List<Vector2>();
        var rList2 = new List<List<DungeonRoom>>();

        foreach (var data in Vars.UserData.AllDungeonData)
        {
            data.Value.dungeonRoomList = ArrayConvertList(data.Value.dungeonRoomArray);
        }

        foreach (var data in Vars.UserData.AllDungeonData)
        {
            //list.Add(data.Value.curDungeonData);
            //rList.Add(data.Value.dungeonRoomArray);
            pList.Add(data.Value.curPlayerGirlData);
            pList2.Add(data.Value.curPlayerBoyData);
            iList.Add(data.Key);
            rList2.Add(data.Value.dungeonRoomList);
        }
        //dungeonMapData.dungeonRoomArray = rList;
        dungeonMapData.curPlayerGirlData = pList;
        dungeonMapData.curPlayerBoyData = pList2;
        dungeonMapData.dungeonIndex = iList;
        dungeonMapData.dungeonRoomList = rList2;
        dungeonMapData.curDungeonIndex = Vars.UserData.curDungeonIndex;
        dungeonMapData.curDungeonRoomIndex = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData.roomIdx;
        SaveLoadSystem.Save(dungeonMapData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.DungeonMap);
    }

    //dungeonMapData.dungeonRoomList = ArrayConvertList(Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray);
    // = Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].curDungeonData;
    //dungeonMapData.curPlayerData = Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].curPlayerData;
    //dungeonMapData.dungeonRoomArray = Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray;
    //dungeonMapData.curDungeonIndex = Vars.UserData.curDungeonIndex;

    private List<DungeonRoom> ArrayConvertList(DungeonRoom[] array)
    {
        List<DungeonRoom> list = new List<DungeonRoom>();
        // Ω√¿€ ¿Œµ¶Ω∫
        int curIdx = 100;
        while(array[curIdx].nextRoomIdx != -1)
        {
            list.Add(array[curIdx]);
            curIdx = array[curIdx].nextRoomIdx;
        }
        list.Add(array[curIdx]);

        return list;
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
    private void SaveWorldMapData()
    {
        worldMapSaveData = new WorldMapData_0();
        worldMapSaveData.WorldMapNodeStruct = Vars.UserData.WorldMapNodeStruct;
        worldMapSaveData.WorldMapTree = Vars.UserData.WorldMapTree;
        SaveLoadSystem.Save(worldMapSaveData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMapData);
    }
    private void SaveWorldMapPlayer()
    {
        worldMapPlayerData = new WorldMapPlayerData_0();
        worldMapPlayerData.WorldMapPlayerData = Vars.UserData.WorldMapPlayerData;
        SaveLoadSystem.Save(worldMapPlayerData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMapPlayerData);
    }

    private void SaveConsumableData()
    {
        consumableSaveData = new ConsumableSaveData_0();
        consumableSaveData.curStamina = Vars.UserData.uData.CurStamina;
        consumableSaveData.curIngameHour = Vars.UserData.uData.CurIngameHour;
        consumableSaveData.curIngameMinute = Vars.UserData.uData.CurIngameMinute;
        consumableSaveData.curTimeState =ConsumeManager.CurTimeState;
        consumableSaveData.date = Vars.UserData.uData.Date;
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
            for (int i = 0; i < dungeonMapData.dungeonIndex.Count; i++)
            {
                //dungeonData.curDungeonData = dungeonMapData.curDungeonData[i];
                var dungeonData = new DungeonData();
                dungeonData.dungeonRoomList = dungeonMapData.dungeonRoomList[i];
                dungeonData.curPlayerGirlData = dungeonMapData.curPlayerGirlData[i];
                dungeonData.curPlayerBoyData = dungeonMapData.curPlayerBoyData[i];

                ListConvertArray(dungeonMapData.dungeonRoomList[i], dungeonData.dungeonRoomArray);

                if(!Vars.UserData.AllDungeonData.ContainsKey(dungeonMapData.dungeonIndex[i]))
                {
                    Vars.UserData.AllDungeonData.Add(dungeonMapData.dungeonIndex[i], dungeonData);
                }
            }
            Vars.UserData.curDungeonIndex = dungeonMapData.curDungeonIndex;
            Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData = 
                Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray[dungeonMapData.curDungeonRoomIndex];

            //Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].roomList = dungeonMapData.dungeonRoomList;
        }
    }

    private void ListConvertArray(List<DungeonRoom> list, DungeonRoom[] array)
    {
        foreach(var data in list)
        {
            array[data.roomIdx] = data;
        }
    }

    private void LoadWorldMapNode()
    {
        worldMapSaveData = (WorldMapData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMapData);
        if (worldMapSaveData != null)
        {
            Vars.UserData.WorldMapNodeStruct = worldMapSaveData.WorldMapNodeStruct;
            Vars.UserData.WorldMapTree = worldMapSaveData.WorldMapTree;
        }
    }
    private void LoadWorldMapPlayer()
    {
        worldMapPlayerData = (WorldMapPlayerData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMapPlayerData);
        if (worldMapPlayerData != null)
        {
            Vars.UserData.WorldMapPlayerData = worldMapPlayerData.WorldMapPlayerData;
        }
    }

    private void LoadConsumableData()
    {
        consumableSaveData = (ConsumableSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.ConsumableData);
        if (consumableSaveData != null)
        {
            Vars.UserData.uData.CurStamina = consumableSaveData.curStamina;
            Vars.UserData.uData.CurIngameHour = consumableSaveData.curIngameHour;
            Vars.UserData.uData.CurIngameMinute = consumableSaveData.curIngameMinute;
            Vars.UserData.uData.Date = consumableSaveData.date;
            ConsumeManager.CurTimeState = consumableSaveData.curTimeState;
        }
    }
}