using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimePrograss : MonoBehaviour
{
    public TextMeshProUGUI progressIndicator;
    public Image timeloadingBar;

    private float currentValue= 0f;
    
    void Update()
    {
        if (currentValue <= 12)
        {
            // ³·
            ConsumeManager.CurTimeState = TimeState.DayTime;
            progressIndicator.text = "³·";
        }
        else
        {
            // ¹ã 
            ConsumeManager.CurTimeState = TimeState.NightTime;
            progressIndicator.text = "¹ã";
        }

        currentValue = Vars.UserData.uData.CurIngameHour + (Vars.UserData.uData.CurIngameMinute / 60);
        timeloadingBar.fillAmount = currentValue / 24;
    }
}
