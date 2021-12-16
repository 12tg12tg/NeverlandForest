using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public class SaveLoadTest : MonoBehaviour
{
    public PlayerSaveData_0 data0 = new PlayerSaveData_0();
    public PlayerSaveData_1 data1 = new PlayerSaveData_1();

    public int recentVersion;
    public int num;
    public string str;
    public bool isZero = true;
    public List<int> list = new List<int>();
    public Color color;



    private void Start()
    {
        SaveLoadSystem.Init();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Save Text 0"))
        {
            MakeSave0();
            Save0(SaveLoadSystem.Modes.Text);
        }
        if (GUILayout.Button("Load Text 0"))
        {
            Load0(SaveLoadSystem.Modes.Text);
        }

        if(GUILayout.Button("Save Bin 0"))
        {
            MakeSave0();
            Save0(SaveLoadSystem.Modes.Binary);
        }

        if (GUILayout.Button("Load Bin 0"))
        {
            Load0(SaveLoadSystem.Modes.Binary);
        }

        if (GUILayout.Button("Save Text 1"))
        {
            MakeSave1();
            Save1(SaveLoadSystem.Modes.Text);
        }
        if (GUILayout.Button("Load Text 1"))
        {
            Load1(SaveLoadSystem.Modes.Text);
        }

        if (GUILayout.Button("Save Binary 1"))
        {
            MakeSave1();
            Save1(SaveLoadSystem.Modes.Binary);
        }
        if (GUILayout.Button("Load Binary 1"))
        {
            Load1(SaveLoadSystem.Modes.Binary);
        }
    }

    private void MakeSave0()
    {
        data0.intData = num;
        data0.stringData = str;
        data0.boolData = isZero;
        if (data0.listData == null)
            data0.listData = new List<int>();
        data0.listData.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            data0.listData.Add(list[i]);
        }
    }

    private void MakeSave1()
    {
        data1.intData = num;
        data1.stringData = str;
        data1.boolData = isZero;
        if (data1.listData == null)
            data1.listData = new List<int>();
        data1.listData.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            data1.listData.Add(list[i]);
        }
        data1.color = color;
    }

    public void Save0(SaveLoadSystem.Modes mode)
    {
        Debug.Log("저장진행0");
        SaveLoadSystem.Save(data0, mode, SaveLoadSystem.SaveType.Player);
        string json = JsonConvert.SerializeObject(data0, Formatting.Indented);
        Debug.Log(json);
    }
    public void Save1(SaveLoadSystem.Modes mode)
    {
        Debug.Log("저장진행1");
        SaveLoadSystem.Save(data1, mode, SaveLoadSystem.SaveType.Player);
        string json = JsonConvert.SerializeObject(data0, Formatting.Indented);
        Debug.Log(json);
    }

    public void Load0(SaveLoadSystem.Modes mode)
    {
        Debug.Log("로드진행");
        SaveDataBase data = SaveLoadSystem.Load(mode, SaveLoadSystem.SaveType.Player);
        data0 = data as PlayerSaveData_0;

        recentVersion = data0.Version;
        num = data0.intData;
        str = data0.stringData;
        isZero = data0.boolData;
        list.Clear();
        for (int i = 0; i < data0.listData.Count; i++)
        {
            list.Add(data0.listData[i]);
        }
        //color = data0.color;
    }

    public void Load1(SaveLoadSystem.Modes mode)
    {
        Debug.Log("로드진행");
        SaveDataBase data = SaveLoadSystem.Load(mode, SaveLoadSystem.SaveType.Player);
        data1 = data as PlayerSaveData_1;

        recentVersion = data1.Version;
        num = data1.intData;
        str = data1.stringData;
        isZero = data1.boolData;
        list.Clear();
        for (int i = 0; i < data1.listData.Count; i++)
        {
            list.Add(data1.listData[i]);
        }
        color = data1.color;
    }
}
