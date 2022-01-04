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
            // ��
            ConsumeManager.CurTimeState = TimeState.DayTime;
            currentValue = ConsumeManager.CurIngameHour+(ConsumeManager.CurIngameMinute/60);
            progressIndicator.text = "��";
        }
        else
        {
            // �� 
            ConsumeManager.CurTimeState = TimeState.NightTime;
            progressIndicator.text = "��";
        }

        timeloadingBar.fillAmount = currentValue / 24;
    }
}
