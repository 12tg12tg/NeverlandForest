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
        // TODO : 던전 포기 기능은 던전 씬에서만 동작 하도록 추가 해야함
        if (GameManager.Manager.State ==GameState.Dungeon)
        {
            Vars.UserData.WorldMapPlayerData.isClear = false;
            SoundManager.Instance.Play(SoundType.Se_Fail);
            GameManager.Manager.LoadScene(GameScene.World);
        }
    }
    public void ProgramDown() => GameManager.Manager.GoToGameEnd();
}
