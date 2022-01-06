using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoyHpBar : MonoBehaviour
{
    public Slider slider;
    public void Start()
    {
       /* Debug.Log($" Vars.UserData.hunterHp { Vars.UserData.hunterHp }");
        Debug.Log($"Vars.hunterMaxHp {Vars.hunterMaxHp}");*/
    }
    void Update()
    {
        slider.value = Vars.UserData.hunterHp / Vars.hunterMaxHp;
    }
}
