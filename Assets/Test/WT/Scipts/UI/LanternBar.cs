using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternBar : MonoBehaviour
{
    public Slider slider;
    public Slider blight;
    // Update is called once per frame
    public void Start()
    {
        ConsumeManager.init(); 
    }
    void Update()
    {
        slider.value = Vars.UserData.uData.LanternCount / Vars.lanternMaxCount;
        if (Vars.UserData.uData.lanternState== LanternState.Level4)
        {
            blight.value = 1f;
        }
        else if(Vars.UserData.uData.lanternState == LanternState.Level3)
        {
            blight.value = 0.75f;
        }
        else if (Vars.UserData.uData.lanternState == LanternState.Level2)
        {
            blight.value = 0.5f;
        }
        else if (Vars.UserData.uData.lanternState == LanternState.Level1)
        {
            blight.value = 0.25f;
        }
        else if (Vars.UserData.uData.lanternState == LanternState.None)
        {
            blight.value = 0f;
        }
    }
    public void OnGUI()
    {
       /* if (GUILayout.Button("랜턴밝기검사"))
        {
            Debug.Log(slider.value);
            Debug.Log(blight.value);
            Debug.Log($" Vars.UserData.uData.LanternCount{ Vars.UserData.uData.LanternCount}");
            Debug.Log($" Vars.lanternMaxCount{ Vars.lanternMaxCount}");
            Debug.Log($"Vars.UserData.uData.lanternState{Vars.UserData.uData.lanternState}");
        }*/
    }
}
