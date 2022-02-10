using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public MainTutorial mainTutorial = new MainTutorial();
    public ContentsTutorial contentsTutorial = new ContentsTutorial();
    public TMP_Text text;

    [Header("UI")]
    public RectTransform handIcon;
    public RectTransform dialogBox;
    public GameObject blackoutPanel;
    public TMP_Text dialogBoxText;
    public GameObject storyBoard;
    public RectTransform blackout;

    [Header("스프라이트")]
    public Sprite rect;
    public Sprite circle;

    public void Init()
    {
        if(blackoutPanel != null)
            blackout = blackoutPanel.transform.GetChild(0).GetComponent<RectTransform>();

        mainTutorial.Init(); // 저장정보불러오기
        contentsTutorial.Init();

        CheckMainTutorial();
    }

    public void CheckMainTutorial()
    {
        switch (mainTutorial.MainTutorialStage)
        {
            case MainTutorialStage.Story:
                storyBoard.SetActive(true);
                StartCoroutine(mainTutorial.tutorialStory.CoTutorialStory(text, () => {
                    mainTutorial.NextMainTutorial();
                    CheckMainTutorial();
                    SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Scene);
                    text.transform.parent.gameObject.SetActive(false);
                    storyBoard.SetActive(false);
                }));
                break;
            case MainTutorialStage.Battle:
                BattleManager.initState = BattleInitState.Tutorial;
                var gm = GameManager.Manager;
                gm.Production.FadeIn(() => gm.LoadScene(GameScene.Battle));
                break;
            case MainTutorialStage.Move:
                mainTutorial.tutorialMove.delay = 0f;
                StartCoroutine(mainTutorial.tutorialMove.CoMoveTutorial());
                break;
            case MainTutorialStage.Event:
                StartCoroutine(mainTutorial.tutorialGathering.CoGatheringTutorial());
                break;
            case MainTutorialStage.Stamina:
                mainTutorial.tutorialMainRoom.delay = 0f;
                StartCoroutine(mainTutorial.tutorialMainRoom.CoMainRoomTutorial());
                break;
            case MainTutorialStage.Camp:
                StartCoroutine(mainTutorial.tutorialCamp.CoCampTutorial());
                break;
            case MainTutorialStage.Clear:
                StartCoroutine(mainTutorial.tutorialMainRoom.CoTutorialEnd());
                break;
        }
    }

    public Button TutorialTargetButtonActivate(Button target)
    {
        var targetObject = target.gameObject;
        var clone = Instantiate(targetObject, blackoutPanel.transform, true);

        var btn = clone.GetComponent<Button>();
        btn.onClick.AddListener(() =>  Destroy(btn.gameObject) );

        return btn;
    }

    public void BlackPanelOn()
    {
        var panel = blackoutPanel.transform.GetChild(1).gameObject;
        panel.SetActive(true);
    }

    public void BlackPanelOff()
    {
        var panel = blackoutPanel.transform.GetChild(1).gameObject;
        panel.SetActive(false);
    }
}