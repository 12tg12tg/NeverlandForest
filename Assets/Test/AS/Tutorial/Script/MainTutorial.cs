using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum MainTutorialStage
{
    Story,
    Battle,
    Move,
    Lanturn,
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
    public void Init()
    {
        // 저장된 데이터 가져오기
        MainTutorialStage = MainTutorialStage.Move;
    }

    public void NextMainTutorial()
    {
        if (MainTutorialStage != MainTutorialStage.Clear)
        {
            if (MainTutorialStage == MainTutorialStage.Move)
                MainTutorialStage += 2;
            else
                MainTutorialStage++;
        }
    }
}
