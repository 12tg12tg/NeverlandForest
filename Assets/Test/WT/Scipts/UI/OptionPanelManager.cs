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

    private bool isBgmOn =true;
    private bool isSoundOn = true;
    public  void BgmButtonClick()
    {
        isBgmOn = !isBgmOn;
        var buttonImg = bgmButton.GetComponent<Image>().sprite;
        var soundManger = SoundManager.Instance;
        if (isBgmOn)
        {
            buttonImg = bgmOnImage;
            soundManger.MuteBgm = false;
        }
        else
        {
            buttonImg = bgmOffImage;
            soundManger.MuteBgm = true;
        }
        soundManger.SetMuteBGM(soundManger.MuteBgm);
    }
    public void SoundButtonClick()
    {
        isSoundOn = !isSoundOn;
        var soundManger = SoundManager.Instance;
       
        var buttonImg = soundButton.GetComponent<Image>().sprite;
        if (isBgmOn)
        {
            buttonImg = soundOnImage;
            soundManger.MuteSf = false;
        }
        else
        {
            buttonImg = soundOffImage;
            soundManger.MuteSf = true;
        }
        SoundManager.Instance.SetMuteBGM(soundManger.MuteSf);
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

    public void CloseAndSoundSave()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Option);
    }

}
