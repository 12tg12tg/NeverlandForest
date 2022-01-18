using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoCamp : MonoBehaviour
{
    public DungeonSystem dungeonSystem;
    public void OpenCampScene()
    {
        dungeonSystem.DungeonSystemData.curPlayerGirlData.SetUnitData(dungeonSystem.dungeonPlayerGirl);
        dungeonSystem.DungeonSystemData.curPlayerBoyData.SetUnitData(dungeonSystem.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = dungeonSystem.DungeonSystemData;

        SceneManager.LoadScene("JYK_Test_Main");
    }
}
