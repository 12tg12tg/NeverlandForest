using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GirlHpBar : MonoBehaviour
{
    public Slider slider;
    public void Start()
    {
      /*  Debug.Log($" Vars.UserData.herbalistHp { Vars.UserData.herbalistHp }");
        Debug.Log($"Vars.herbalistMaxHp {Vars.herbalistMaxHp}");*/
    }
    void Update()
    {
        slider.value = Vars.UserData.uData.HerbalistHp / Vars.herbalistMaxHp;
    }
}
