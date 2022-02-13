using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
[DefaultExecutionOrder(11)]
public class GatheringTutorial : MonoBehaviour
{
    public bool isGatheringTutorial = false;
    public int TutorialStep { get; set; } = 0;

    public TutorialTool tutorialTool;
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


    public float delay;

    private readonly int gatheringTouchStep = 0;
    private readonly int gatheringMoveStep = 1;
    private readonly int gatheringStartStep = 2;
    private readonly int gatheringToolUseStep = 5;
    private readonly int gatheringToolSelectItem = 7;
    private readonly int gatheringToolGetItem = 8;
    private readonly int gatheringToolCloseBtn = 10;

    [Header("포지션 타겟")]
    public RectTransform rewardUp;
    public RectTransform rewardItem;
    public RectTransform getItem;
    public RectTransform itemList;
    public RectTransform closeBtn;

    private void Awake()
    {
        dialogBox = tutorialTool.dialogBox;
        handIcon = tutorialTool.handIcon;
        blackout = tutorialTool.blackout;
        rect = tutorialTool.rect;
        circle = tutorialTool.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        delay += Time.deltaTime;
        if (GameManager.Manager.MultiTouch.TouchCount > 0 &&
            delay > 1f &&
            TutorialStep != gatheringTouchStep &&
            TutorialStep != gatheringMoveStep &&
            TutorialStep != gatheringStartStep &&
            TutorialStep != gatheringToolUseStep &&
            TutorialStep != gatheringToolSelectItem &&
            TutorialStep != gatheringToolGetItem &&
            TutorialStep != gatheringToolCloseBtn
            )
        {
            delay = 0f;
            TutorialStep++;
            Debug.Log(TutorialStep);
        }
    }

