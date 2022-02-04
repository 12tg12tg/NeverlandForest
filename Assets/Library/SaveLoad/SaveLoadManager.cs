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
    CraftSaveData_0 craftSaveData;
    RandomEventSaveData_0 randomEvent;
    ItemExperienceSaveData_0 itemExperienceSaveData;
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
            case SaveLoadSystem.SaveType.DungeonMap:
                SaveDungeonMap();
                break;
            case SaveLoadSystem.SaveType.WorldMapData:
                SaveWorldMapData();
                break;
            case SaveLoadSystem.SaveType.WorldMapPlayerData:
                SaveWorldMapPlayer();
                break;
            case SaveLoadSystem.SaveType.Craft:
                SaveCraft();
                break;
            case SaveLoadSystem.SaveType.ItemExperience:
                SaveExperience();
                break;
            case SaveLoadSystem.SaveType.RandomEvent:
                SaveRandomEvent();
            case SaveLoadSystem.SaveType.ConsumableData:
                SaveConsumableData();
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
            case SaveLoadSystem.SaveType.DungeonMap:
                LoadDungeonMap();
                break;
            case SaveLoadSystem.SaveType.WorldMapData:
                LoadWorldMapNode();
                break;
            case SaveLoadSystem.SaveType.WorldMapPlayerData:
                LoadWorldMapPlayer();
                break;
            case SaveLoadSystem.SaveType.Craft:
                LoadCraft();
                break;
            case SaveLoadSystem.SaveType.ItemExperience:
                LoadExperience();
                break;
            case SaveLoadSystem.SaveType.RandomEvent:
                LoadRandomEvent();
                break;
            case SaveLoadSystem.SaveType.ConsumableData:
                LoadConsumableData();
                break;
        }
    }
    private void SavePlayer()
    {
    }
    private void SaveOption()
    {
    }

    private void SaveRandomEvent()
    {
        randomEvent = new RandomEventSaveData_0();

        randomEvent.randomEventAllData.AddRange(Vars.UserData.randomEventDatas);
        randomEvent.useEventIDs.AddRange(Vars.UserData.useEventID);
        randomEvent.isFirst = Vars.UserData.isFirst;

        SaveLoadSystem.Save(randomEvent, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.RandomEvent);
    }

    private void SaveDungeonMap()
    {
        dungeonMapData = new DungeonMapSaveData_0();
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
            pList.Add(data.Value.curPlayerGirlData);
            pList2.Add(data.Value.curPlayerBoyData);
            iList.Add(data.Key);
            rList2.Add(data.Value.dungeonRoomList);
        }
        dungeonMapData.curPlayerGirlData = pList;
        dungeonMapData.curPlayerBoyData = pList2;
        dungeonMapData.dungeonIndex = iList;
        dungeonMapData.dungeonRoomList = rList2;
        dungeonMapData.curDungeonIndex = Vars.UserData.curDungeonIndex;
        dungeonMapData.curDungeonRoomIndex = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData.roomIdx;
        SaveLoadSystem.Save(dungeonMapData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.DungeonMap);
    }

    private List<DungeonRoom> ArrayConvertList(DungeonRoom[] array)
    {
        List<DungeonRoom> list = new List<DungeonRoom>();
        // ���� �ε���
        int curIdx = 100;
        while (array[curIdx].nextRoomIdx != -1)
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
    private void SaveCraft()
    {
        craftSaveData = new CraftSaveData_0();
        craftSaveData.haveCraft = Vars.UserData.HaveCraftIDList;
        SaveLoadSystem.Save(craftSaveData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Craft);

    }
    private void SaveExperience()
    {
        itemExperienceSaveData = new ItemExperienceSaveData_0();
        itemExperienceSaveData.haveItemExperience = Vars.UserData.experienceHaveItemList;
        SaveLoadSystem.Save(itemExperienceSaveData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.ItemExperience);

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
        consumableSaveData.curIngameHour = Vars.UserData.uData.CurIngameHour;
        consumableSaveData.curIngameMinute = Vars.UserData.uData.CurIngameMinute;

        consumableSaveData.curTimeState = ConsumeManager.CurTimeState;
        consumableSaveData.curlanternstate = ConsumeManager.CurLanternState;

        consumableSaveData.date = Vars.UserData.uData.Date;
        consumableSaveData.tiredness = Vars.UserData.uData.Tiredness;
        consumableSaveData.hunger = Vars.UserData.uData.Hunger;
        consumableSaveData.bonfireTime = Vars.UserData.uData.BonfireHour;
        SaveLoadSystem.Save(consumableSaveData, SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.ConsumableData);

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
    private void LoadCraft()
    {
        craftSaveData = (CraftSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.Craft);
        if (craftSaveData != null)
        {
            Vars.UserData.HaveCraftIDList = craftSaveData.haveCraft;
        }
    }

    private void LoadExperience()
    {
        itemExperienceSaveData = (ItemExperienceSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.ItemExperience);
        if (itemExperienceSaveData != null)
        {
            Vars.UserData.experienceHaveItemList = itemExperienceSaveData.haveItemExperience;
        }
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

                if (!Vars.UserData.AllDungeonData.ContainsKey(dungeonMapData.dungeonIndex[i]))
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
    private void LoadRandomEvent()
    {
        randomEvent = (RandomEventSaveData_0)SaveLoadSystem.Load(SaveLoadSystem.Modes.Text, SaveLoadSystem.SaveType.RandomEvent);
        if(randomEvent != null)
        {
            Vars.UserData.randomEventDatas = randomEvent.randomEventAllData;
            Vars.UserData.useEventID = randomEvent.useEventIDs;
            Vars.UserData.isFirst = randomEvent.isFirst;
        }
    }
    private void ListConvertArray(List<DungeonRoom> list, DungeonRoom[] array)
    {
        foreach (var data in list)
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
            Vars.UserData.uData.Tiredness = consumableSaveData.tiredness;
            Vars.UserData.uData.CurIngameHour = consumableSaveData.curIngameHour;
            Vars.UserData.uData.CurIngameMinute = consumableSaveData.curIngameMinute;

            ConsumeManager.CurTimeState = consumableSaveData.curTimeState;
            ConsumeManager.CurLanternState = consumableSaveData.curlanternstate;

            Vars.UserData.uData.Date = consumableSaveData.date;
            Vars.UserData.uData.Hunger = consumableSaveData.hunger;
            Vars.UserData.uData.BonfireHour   = consumableSaveData.bonfireTime;
        }
    }
}