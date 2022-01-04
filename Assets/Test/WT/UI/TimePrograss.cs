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
    public float speed;
    
    void Update()
    {
        if (currentValue < 100)
        {
            currentValue += speed * Time.deltaTime;
            progressIndicator.text = ((int)currentValue).ToString() + "%";
        }
        else
        {
            progressIndicator.text = "Done";
        }

        timeloadingBar.fillAmount = currentValue / 100;
    }
}
