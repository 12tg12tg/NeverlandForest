using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampManager : MonoBehaviour
{
    public enum CampEvent
    {
        StartCook,
        StartGathering,
        StartSleep,
    }

    public void OnEnable()
    {
        EventBus<CampEvent>.Subscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartSleep, StartSleep);
    }
    private void OnDisable()
    {
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.ResetEventBus();
    }

    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Cooking Scene");
        SceneManager.LoadScene("Wt_Scene");
    }

    public void OpenGatheringScene(object[] vals)
    {
        if (vals.Length != 0) return;
        //SceneManager.LoadScene("WorldMap");
        Debug.Log($"Open OpenGathering Scene");
    }

    public void StartSleep(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Go Sleep ");
    }
    public void OpenDiary()
    {
        Debug.Log($"Open Diary window");
    }
    public void OpenInventory()
    {
        Debug.Log($"Open Inventory window");
    }
    public void GoWorldMap()
    {
        SceneManager.LoadScene("WorldMap");
    }
    public void GoDungeon()
    {
       SceneManager.LoadScene("2ENO_RandomMap");
    }
   
    public void OpenMiniMap()
    {
        Debug.Log($"Open MiniMap ");
    }
}
