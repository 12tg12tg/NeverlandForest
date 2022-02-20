using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CampTutorial : MonoBehaviour
{
    public CampTutorialManager camTM;

    public Button bottomitemTagButton;
    public Button campBonableCheckButton;
    public Button campDiaryquickButton;

    public Button craftIcon_button;
    public Button craft_startbutton;
    public Button craft_moreMakebutton;

    public Button cook_recipeIconbutton;
    public Button cook_startbutton;
    public Button cook_moreCookbutton;

    public Button sleep_plusbutton;
    public Button sleep_startbutton;

    public Button gathering_plusbutton;
    public Button gathering_startbutton;

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
    private bool tutorialCraftTouch;
    public bool TutorialCraftTouch
    {
        get => tutorialCraftTouch;
        set { tutorialCraftTouch = value; }
    }
    private bool tutorialCraftIconClick;
    public bool TutorialCraftIconClick
    {
        get => tutorialCraftIconClick;
        set { tutorialCraftIconClick = value; }
    }

    private bool tutorialCraftStartbuttonClick;
    public bool TutorialCraftStartbuttonClick
    {
        get => tutorialCraftStartbuttonClick;
        set { tutorialCraftStartbuttonClick = value; }
    }
    private bool tutorialCraftMoremakebuttonClick;
    public bool TutorialCraftMoremakebuttonClick
    {
        get => tutorialCraftMoremakebuttonClick;
        set { tutorialCraftMoremakebuttonClick = value; }
    }

    private bool tutorialCook_recipeIconButton;
    public bool TutorialCook_recipeIconButton
    {
        get => tutorialCook_recipeIconButton;
        set { tutorialCook_recipeIconButton = value; }
    }

    private bool tutorialCook_startButton;
    public bool TutorialCook_startButton
    {
        get => tutorialCook_startButton;
        set { tutorialCook_startButton = value; }
    }

    private bool tutorialMorecookingbuttonClick;
    public bool TutorialMorecookingbuttonClick
    {
        get => tutorialMorecookingbuttonClick;
        set { tutorialMorecookingbuttonClick = value; }
    }

    private bool tutorialSleepplusButtonClick;
    public bool TutorialSleepplusButtonClick
    {
        get => tutorialSleepplusButtonClick;
        set { tutorialSleepplusButtonClick=value; }
    }
    private bool tutorialSleep_startButtonClick;
    public bool TutorialSleep_startButtonClick
    {
        get => tutorialSleep_startButtonClick;
        set { tutorialSleep_startButtonClick = value; }
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

    private bool tutorialgathering_plusbuttonClick;
    public bool Tutorialgathering_plusbuttonClick
    {
        get => tutorialgathering_plusbuttonClick;
            set { tutorialgathering_plusbuttonClick = value; }
    }

    private bool tutorialgathering_startbuttonClick;
    public bool Tutorialgathering_startbuttonClick
    {
        get => tutorialgathering_startbuttonClick;
        set { tutorialgathering_startbuttonClick = value; }
    }

    private void Awake()
    {
        dialogBox = camTM.dialogBox;
        handIcon = camTM.handIcon;
        blackout = camTM.blackout;
        rect = camTM.rect;
        circle = camTM.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();
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

    public IEnumerator CoCampTutorial()
    {
        //조합 튜토리얼
        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        isTutorialFirst = true;
        LongTouch(0.25f,0.6f,0.5f,0.6f,"이것은 제작을 할수있는 공간이야. \n" +
            "도구나, 설치물,물약류를 만들수 있는곳이지.",true); //craft 
        yield return new WaitUntil(() => tutorialCraftTouch);
        SetActive(false);
        yield return new WaitForSeconds(2f);
        camTM.BlackPanelOn();
        LongTouch(0.54f, 0.75f, 0.4f, 0.8f, "제작대에서는 가지고있는 제조법을 볼수있어 \n" +
            "그리고 제조법을 누르면 해당 조합식을 알 수 있지.", false, true); //Craft iconbutton
        camTM.TutorialTargetButtonActivate(craftIcon_button);
        yield return new WaitUntil(() => tutorialCraftIconClick);
        LongTouch(0.63f, 0.37f, 0.4f, 0.35f, "마침 씨앗을 가지고있으니깐 오일을 만들어보자",
            false,true); //Craft startbutton
        camTM.TutorialTargetButtonActivate(craft_startbutton);
        yield return new WaitUntil(() => tutorialCraftStartbuttonClick);
        LongTouch(0.64f, 0.37f, 0.4f, 0.35f, "이제 돌아갈까",
          false,true); //Craft backbutton
        camTM.TutorialTargetButtonActivate(craft_moreMakebutton);
        yield return new WaitUntil(() => tutorialCraftMoremakebuttonClick);
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 다음것은 진행해보자",false,true); //Craft xbutton
        tutorialCraftTouch = false;
        camTM.BlackPanelOff();
        yield return new WaitUntil(() => isquitbuttonClick);

        //요리 튜토리얼
        isquitbuttonClick = false;
        iscraftFinish = true;
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.68f, 0.5f, 0.5f, 0.5f, "이것은 요리를 할 수 있는 솥이야. \n" +
            "스태미나중 변동스태미나를 회복 할 수 있는 곳이지. \n" +
            "물론 레시피가 있어야 요리를 만들어서 먹을 수 있어.",false,true); //recipe 
        yield return new WaitUntil(() => tutorialCookingTouch);
        tutorialCookingTouch = false;
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(cook_recipeIconbutton);
        LongTouch(0.54f, 0.75f, 0.4f, 0.8f, "요리솥에서는 가지고있는 레시피를 볼 수 있어", false, true); //recipe iconclick
        yield return new WaitUntil(() => tutorialCook_recipeIconButton);
        LongTouch(0.62f, 0.35f, 0.4f, 0.33f, "마침 버섯을 가지고있으니깐 버섯구이를 만들어보자",
            false, true); //cook startbutton
        camTM.TutorialTargetButtonActivate(cook_startbutton);
        yield return new WaitUntil(() =>tutorialCook_startButton);
        camTM.TutorialTargetButtonActivate(cook_moreCookbutton);
        LongTouch(0.64f, 0.4f, 0.4f, 0.35f, "이제 돌아갈까",
          false, true); //Craft backbutton
        yield return new WaitUntil(() => tutorialMorecookingbuttonClick);
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        camTM.BlackPanelOff();
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 다음것은 진행해보자", false, true); //reipce xbutton
        yield return new WaitUntil(() => isquitbuttonClick);

        //모닥불 튜토리얼
        isquitbuttonClick = false;
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(bottomitemTagButton);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.9f, 0.2f, 0.9f, 0.5f, "아이템 확인", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheckFinish);
        tutorialBonableItemCheckFinish = false;
        camTM.TutorialTargetButtonActivate(campBonableCheckButton);
        LongTouch(0.5f, 0.45f, 0.5f, 0.8f, "이것은 모닥불을 시간을 알려주고 아래 인벤토리에서 \n" +
            "태울 수 있는 아이템을 알려 주는 버튼이야 태울수 있는 아이템이 빨갛게 변화하게 되지.",false,false,false,true); //bonfire 
        yield return new WaitUntil(() => tutorialBonfirecheckButtonClick);
        tutorialBonfirecheckButtonClick = false;
        isWaitingTouch = true;
        LongTouchRect(0.7f, 0.15f, 0.6f, 0.5f, "아이템 확인", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheck);
        yield return new WaitForSeconds(2.5f);
        camTM.BlackPanelOff();

        //수면 튜토리얼
        LongTouch(0.5f, 0.7f, 0.5f, 0.8f, "여기는 잠을 자는곳이야 \n" +
            "너가 가지고 있는 모닥불시간에 비례해 \n" +
            "회복 할 수 있는 스태미나까지 회복시켜주지.", false, false, false, true); 
        yield return new WaitUntil(() =>tutorialSleepingTouch);
        tutorialSleepingTouch = false;
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(sleep_plusbutton);
        LongTouch(0.7f, 0.66f, 0.5f, 0.7f, "텐트에서는 가지고 있는 모닥불 시간만큼 휴식을 취할 수 있어 \n" +
            "시간을 사용해보자", false, true); //sleep plus click
        yield return new WaitUntil(() => tutorialSleepplusButtonClick);
        camTM.TutorialTargetButtonActivate(sleep_startbutton);
        LongTouch(0.63f, 0.4f, 0.65f, 0.7f, "휴식을 취하자 ", false, false,false,true); //sleep start click
        yield return new WaitUntil(() => tutorialSleep_startButtonClick);
        yield return new WaitForSeconds(2f);
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르고 다음것은 진행해보자", false, true); //sleeping xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        camTM.BlackPanelOff();
        isquitbuttonClick = false;
        yield return new WaitForSeconds(3f);

        // 채집 튜토리얼
        issleepingFinish = true;
        LongTouch(0.8f, 0.6f, 0.5f, 0.8f, "이곳은 남은 모닥불시간을 소비해서 \n" +
            "채집을 할 수있는곳이야."); //gathering
        yield return new WaitUntil(() =>tutorialGahteringingTouch);
        tutorialGahteringingTouch = false;
        SetActive(false);
        yield return new WaitForSeconds(2f);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(gathering_plusbutton);
        LongTouch(0.7f, 0.66f, 0.5f, 0.7f, "텐트에서는 가지고 있는 모닥불 시간만큼 채집을 할 수 있어 \n" +
            "시간을 사용해보자", false, true); //gathering plus click
        yield return new WaitUntil(() => tutorialgathering_plusbuttonClick);
        camTM.TutorialTargetButtonActivate(gathering_startbutton);
        LongTouch(0.63f, 0.4f, 0.65f, 0.7f, "채집을 해보자 ", false, false, false, true); //gathering start click
        yield return new WaitUntil(() => tutorialgathering_startbuttonClick);
        yield return new WaitForSeconds(2f);
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x버튼을 누르면튜토리얼이 종료됩니다.", false, true); //gathering xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        camTM.BlackPanelOff();
        CampTutorialEnd();
        //인호 던전 튜토리얼 신으로 가야됨.
        CampManager.curinitState = CampManager.CampinitState.None;
        GameManager.Manager.LoadScene(GameScene.TutorialDungeon);
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
