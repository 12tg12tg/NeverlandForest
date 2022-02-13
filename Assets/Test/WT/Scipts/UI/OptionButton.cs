using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public GameObject panel;
    public void Open()
    {
        GameManager.Manager.optionBlackPanel.SetActive(true);
    }
}
