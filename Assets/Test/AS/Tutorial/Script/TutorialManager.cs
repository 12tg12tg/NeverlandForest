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
        var gm = GameManager.Manager;
        switch (mainTutorial.MainTutorialStage)
        {
            case MainTutorialStage.Story:
                var canvasGo = GameObject.FindGameObjectWithTag("StoryCanvas");
                storyBoard = canvasGo.transform.GetChild(1).gameObject;
                storyBoard.SetActive(true);
                text = storyBoard.transform.GetChild(1).GetComponent<TMP_Text>();
                gm.Production.black.SetActive(false);
                StartCoroutine(mainTutorial.tutorialStory.CoTutorialStory(text, () => {
                    mainTutorial.NextMainTutorial();
                    CheckMainTutorial();
                    text.transform.parent.gameObject.SetActive(false);
                    storyBoard.SetActive(false);
                }));
                break;
            case MainTutorialStage.Battle:
                BattleManager.initState = BattleInitState.Tutorial;
                gm.Production.FadeIn(() => gm.LoadScene(GameScene.Battle));
                break;
            case MainTutorialStage.Move:
                gm.Production.FadeIn(() => gm.LoadScene(GameScene.TutorialDungeon));
                break;
            case MainTutorialStage.Event:
                gm.Production.FadeIn(() => gm.LoadScene(GameScene.TutorialDungeon));
                break;
            case MainTutorialStage.Stamina:
                gm.Production.FadeIn(() => gm.LoadScene(GameScene.TutorialDungeon));
                break;
            case MainTutorialStage.Camp:
                CampManager.curinitState = CampManager.CampinitState.Tutorial;
                gm.Production.FadeIn(() => gm.LoadScene(GameScene.Camp));
                break;
            case MainTutorialStage.Clear:
                if(Vars.UserData.isPlayerDungeonIn)
                {
                    gm.Production.FadeIn(() => gm.LoadScene(GameScene.Dungeon));
                }
                else
                {
                    GameManager.Manager.LoadScene(GameScene.World);
                }
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