    public IEnumerator CoGatheringTutorial()
    {
        isGatheringTutorial = true;
        delay = 0f;
        GatheringTouch();
        yield return new WaitWhile(() => TutorialStep < 1);

        SetActive(false);
        var gatheringSystem = DungeonSystem.Instance.gatheringSystem;
        gatheringSystem.GoGatheringObject(eventObject.transform.position);

        yield return new WaitWhile(() => TutorialStep < 2);

        GatheringStartExplain();
        yield return new WaitWhile(() => TutorialStep < 3);

        yield return new WaitForSeconds(0.05f);

        GatheringToolUse();
        yield return new WaitWhile(() => TutorialStep < 4);

        //GatheringToolNoUse();
        TutorialStep++;
        yield return new WaitWhile(() => TutorialStep < 5);

        GatheringToolUseStart();
        yield return new WaitWhile(() => TutorialStep < 6);

        GatheringReWardExplain();
        yield return new WaitWhile(() => TutorialStep < 7);

        GatheringReWardItemExplain();
        yield return new WaitWhile(() => TutorialStep < 8);

        GatheringGetItemExplain();
        yield return new WaitWhile(() => TutorialStep < 9);

        GatheringItemListExplain();
        yield return new WaitWhile(() => TutorialStep < 10);

        GatheringEndTouch();
        yield return new WaitWhile(() => TutorialStep < 11);

        GatheringTutorialEnd();
        isGatheringTutorial = false;
        GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial(false);
    }

    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }
    // 잘 만든건지는 모르겠...
    public void ButtonAddOneUseStepPlus(Button button)
    {
        UnityAction action = () => { TutorialStep++; delay = 0f; };
        button.onClick.AddListener(action);
        button.onClick.AddListener(() => button.onClick.RemoveListener(action));
    }

    public void GatheringTouch()
    {
        SetActive(true, true, true);
        eventObject = DungeonSystem.Instance.eventObjectGenerate.eventObjInstanceList[0];

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(100f, 100f);

        var pos = Camera.main.WorldToViewportPoint(eventObject.transform.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxWidth + arrowSize;
        var boxPos = new Vector2(pos.x - boxOffset, pos.y);

        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(pos.x, pos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = pos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "던전 내 채집 할 수 있는 요소가 있어. 터치 해 보자";
    }

    public void GatheringStartExplain()
    {
        SetActive(true, true, true);

        var yesButton = DungeonSystem.Instance.DungeonCanvas.transform.GetChild(2).GetComponentInChildren<Button>();

        tutorialTool.BlackPanelOn();
        yesButton = tutorialTool.TutorialTargetButtonActivate(yesButton);

        ButtonAddOneUseStepPlus(yesButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = yesButton.GetComponentInChildren<RectTransform>().sizeDelta + new Vector2(10f, 10f);

        target = yesButton.gameObject.GetComponent<RectTransform>();

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x - boxWidth / 2, pos.y - boxOffset);

        dialogBoxObj.right.SetActive(false);
        dialogBoxObj.up.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(pos.x, pos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = pos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "채집은 맨손 혹은 도구를 사용 할 수 있어";
    }
    // + 165 - 125
    public void GatheringToolUse()
    {
        SetActive(true, true);
        target = DungeonSystem.Instance.gatheringSystem.handbutton.GetComponent<RectTransform>();

        blackout.GetComponent<Image>().sprite = rect;
        var size = target.GetComponentInChildren<RectTransform>().sizeDelta;
        blackout.sizeDelta = size + new Vector2(10f, 10f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x - boxWidth / 2, pos.y + boxOffset);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBoxObj.up.SetActive(false);
        dialogBoxObj.down.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "채집은 맨손을 사용하거나";
    }

    //public void GatheringToolNoUse()
    //{
    //    SetActive(true, true);
    //    target = DungeonSystem.Instance.gatheringSystem.handbutton.GetComponent<RectTransform>();
    //    var handButton = target.GetComponent<Button>();

    //    blackout.GetComponent<Image>().sprite = rect;
    //    blackout.sizeDelta = handButton.GetComponentInChildren<RectTransform>().sizeDelta + new Vector2(10f, 10f);

    //    var uiCam = GameManager.Manager.cm.uiCamera;
    //    var pos = uiCam.WorldToViewportPoint(target.position);
    //    pos.x *= canvasRt.width;
    //    pos.y *= canvasRt.height;

    //    var boxOffset = boxHeight + arrowSize;
    //    var boxPos = new Vector2(pos.x, pos.y + boxOffset);

    //    dialogBoxObj.down.SetActive(true);

    //    var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
    //    blackBg.anchoredPosition -= new Vector2(pos.x, pos.y) - blackout.anchoredPosition;
    //    blackout.anchoredPosition = pos;
    //    dialogBox.anchoredPosition = boxPos;
    //    dialogText.text = "맨손 채집 설명";
    //}

    public void GatheringToolUseStart()
    {
        SetActive(true, true, true);
        target = DungeonSystem.Instance.gatheringSystem.toolbutton.GetComponent<RectTransform>();
        var toolButton = target.GetComponent<Button>();

        toolButton = tutorialTool.TutorialTargetButtonActivate(toolButton);
        ButtonAddOneUseStepPlus(toolButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x - boxWidth / 2, pos.y + boxOffset);

        dialogBoxObj.up.SetActive(false);
        dialogBoxObj.down.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(pos.x, pos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = pos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "더 큰 보상을 위해 도구를 사용 할 수 있어";
    }

    public void GatheringReWardExplain()
    {
        SetActive(true, true);

        target = rewardUp;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta;

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(rewardUp.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(canvasRt.width * 0.7f - boxOffset - 280f, canvasRt.height * 0.8f);
        var scrPos = new Vector2(pos.x + 50f, pos.y - 50f);
        dialogBoxObj.down.SetActive(false);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "채집 대상에 따라 받을 수 있는 보상이 달라";
    }

    public void GatheringReWardItemExplain()
    {
        SetActive(true, true, true);
        target = rewardItem;
        var itemButton = target.GetComponentInChildren<Button>();

        itemButton = tutorialTool.TutorialTargetButtonActivate(itemButton);
        ButtonAddOneUseStepPlus(itemButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(3f, 3f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset - blackout.sizeDelta.x / 2, pos.y);
        var scrPos = pos;

        dialogBoxObj.right.SetActive(true);

        dialogBox.pivot = new Vector2(0f, 0.5f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "보상 아이템은 선택하거나 모두 받기를 할 수 있어";
    }

    public void GatheringGetItemExplain()
    {
        SetActive(true, true, true);
        target = getItem;
        var getButton = target.GetComponent<Button>();

        getButton = tutorialTool.TutorialTargetButtonActivate(getButton);
        ButtonAddOneUseStepPlus(getButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = new Vector2(170f, 37f);

        var boxOffset = boxWidth + arrowSize;
        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxOffset - blackout.sizeDelta.x / 2, pos.y);
        var scrPos = pos;

        dialogBoxObj.right.SetActive(true);
        dialogBox.pivot = new Vector2(0f, 0.5f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "받기 버튼을 눌러보자";
    }

    public void GatheringItemListExplain()
    {
        SetActive(true, true);
        target = itemList;

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f,10f + target.sizeDelta.y);

        var boxOffset = boxHeight + arrowSize;
        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x, pos.y + boxOffset);
        var scrPos = new Vector2(pos.x, pos.y - target.sizeDelta.y / 2);

        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBoxObj.right.SetActive(false);
        dialogBoxObj.down.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "아래의 아이템 리스트에 옮겨진 것을 확인 할 수 있어";
    }

    public void GatheringEndTouch()
    {
        SetActive(true, true, true);
        target = closeBtn;
        var closeButton = target.GetComponentInChildren<Button>();

        closeButton = tutorialTool.TutorialTargetButtonActivate(closeButton);
        ButtonAddOneUseStepPlus(closeButton);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var boxOffset = boxHeight + arrowSize;
        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxPos = new Vector2(pos.x - boxWidth/2, pos.y - boxOffset);
        var scrPos = pos;

        dialogBox.pivot = new Vector2(0f, 0.5f);
        dialogBoxObj.down.SetActive(false);
        dialogBoxObj.up.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "닫기 버튼으로 보상 창을 나갈 수 있어";
    }

    public void GatheringTutorialEnd()
    {
        tutorialTool.BlackPanelOff();
        SetActive(false);
        dialogBoxObj.up.SetActive(false);
        Destroy(this);
    }
}
