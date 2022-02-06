using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public MainTutorial mainTutorial = new MainTutorial();
    public ContentsTutorial contentsTutorial = new ContentsTutorial();
    public TutorialPlayer tutorialPlayer;
    public TMP_Text text;

    public void Init()
    {
        mainTutorial.Init();
        contentsTutorial.Init();
        CheckMainTutorial();
    }

    public void CheckMainTutorial()
    {
        switch (mainTutorial.MainTutorialStage)
        {
            case MainTutorialStage.Story:
                StartCoroutine(mainTutorial.tutorialStory.CoTutorialStory(text));
                break;
            case MainTutorialStage.Battle:

                break;
            case MainTutorialStage.Move:

                break;
            case MainTutorialStage.Lanturn:
                break;
            case MainTutorialStage.Event:
                break;
            case MainTutorialStage.Stamina:
                break;
            case MainTutorialStage.Camp:
                break;
            case MainTutorialStage.Clear:
                tutorialPlayer.isMainTutorial = false;
                break;
        }
        mainTutorial.NextMainTutorial();
    }
}