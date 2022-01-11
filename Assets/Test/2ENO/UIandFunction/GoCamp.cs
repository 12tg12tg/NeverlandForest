using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoCamp : MonoBehaviour
{
    public DungeonSystem dungeonSystem;
    public void OpenCampScene()
    {
        dungeonSystem.DungeonSystemData.curPlayerData.SetUnitData(dungeonSystem.dungeonPlayer);
        Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystem.DungeonSystemData;

        SceneManager.LoadScene("JYK_Test_Main");
    }
}
