using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartWindow : GenericWindow
{
    public Button newGamebutton;

    public override void Open()
    {
        var canContinue = true;

        newGamebutton.gameObject.SetActive(canContinue);

        if (newGamebutton.gameObject.activeSelf)
        {
            firstSelected = newGamebutton.gameObject;
        }
        base.Open();
        Debug.Log("start¿ÀÇÂ");
    }
    public void NewGame()
    {
        OnNextWindow();
    }
}
