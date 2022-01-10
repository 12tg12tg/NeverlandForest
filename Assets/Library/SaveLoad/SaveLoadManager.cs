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
    WorldMapNodeData_0 worldMapSaveNodeData;
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
            case SaveLoadSystem.SaveType.WorldMapNode:
                SaveWorldMapNode();
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
            case SaveLoadSystem.SaveType.WorldMapNode:
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
        var pList = new List<PlayerDungeonUnitData>();
        var iList = new List<Vector2>();
        //var rList = new List<DungeonRoom[]>();
        var rList2 = new List<List<DungeonRoom>>();

        foreach (var data in Vars.UserData.CurAllDungeonData)
        {
            data.Value.dungeonRoomList = ArrayConvertList(data.Value.dungeonRoomArray);
        }

        foreach (var data in Vars.UserData.CurAllDungeonData)
        {
            //list.Add(data.Value.curDungeonData);
            pList.Add(data.Value.curPlayerData);
            iList.Add(data.Key);
            //rList.Add(data.Value.dungeonRoomArray);
            rList2.Add(data.Value.dungeonRoomList);
        }
        dungeonMapData.curPlayerData = pList;
        dungeonMapData.dungeonIndex = iList;
        //dungeonMapData.dungeonRoomArray = rList;
        dungeonMapData.dungeonRoomList = rList2;
        dungeonMapData.curDungeonIndex = Vars.UserData.curDungeonIndex;

        dungeonMapData.curDungeonRoomData = Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData;
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
    private void SaveWorldMapNode()
    {
        worldMapSaveNodeData = new WorldMapNodeData_0();
        worldMapSaveNodeData.MapNodeStruct = Vars.UserData.WorldMapNodeStruct;
        SaveLoadSystem.Save(worldMapSaveNodeData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMapNode);
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
        consumableSaveData.curStamina = Vars.UserData.CurStamina;
        consumableSaveData.curIngameHour = Vars.UserData.CurIngameHour;
        consumableSaveData.curIngameMinute = Vars.UserData.CurIngameMinute;
        consumableSaveData.curTimeState =ConsumeManager.CurTimeState;
        consumableSaveData.date = Vars.UserData.date;
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
                var dungeonData = new DungeonData();
                dungeonData.dungeonRoomList = dungeonMapData.dungeonRoomList[i];
                //dungeonData.curDungeonData = dungeonMapData.curDungeonData[i];
                dungeonData.curPlayerData = dungeonMapData.curPlayerData[i];

                ListConvertArray(dungeonMapData.dungeonRoomList[i], dungeonData.dungeonRoomArray);

                if(!Vars.UserData.CurAllDungeonData.ContainsKey(dungeonMapData.dungeonIndex[i]))
                {
                    Vars.UserData.CurAllDungeonData.Add(dungeonMapData.dungeonIndex[i], dungeonData);
                }
               
            }
            Vars.UserData.curDungeonIndex = dungeonMapData.curDungeonIndex;
            Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData = dungeonMapData.curDungeonRoomData;
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
        worldMapSaveNodeData = (WorldMapNodeData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.WorldMapNode);
        if (worldMapSaveNodeData != null)
        {
            Vars.UserData.WorldMapNodeStruct = worldMapSaveNodeData.MapNodeStruct;
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
            Vars.UserData.CurStamina = consumableSaveData.curStamina;
            Vars.UserData.CurIngameHour = consumableSaveData.curIngameHour;
            Vars.UserData.CurIngameMinute = consumableSaveData.curIngameMinute;
            ConsumeManager.CurTimeState = consumableSaveData.curTimeState;
            Vars.UserData.date = consumableSaveData.date;
        }
    }
}