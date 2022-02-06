using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimePrograss : MonoBehaviour
{
    private float currentValue = 0f;

    public Image timeloadingBar;
    public Image changableDayState;

    public Sprite daySprite;
    public Sprite nightSprite1;
    public Sprite nightSprite2;
    public Sprite nightSprite3;
    public Sprite nightSprite4;
    public Sprite nightSprite5;
    
    void Update()
    {
        if (currentValue <= 12)
        {
            // ³·
            ConsumeManager.CurTimeState = TimeState.DayTime;
            changableDayState.sprite = daySprite;
        }
        else
        {
            // ¹ã 
            ConsumeManager.CurTimeState = TimeState.NightTime;
            var date = Vars.UserData.uData.Date;
            if (date%15==1 || date % 15 == 2|| date % 15 == 3)
            {
                changableDayState.sprite = nightSprite1;
            }
            else if (date % 15 == 4 || date % 15 == 5 || date % 15 == 6)
            {
                changableDayState.sprite = nightSprite2;
            }
            else if (date % 15 ==7 || date % 15 == 8 || date % 15 == 9)
            {
                changableDayState.sprite = nightSprite3;
            }
            else if (date % 15 == 10 || date % 15 == 11 || date % 15 == 12 || date % 15 == 13 || date % 15 == 14)
            {
                changableDayState.sprite = nightSprite4;
            }
            else
            {
                changableDayState.sprite = nightSprite5;
            }
        }
        currentValue = Vars.UserData.uData.CurIngameHour + (Vars.UserData.uData.CurIngameMinute / 60);
        timeloadingBar.fillAmount = currentValue / 24;
    }
}
