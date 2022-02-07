using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionPanelManager : MonoBehaviour
{

    public GameObject bgmButton;
    public GameObject soundButton;


    public Sprite bgmOnImage;
    public Sprite bgmOffImage;

    public Sprite soundOnImage;
    public Sprite soundOffImage;

    private bool isBgmOn =true;
    private bool isSoundOn = true;

    public void Update()
    {
        
    }

    public  void BgmButtonClick()
    {
        isBgmOn = !isBgmOn;

        if (isBgmOn)
        {
            bgmButton.GetComponent<Image>().sprite = bgmOnImage;
        }
        else
        {
            bgmButton.GetComponent<Image>().sprite = bgmOffImage;
        }
    }
    public void SoundButtonClick()
    {
        isSoundOn = !isSoundOn;

        if (isSoundOn)
        {
            soundButton.GetComponent<Image>().sprite = soundOnImage;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOffImage;
        }
    }

    public void DungeonGiveUp()
    {

    }
    public void ProgramDown()
    {

    }

}
