using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public MainTutorial mainTutorial = new MainTutorial();
    public ContentsTutorial contentsTutorial = new ContentsTutorial();
    public TutorialPlayer tutorialPlayer;
    public TMP_Text text;

    [Header("UI")]
    public RectTransform handIcon;
    public RectTransform dialogBox;
    public RectTransform blackout;
    public Button nextStepButton;
    public TMP_Text dialogBoxText;

    [Header("��������Ʈ")]
    public Sprite rect;
    public Sprite circle;

    public void Init()
    {
        mainTutorial.Init();
        contentsTutorial.Init();
        nextStepButton.gameObject.SetActive(false);
        //CheckMainTutorial();
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
                mainTutorial.tutorialMove = gameObject.AddComponent<MoveTutorial>();
                StartCoroutine(mainTutorial.tutorialMove.CoMoveTutorial());
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

    public void NextStep()
    {
        mainTutorial.tutorialMove.TutorialStep++;
        mainTutorial.tutorialMove.delay = 0f;
    }
}