using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum MainTutorialStage
{
    Story,
    Battle,
    Move,
    //Lanturn,
    Event,
    Stamina,
    Camp,
    Clear
}

public class MainTutorial
{
    public MainTutorialStage MainTutorialStage { get;  set; } = MainTutorialStage.Story;

    public TutorialStory tutorialStory = new TutorialStory();

    public MoveTutorial tutorialMove;
    public GatheringTutorial tutorialGathering;
    public MainRoomTutorial tutorialMainRoom;

    public void Init()
    {
        // 저장된 데이터 가져오기
        MainTutorialStage = Vars.UserData.mainTutorial;
        MainTutorialStage =
            MainTutorialStage == MainTutorialStage.Event ||
            MainTutorialStage == MainTutorialStage.Stamina ? 
            MainTutorialStage.Move : MainTutorialStage;
        Vars.UserData.mainTutorial = MainTutorialStage;
    }

    public void NextMainTutorial(bool isCheck = true)
    {
        if (MainTutorialStage != MainTutorialStage.Clear)
        {
            MainTutorialStage++;
            Vars.UserData.mainTutorial = MainTutorialStage;
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Scene);
            if(isCheck)
                GameManager.Manager.TutoManager.CheckMainTutorial();
        }
    }
}
