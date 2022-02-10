using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternBar : MonoBehaviour
{
    public Slider slider;
    public Image day_correction;
    private RectTransform day_correction_StartScale;
    private float scaleUPtime = 0.3f;
    private float scaleDownTime = 0.3f;
    public void Start()
    {
        ConsumeManager.Init();
        day_correction_StartScale = day_correction.rectTransform;
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
