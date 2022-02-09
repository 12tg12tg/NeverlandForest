using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternBar : MonoBehaviour
{
    public Slider slider;
    public Slider blight;
    public Image day_correction;
    private RectTransform day_correction_StartScale;
    private float scaleUPtime = 0.3f;
    private float scaleDownTime = 0.3f;
    public void Start()
    {
        ConsumeManager.init();
        day_correction_StartScale = day_correction.rectTransform;
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


        if (ConsumeManager.CurTimeState == TimeState.DayTime)
        {
            day_correction.gameObject.SetActive(true);
         /*   Utility.CoScaleChange(day_correction.GetComponent<RectTransform>(),
                new Vector3(4f, 4f, 4f), scaleUPtime);
            Utility.CoScaleChange(day_correction.GetComponent<RectTransform>(),
               new Vector3(1f, 1f, 1f), scaleDownTime);*/
        }
        else
        {
            day_correction.gameObject.SetActive(false);
        }
    }
}
