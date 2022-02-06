using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public GameObject OptionPanel;

    public void OnClickButton()
    {
        OptionPanel.SetActive(true);
    }
   
}
