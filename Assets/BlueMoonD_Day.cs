using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlueMoonD_Day : MonoBehaviour
{
    public void Start()
    {
        GetBlueMoonDay();
    }
    private  void GetBlueMoonDay()
    {
        var date = Vars.UserData.uData.Date;
        if (date>15)
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = "블루문까지" + "\n" + 
                "D-" + (15 - date%15).ToString();
        }
        else
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = "블루문까지" +"\n"+
                "D-"+(15 - date).ToString();
        }
    }
}
