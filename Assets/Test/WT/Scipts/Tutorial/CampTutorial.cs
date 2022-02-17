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
        //���� Ʃ�丮��
        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        isTutorialFirst = true;
        LongTouch(0.25f,0.6f,0.5f,0.6f,"�̰��� ������ �Ҽ��ִ� �����̾�. \n" +
            "������, ��ġ��,������� ����� �ִ°�����.",true); //craft 
        yield return new WaitUntil(() => tutorialCraftTouch);
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.54f, 0.75f, 0.4f, 0.8f, "���۴뿡���� �������ִ� �������� �����־� \n" +
            "�׸��� �������� ������ �ش� ���ս��� �� �� ����.", false, true); //Craft iconbutton
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(craftIcon_button);
        yield return new WaitUntil(() => tutorialCraftIconClick);
        SetActive(false);
        camTM.BlackPanelOff();
        LongTouch(0.65f, 0.37f, 0.4f, 0.35f, "��ħ ������ �����������ϱ� ������ ������",
            false,true); //Craft startbutton
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(craft_startbutton);
        camTM.BlackPanelOff();
        yield return new WaitUntil(() => tutorialCraftStartbuttonClick);
        SetActive(false);
        camTM.BlackPanelOn();
        LongTouch(0.62f, 0.37f, 0.4f, 0.35f, "���� ���ư���",
          false,true); //Craft backbutton
        camTM.TutorialTargetButtonActivate(craft_moreMakebutton);
        camTM.BlackPanelOff();
        yield return new WaitUntil(() => tutorialCraftMoremakebuttonClick);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���",false,true); //Craft xbutton
        tutorialCraftTouch = false;
        yield return new WaitUntil(() => isquitbuttonClick);

        //�丮 Ʃ�丮��

        isquitbuttonClick = false;
        SetActive(false);
        iscraftFinish = true;
        yield return new WaitForSeconds(2.5f);
        camTM.BlackPanelOff();
        LongTouch(0.68f, 0.5f, 0.5f, 0.5f, "�̰��� �丮�� �� �� �ִ� ���̾�. \n" +
            "���¹̳��� �������¹̳��� ȸ�� �� �� �ִ� ������. \n" +
            "���� �����ǰ� �־�� �丮�� ���� ���� �� �־�.",false,true); //recipe 
        yield return new WaitUntil(() => tutorialCookingTouch);
        tutorialCookingTouch = false;
        yield return new WaitForSeconds(2.5f);
        SetActive(false);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(cook_recipeIconbutton);
        camTM.BlackPanelOff();
        LongTouch(0.54f, 0.75f, 0.4f, 0.8f, "�丮�ܿ����� �������ִ� �����Ǹ� �� �� �־�", false, true); //recipe iconclick
        yield return new WaitUntil(() => tutorialCook_recipeIconButton);
        SetActive(false);
        LongTouch(0.65f, 0.37f, 0.4f, 0.33f, "��ħ ������ �����������ϱ� �������̸� ������",
            false, true); //cook startbutton
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(cook_startbutton);
        camTM.BlackPanelOff();
        yield return new WaitUntil(() =>tutorialCook_startButton);
        SetActive(false);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(cook_moreCookbutton);
        camTM.BlackPanelOff();
        LongTouch(0.62f, 0.37f, 0.4f, 0.35f, "���� ���ư���",
          false, true); //Craft backbutton
        yield return new WaitUntil(() => tutorialMorecookingbuttonClick);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        camTM.BlackPanelOff();
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���", false, true); //reipce xbutton
        yield return new WaitUntil(() => isquitbuttonClick);

        //��ں� Ʃ�丮��

        isquitbuttonClick = false;
        camTM.TutorialTargetButtonActivate(bottomitemTagButton);
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.9f, 0.2f, 0.9f, 0.5f, "������ Ȯ��", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheckFinish);
        tutorialBonableItemCheckFinish = false;
        camTM.TutorialTargetButtonActivate(campBonableCheckButton);
        LongTouch(0.5f, 0.45f, 0.5f, 0.8f, "�̰��� ��ں��� �ð��� �˷��ְ� �Ʒ� �κ��丮���� \n" +
            "�¿� �� �ִ� �������� �˷� �ִ� ��ư�̾� �¿�� �ִ� �������� ������ ��ȭ�ϰ� ����.",false,false,false,true); //bonfire 
        yield return new WaitUntil(() => tutorialBonfirecheckButtonClick);
        tutorialBonfirecheckButtonClick = false;
        isWaitingTouch = true;
        LongTouchRect(0.7f, 0.15f, 0.6f, 0.5f, "������ Ȯ��", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheck);
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        camTM.BlackPanelOff();

        //���� Ʃ�丮��

        LongTouch(0.5f, 0.7f, 0.5f, 0.8f, "����� ���� �ڴ°��̾� \n" +
            "�ʰ� ������ �ִ� ��ںҽð��� ����� \n" +
            "ȸ�� �� �� �ִ� ���¹̳����� ȸ����������.", false, false, false, true); 
        yield return new WaitUntil(() =>tutorialSleepingTouch);
        tutorialSleepingTouch = false;
        yield return new WaitForSeconds(2.5f);
        SetActive(false);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(sleep_plusbutton);
        camTM.BlackPanelOff();
        LongTouch(0.7f, 0.66f, 0.5f, 0.7f, "��Ʈ������ ������ �ִ� ��ں� �ð���ŭ �޽��� ���� �� �־� \n" +
            "�ð��� ����غ���", false, true); //sleep plus click
        yield return new WaitUntil(() => tutorialSleepplusButtonClick);
        SetActive(false);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(sleep_startbutton);
        camTM.BlackPanelOff();
        LongTouch(0.63f, 0.4f, 0.65f, 0.7f, "�޽��� ������ ", false, false,false,true); //sleep start click
        yield return new WaitUntil(() => tutorialSleep_startButtonClick);
        yield return new WaitForSeconds(2f);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���", false, true); //sleeping xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        camTM.BlackPanelOff();
        isquitbuttonClick = false;
        SetActive(false);
        yield return new WaitForSeconds(3f);

        // ä�� Ʃ�丮��



        issleepingFinish = true;
        LongTouch(0.8f, 0.6f, 0.5f, 0.8f, "�̰��� ���� ��ںҽð��� �Һ��ؼ� \n" +
            "ä���� �� ���ִ°��̾�."); //gathering
        yield return new WaitUntil(() =>tutorialGahteringingTouch);
        tutorialGahteringingTouch = false;
        yield return new WaitForSeconds(2f);
        SetActive(false);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(gathering_plusbutton);
        camTM.BlackPanelOff();
        LongTouch(0.7f, 0.66f, 0.5f, 0.7f, "��Ʈ������ ������ �ִ� ��ں� �ð���ŭ ä���� �� �� �־� \n" +
            "�ð��� ����غ���", false, true); //gathering plus click
        yield return new WaitUntil(() => tutorialgathering_plusbuttonClick);
        SetActive(false);
        camTM.BlackPanelOn();
        camTM.TutorialTargetButtonActivate(gathering_startbutton);
        camTM.BlackPanelOff();
        LongTouch(0.63f, 0.4f, 0.65f, 0.7f, "ä���� �غ��� ", false, false, false, true); //gathering start click
        yield return new WaitUntil(() => tutorialgathering_startbuttonClick);
        yield return new WaitForSeconds(2f);
        camTM.TutorialTargetButtonActivate(campDiaryquickButton);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������Ʃ�丮���� ����˴ϴ�.", false, true); //gathering xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        camTM.BlackPanelOff();
        CampTutorialEnd();
        //��ȣ ���� Ʃ�丮�� ������ ���ߵ�.
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
