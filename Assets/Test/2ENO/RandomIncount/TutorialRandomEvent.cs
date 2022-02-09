using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TutorialRandomEvent : MonoBehaviour
{
    public bool isRandomEventTutorial = false;
    public int TutorialStep { get; set; } = 0;

    private RectTransform target;
    private GameObject eventObject;

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

    private TutorialManager tutorialManager;

    public float delay;

    private readonly int randomEventSelectButton = 2;
    private readonly int randomEventFastClose = 6;
    private readonly int randomEventReturnBtn = 8;
    private readonly int randomEventAllItemGet = 9;
    private readonly int randomEventEndClose = 10;
    private readonly int randomEventEndClose2 = 11;

    [Header("포지션 타겟")]
    public RectTransform randEventDesc;
    public RectTransform randAllSelectInfo;
    public RectTransform selectButton1;
    public RectTransform selectedName;
    public RectTransform selectedResult;
    public RectTransform selectedReward;
    public RectTransform closeButtton;
    public RectTransform remainWindow;
    public RectTransform returnButton;
    public RectTransform allget;

    private void Start()
    {
        tutorialManager = GameManager.Manager.tm;
        dialogBox = tutorialManager.dialogBox;
        handIcon = tutorialManager.handIcon;
        blackout = tutorialManager.blackout;
        rect = tutorialManager.rect;
        circle = tutorialManager.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();

        //TODO : 임시용
        ConsumeManager.CostDataReset();
    }

    private void Update()
    {
        if (isRandomEventTutorial)
        {
            delay += Time.deltaTime;
            if (GameManager.Manager.MultiTouch.TouchCount > 0 &&
                delay > 1f &&
                TutorialStep != randomEventSelectButton &&
                TutorialStep != randomEventFastClose &&
                TutorialStep != randomEventReturnBtn &&
                TutorialStep != randomEventAllItemGet &&
                TutorialStep != randomEventEndClose &&
                TutorialStep != randomEventEndClose2
                )
            {
                delay = 0f;
                TutorialStep++;
                Debug.Log(TutorialStep);
            }
        }
    }

    public void StartRandomEventTutorial()
    {
        StartCoroutine(CoRandomEventTutorial());
    }

    public IEnumerator CoRandomEventTutorial()
    {
        tutorialManager.BlackPanelOn();
        isRandomEventTutorial = true;

        RandomEventDescExplain();
        yield return new WaitWhile(() => TutorialStep < 1);

        RandomEventSelectButtonsExplain();
        yield return new WaitWhile(() => TutorialStep < 2);

        RandomEventSelectInfoExplain();
        yield return new WaitWhile(() => TutorialStep < 3);

        yield return new WaitForSeconds(0.05f);

        RandomEventSelectedNameExplain();
        yield return new WaitWhile(() => TutorialStep < 4);

        RandomEventSelectedResultExplain();
        yield return new WaitWhile(() => TutorialStep < 5);

        RandomEventSelectedRewardExplain();
        yield return new WaitWhile(() => TutorialStep < 6);

        RandomEventFastClose();
        yield return new WaitWhile(() => TutorialStep < 7);

        RandomEventRemainItemExplain();
        yield return new WaitWhile(() => TutorialStep < 8);

        RandomEventReturnButton();
        yield return new WaitWhile(() => TutorialStep < 9);

        RandomEventAllItemGet();
        yield return new WaitWhile(() => TutorialStep < 10);

        RandomEventCloseBtn();
        yield return new WaitWhile(() => TutorialStep < 11);

        RandomEventEnd();
        isRandomEventTutorial = false;
        tutorialManager.BlackPanelOff();
    }

    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }
    public void ButtonAddOneUseStepPlus(Button button)
    {
        UnityAction action = () => { TutorialStep++; delay = 0f; };
        button.onClick.AddListener(action);
        button.onClick.AddListener(() => button.onClick.RemoveListener(action));
    }

    
    public void RandomEventDescExplain()
    {
        SetActive(true, true);
        target = randEventDesc;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(20f, 20f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x + boxOffset, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(0f, 0.5f);
        dialogBoxObj.left.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "랜덤이벤트 내용 설명";
    }

    public void RandomEventSelectButtonsExplain()
    {
        SetActive(true, true);
        target = randAllSelectInfo;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(20f, 20f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(1f, 0.5f);
        dialogBoxObj.left.SetActive(false);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "랜덤이벤트 선택지 버튼들 및 내용 설명";
    }

    // DirectTouch
    public void RandomEventSelectInfoExplain()
    {
        SetActive(true, true, true);
        target = selectButton1;
        var targetButton = target.GetComponent<Button>();

        targetButton = tutorialManager.TutorialTargetButtonActivate(targetButton);
        ButtonAddOneUseStepPlus(targetButton);

        UnityAction<int> tempAction = RandomEventManager.Instance.tutorialEvent.SelectFeedBack;
        targetButton.onClick.AddListener(() => tempAction.Invoke(2));

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = pos;

        dialogBox.pivot = new Vector2(1f, 0.5f); 
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "랜덤이벤트 선택지 버튼 터치 설명";
    }

    public void RandomEventSelectedNameExplain()
    {
        SetActive(true, true);
        target = selectedName;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x + boxOffset, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(0f, 0.5f);
        dialogBoxObj.right.SetActive(false);
        dialogBoxObj.left.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "선택이벤트 이름 설명";
    }

    public void RandomEventSelectedResultExplain()
    {
        SetActive(true, true);
        target = selectedResult;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(20f, 20f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x + boxOffset, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(0f, 0.5f);
        dialogBoxObj.left.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "선택이벤트 결과 내용 설명";
    }

    public void RandomEventSelectedRewardExplain()
    {
        SetActive(true, true);
        target = selectedReward;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 0f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(1f, 0.5f);
        dialogBoxObj.left.SetActive(false);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "랜덤이벤트 보상 설명";
    }
    // DirectTouch
    public void RandomEventFastClose()
    {
        SetActive(true, true, true);
        target = closeButtton;
        var targetButton = target.GetComponent<Button>();

        targetButton = tutorialManager.TutorialTargetButtonActivate(targetButton);
        ButtonAddOneUseStepPlus(targetButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = pos;

        dialogBox.pivot = new Vector2(0f, 0.5f);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "보상 받지않고 닫기 버튼";
    }

    public void RandomEventRemainItemExplain()
    {
        SetActive(true, true);
        target = remainWindow;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset - blackout.sizeDelta.x / 2, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(0f, 0.5f);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "획득하지 않은 보상 창 설명";
    }
    // DirectTouch
    public void RandomEventReturnButton()
    {
        SetActive(true, true, true);
        target = returnButton;
        var targetButton = target.GetComponent<Button>();

        targetButton = tutorialManager.TutorialTargetButtonActivate(targetButton);
        ButtonAddOneUseStepPlus(targetButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = pos;

        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "되돌아가기 버튼 설명";
    }
    // DirectTouch
    public void RandomEventAllItemGet()
    {
        SetActive(true, true, true);
        target = allget;
        var targetButton = target.GetComponent<Button>();

        targetButton = tutorialManager.TutorialTargetButtonActivate(targetButton);
        ButtonAddOneUseStepPlus(targetButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = pos;

        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "모두 받기 버튼";
    }
    // DirectTouch
    public void RandomEventCloseBtn()
    {
        SetActive(true, false, true);
        target = closeButtton;
        var targetButton = target.GetComponent<Button>();

        targetButton = tutorialManager.TutorialTargetButtonActivate(targetButton);
        ButtonAddOneUseStepPlus(targetButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var scrPos = pos;

        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
    }

    public void RandomEventEnd()
    {
        SetActive(false);
        dialogBoxObj.right.SetActive(false);
        dialogBox.pivot = new Vector2(0f, 0.5f);
        Destroy(this);
    }
}
