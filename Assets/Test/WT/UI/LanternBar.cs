using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternBar : MonoBehaviour
{
    public Slider slider;
    public Slider blight;
    // Update is called once per frame
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
        else
        {
            blight.value = 0;
        }

    }
}
