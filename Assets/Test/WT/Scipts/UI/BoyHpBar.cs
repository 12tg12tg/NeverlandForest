using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoyHpBar : MonoBehaviour
{
    public Slider slider;
    void Update()
    {
        slider.value = Vars.UserData.uData.Hp / Vars.maxHp;
    }
}
