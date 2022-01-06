using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternBar : MonoBehaviour
{
    public Slider slider;
    void Start()
    {
      /*  Debug.Log($" Vars.UserData.LanternCount { Vars.UserData.LanternCount }");
        Debug.Log($"Vars.LanternMaxCount {Vars.LanternMaxCount}");*/
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Vars.UserData.LanternCount / Vars.LanternMaxCount;
    }
}
