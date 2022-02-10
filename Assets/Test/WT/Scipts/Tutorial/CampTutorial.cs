using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CampTutorial : MonoBehaviour
{
    public Button bottomitemTagButton;
    public Button campBonableCheckButton;
    public Button campDiaryquickButton;
    public GameObject blackPanel;

    private RectTransform target;

    private Rect canvasRt;
    private RectTransform handIcon;
    private RectTransform dialogBox;
    private TMP_Text dialogText;
    private RectTransform blackout;

    private Sprite rect;
    private Sprite circle;

    private readonly float arrowSize = 50f;
    private readonly float boxWidth = 250f;
    private readonly float boxHeight = 100f;

    private DialogBoxObject dialogBoxObj;

    private bool isquitbuttonClick;
    public bool IsquitbuttonClick
    {
        get => isquitbuttonClick;
        set { isquitbuttonClick = value; }
    }
    private bool  tutorialCraftTouch;
    public bool TutorialCraftTouch
    {
        get => tutorialCraftTouch;
        set { tutorialCraftTouch = value; }
    }
    private bool iscraftFinish;
    public bool IscraftFinish
    {
        get => iscraftFinish;
        set { iscraftFinish = value; }
    }
    private bool  tutorialCookingTouch;
    public bool TutorialCookingTouch
    {
        get => tutorialCookingTouch;
        set { tutorialCookingTouch = value; }
    }
    private bool iscookingFinish;
    public bool IscookingFinish
    {
        get => iscookingFinish;
        set { iscookingFinish = value; }
    }

    private bool tutorialBonfirecheckButtonClick;
    public bool TutorialBonfirecheckButtonClick
    {
        get => tutorialBonfirecheckButtonClick;
        set { tutorialBonfirecheckButtonClick = value; }
    }
    private bool isWaitingTouch;
    private bool tutorialBonableItemCheck;
    private bool tutorialBonableItemCheckFinish;
    public bool TutorialBonableItemCheckFinish
    {
        get => tutorialBonableItemCheckFinish;
        set { tutorialBonableItemCheckFinish = value; }
    }
    private bool  tutorialSleepingTouch;
    public bool TutorialSleepingTouch
    {
        get => tutorialSleepingTouch;
        set { tutorialSleepingTouch = value; }
    }

    private bool issleepingFinish;
    public bool IssleepingFinish
    {
        get => issleepingFinish;
        set { issleepingFinish = value; }
    }


    private bool  tutorialGahteringingTouch;
    public bool TutorialGahteringingTouch
    {
        get => tutorialGahteringingTouch;
        set { tutorialGahteringingTouch = value; }
    }
    private bool isTutorialFirst;
    public bool IsTutorialFirst
    {
        get => isTutorialFirst;
        set { isTutorialFirst = value; }
    }
  

    private void Start()
    {
        var tm = GameManager.Manager.TutoManager;
        tm.mainTutorial.tutorialCamp = this;
        dialogBox = tm.dialogBox;
        handIcon = tm.handIcon;
        blackout = tm.blackout;
        rect = tm.rect;
        circle = tm.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();
        //Vars.UserData.isTutorialCamp = true;
        if (Vars.UserData.isTutorialCamp)
        {
            GameManager.Manager.State = GameState.Tutorial;
            TutorialStart();
        }
        else
        {
            gameObject.SetActive(false);
            blackPanel.SetActive(false);
        }

    }
    private void Update()
    {
        if (isWaitingTouch)
        {
            if (GameManager.Manager.MultiTouch.TouchCount > 0)
            {
                tutorialBonableItemCheck = true;
                isWaitingTouch = false;
            }
        }
    }

    public void TutorialStart()
    {
        var tm = GameManager.Manager.TutoManager;
        tm.mainTutorial.MainTutorialStage = MainTutorialStage.Camp;
        tm.CheckMainTutorial();
    }

    public IEnumerator CoCampTutorial()
    {
        isTutorialFirst = true;
        var tm = GameManager.Manager.TutoManager;
        LongTouch(0.25f,0.6f,0.5f,0.6f,"이것은 제작을 할수있는 공간이야. \n" +
            "도구나, 설치물,물약류를 만들수 있는곳이지. \n" +
            "물론 제조법이 있어야 가능할거야.",true); //craft 

        yield return new WaitUntil(() => tutorialCraftTouch);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        tm.BlackPanelOn();
        tm.TutorialTargetButtonActivate(campDiaryquickButton);

        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 다음것은 진행해보자",false,true); //Craft xbutton
        tutorialCraftTouch = false;
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        tm.BlackPanelOff();
        SetActive(false);
        iscraftFinish = true;

        yield return new WaitForSeconds(2.5f);
        LongTouch(0.68f, 0.5f, 0.5f, 0.5f, "이것은 요리를 할 수 있는 솥이야. \n" +
            "스태미나중 변동스태미나를 회복 할 수 있는 곳이지. \n" +
            "물론 레시피가 있어야 요리를 만들어서 먹을 수 있어.",false,true); //recipe 
        yield return new WaitUntil(() => tutorialCookingTouch);
        tutorialCookingTouch = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        tm.BlackPanelOn();
        tm.TutorialTargetButtonActivate(campDiaryquickButton);

        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 다음것은 진행해보자", false, true); //reipce xbutton
        yield return new WaitUntil(() => isquitbuttonClick);

        isquitbuttonClick = false;
        tm.TutorialTargetButtonActivate(bottomitemTagButton);

        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.9f, 0.2f, 0.9f, 0.5f, "아이템 확인", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheckFinish);
        tutorialBonableItemCheckFinish = false;
        tm.TutorialTargetButtonActivate(campBonableCheckButton);
        
        LongTouch(0.5f, 0.45f, 0.5f, 0.8f, "이것은 모닥불을 시간을 알려주고 아래 인벤토리에서 \n" +
            "태울 수 있는 아이템을 알려 주는 버튼이야 태울수 있는 아이템이 빨갛게 변화하게 되지.",false,false,false,true); //bonfire 
        yield return new WaitUntil(() => tutorialBonfirecheckButtonClick);
        tutorialBonfirecheckButtonClick = false;
        tm.BlackPanelOff();

        isWaitingTouch = true;
        LongTouchRect(0.7f, 0.15f, 0.6f, 0.5f, "아이템 확인", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheck);
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.5f, 0.7f, 0.5f, 0.8f, "여기는 잠을 자는곳이야 \n" +
            "너가 가지고 있는 모닥불시간에 비례해 \n" +
            "회복 할 수 있는 스태미나까지 회복시켜주지.", false, false, false, true); 
        yield return new WaitUntil(() =>tutorialSleepingTouch);
        tutorialSleepingTouch = false;
        yield return new WaitForSeconds(2f);
        tm.BlackPanelOn();

        tm.TutorialTargetButtonActivate(campDiaryquickButton);

        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 다음것은 진행해보자", false, true); //sleeping xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        tm.BlackPanelOff();

        SetActive(false);
        yield return new WaitForSeconds(3f);
        issleepingFinish = true;

        LongTouch(0.8f, 0.6f, 0.5f, 0.8f, "이곳은 남은 모닥불시간을 소비해서 \n" +
            "채집을 할 수있는곳이야."); //gathering
        yield return new WaitUntil(() =>tutorialGahteringingTouch);
        tutorialGahteringingTouch = false;
        SetActive(false);
        yield return new WaitForSeconds(2f);
        tm.BlackPanelOn();

        tm.TutorialTargetButtonActivate(campDiaryquickButton);

        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 튜토리얼이 종료됩니다.", false, true); //gathering xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        tm.BlackPanelOff();

        CampTutorialEnd();

        //인호 던전 튜토리얼 신으로 가야됨.
        //GameManager.Manager.LoadScene(GameScene.Dungeon);
        Vars.UserData.isTutorialCamp = false;

    }
    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }

    public void LongTouch(float widthmultifle, float heightmultifle,float boxwidthmultifle,float boxheightmultifle, string description,
        bool left=false, bool right=false, bool up =false, bool bottom =false)
    {
        SetActive(true, true, true);

        dialogBoxObj.left.SetActive(left);
        dialogBoxObj.right.SetActive(right);
        dialogBoxObj.up.SetActive(up);
        dialogBoxObj.down.SetActive(bottom);

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(200f, 200f);

        var boxPos = new Vector2(canvasRt.width * boxwidthmultifle - boxWidth / 2, canvasRt.height * boxheightmultifle);
        var scrPos = new Vector2(canvasRt.width * widthmultifle, canvasRt.height * heightmultifle);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = $"{description}";
    }

    public void LongTouchRect(float widthmultifle, float heightmultifle, float boxwidthmultifle, float boxheightmultifle, string description,
        bool left = false, bool right = false, bool up = false, bool bottom = false)
    {
        SetActive(true, true, true);

        dialogBoxObj.left.SetActive(left);
        dialogBoxObj.right.SetActive(right);
        dialogBoxObj.up.SetActive(up);
        dialogBoxObj.down.SetActive(bottom);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = new Vector2(600f, 200f);

        var boxPos = new Vector2(canvasRt.width * boxwidthmultifle - boxWidth / 2, canvasRt.height * boxheightmultifle);
        var scrPos = new Vector2(canvasRt.width * widthmultifle, canvasRt.height * heightmultifle);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = $"{description}";
    }

    public void CampTutorialEnd()
    {
        SetActive(false);
        dialogBoxObj.up.SetActive(false);
        Destroy(this);
    }
}
