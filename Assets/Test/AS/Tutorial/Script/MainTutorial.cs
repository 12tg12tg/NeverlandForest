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
    public CampTutorial tutorialCamp;
    public MainRoomTutorial tutorialMainRoom;
    public void Init()
    {
        // 저장된 데이터 가져오기
        //MainTutorialStage = Vars.UserData.mainTutorial;
        MainTutorialStage = MainTutorialStage.Clear;
    }

    public void NextMainTutorial()
    {
        if (MainTutorialStage != MainTutorialStage.Clear)
        {
            MainTutorialStage++;
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Scene);
            Vars.UserData.mainTutorial = MainTutorialStage;
            GameManager.Manager.TutoManager.CheckMainTutorial();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Scene);
        }
    }
}
