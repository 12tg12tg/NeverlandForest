using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoCamp : MonoBehaviour
{
    public void OpenCampScene()
    {
        DungeonSystem.Instance.DungeonSystemData.curPlayerGirlData.SetUnitData(DungeonSystem.Instance.dungeonPlayerGirl);
        DungeonSystem.Instance.DungeonSystemData.curPlayerBoyData.SetUnitData(DungeonSystem.Instance.dungeonPlayerBoy);
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex] = DungeonSystem.Instance.DungeonSystemData;

        SceneManager.LoadScene("JYK_Test_Main");
    }
}
