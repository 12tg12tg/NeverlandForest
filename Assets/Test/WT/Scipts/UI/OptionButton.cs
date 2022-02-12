using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public void Open()
    {
        GameManager.Manager.ObtionPanelUi.SetActive(true);
    }
   
}
