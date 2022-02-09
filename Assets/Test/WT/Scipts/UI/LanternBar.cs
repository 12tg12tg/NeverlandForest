using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternBar : MonoBehaviour
{
    public Slider slider;
    public Image day_correction;
    public void Start()
    {
        ConsumeManager.Init(); 
    }
    void Update()
    {
        slider.value = Vars.UserData.uData.LanternCount / Vars.lanternMaxCount;

        if (ConsumeManager.CurTimeState == TimeState.DayTime)
        {
            day_correction.gameObject.SetActive(true);
        }
        else
        {
            day_correction.gameObject.SetActive(false);
        }
    }
}
