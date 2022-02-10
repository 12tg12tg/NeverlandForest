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
        // ����� ������ ��������
        MainTutorialStage = Vars.UserData.mainTutorial;
    }

    public void NextMainTutorial()
    {
        if (MainTutorialStage != MainTutorialStage.Clear)
        {
            MainTutorialStage++;
            Vars.UserData.mainTutorial = MainTutorialStage;
            GameManager.Manager.TutoManager.CheckMainTutorial();
        }
    }
}
