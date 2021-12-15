using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    PlayerSaveData_1 playerDate;
    OptionSaveData_0 optionDate;

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
        }
    }

    private void SavePlayer()
    {

    }
    private void SaveOption()
    {

    }
    private void LoadPlayer()
    {

    }
    private void LoadOption()
    {

    }
}
