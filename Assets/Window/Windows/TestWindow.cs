using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestWindow : GenericWindow
{
    public Button closeButton;
    public override void Open()
    {
        var canContinue = true;

        closeButton.gameObject.SetActive(canContinue);

        if (closeButton.gameObject.activeSelf)
        {
            firstSelected = closeButton.gameObject;
        }
        base.Open();
        Debug.Log("Å×½ºÆ® open");
    }
    public override void Close()
    {
        base.Close();
    }
}
