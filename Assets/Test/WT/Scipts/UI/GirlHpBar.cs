using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GirlHpBar : MonoBehaviour
{
    public Slider slider;
    void Update()
    {
        slider.value = Vars.UserData.uData.HerbalistHp / Vars.herbalistMaxHp;
    }
}
