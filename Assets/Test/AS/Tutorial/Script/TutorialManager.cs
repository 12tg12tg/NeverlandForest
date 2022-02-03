using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    private MainTutorial mainTutorial = new MainTutorial();
    private ContentsTutorial contentsTutorial;

    public TutorialPlayer tutorialPlayer;

    public TMP_Text text;

    public void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "스토리 튜토리얼"))
        {
            StartCoroutine(mainTutorial.CoTutorialStory(text));
        }
    }

    public void CheckMainTutorial()
    {
        switch (tutorialPlayer.curMainTutorialStage)
        {
            case MainTutorialStage.None:
            case MainTutorialStage.Story:
                StartCoroutine(mainTutorial.CoTutorialStory(text));
                tutorialPlayer.curMainTutorialStage = MainTutorialStage.Battle;
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
    }
}