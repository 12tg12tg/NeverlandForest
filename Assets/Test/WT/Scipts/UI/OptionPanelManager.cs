using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionPanelManager : MonoBehaviour
{
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button soundButton;

    [SerializeField] private Sprite bgmOnImage;
    [SerializeField] private Sprite bgmOffImage;

    [SerializeField] private Sprite soundOnImage;
    [SerializeField] private Sprite soundOffImage;

    [SerializeField] private Button giveUpBtn;

    private bool isBgmOn =true;
    private bool isSoundOn = true;

    private void OnEnable()
    {
        if(GameManager.Manager.State != GameState.Dungeon)
        {
            giveUpBtn.interactable = false;
        }
    }

    public  void BgmButtonClick()
    {
        isBgmOn = !isBgmOn;
        var buttonImg = bgmButton.GetComponent<Image>();
        var soundManger = SoundManager.Instance;
        if (isBgmOn)
        {
            buttonImg.sprite = bgmOnImage;
            soundManger.MuteBgm = false;
        }
        else
        {
            buttonImg.sprite = bgmOffImage;
            soundManger.MuteBgm = true;
        }
        soundManger.SetMuteBGM();
    }
    public void SoundButtonClick()
    {
        isSoundOn = !isSoundOn;
        var soundManger = SoundManager.Instance;
       
        var buttonImg = soundButton.GetComponent<Image>();
        if (isBgmOn)
        {
            buttonImg.sprite = soundOnImage;
            soundManger.MuteSf = false;
        }
        else
        {
            buttonImg.sprite = soundOffImage;
            soundManger.MuteSf = true;
        }
        soundManger.SetMuteSFX();
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

    public void SaveSoundData()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Option);
    }

}
