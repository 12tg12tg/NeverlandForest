using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GatheringTutorial : MonoBehaviour
{
    public int TutorialStep { get; set; } = 0;

    private RectTransform target;
    private GameObject eventObject;

    private GameObject dungeonCanvasRt;

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
    private readonly int gatheringToolUseStep = 3;
    private readonly int gatheringToolNoUseStep = 4;

    private void Awake()
    {
        var tm = GameManager.Manager.tm;
        dialogBox = tm.dialogBox;
        handIcon = tm.handIcon;
        blackout = tm.blackout;
        rect = tm.rect;
        circle = tm.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();

        dungeonCanvasRt = DungeonSystem.Instance.DungeonCanvas;
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
            TutorialStep != gatheringToolNoUseStep
            )
        {
            delay = 0f;
            TutorialStep++;
            Debug.Log(TutorialStep);
        }
    }

    public IEnumerator CoGatheringTutorial()
    {
        GatheringTouch();
        yield return new WaitWhile(() => TutorialStep < 1);

        SetActive(false);
        var gatheringSystem = DungeonSystem.Instance.gatheringSystem;
        gatheringSystem.GoGatheringObject(eventObject.transform.position);

        yield return new WaitWhile(() => TutorialStep < 2);

        GatheringStartExplain();
        yield return new WaitWhile(() => TutorialStep < 3);
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
        UnityAction action = () => { TutorialStep++; };
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
        dialogText.text = "채집 설명";
    }



    public void GatheringStartExplain()
    {
        SetActive(true, true, true);
        var yesButton = DungeonSystem.Instance.DungeonCanvas.transform.GetChild(2).GetComponentInChildren<Button>();
        var noButton = DungeonSystem.Instance.DungeonCanvas.transform.GetChild(2).GetComponentsInChildren<Button>()[1];
        noButton.interactable = false;

        ButtonAddOneUseStepPlus(yesButton);

        target = yesButton.gameObject.GetComponent<RectTransform>();

        var uiCam = GameManager.Manager.cm.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x, pos.y - boxOffset);

        dialogBoxObj.right.SetActive(false);
        dialogBoxObj.up.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(pos.x, pos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = pos;
        handIcon.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "채집 시작버튼 설명";
    }

    public void GatheringToolUse()
    {
        SetActive(true, true, true);
        target = DungeonSystem.Instance.gatheringSystem.toolbutton.GetComponent<RectTransform>();
        var toolButton = target.GetComponent<Button>();
        ButtonAddOneUseStepPlus(toolButton);


    }

    public void GatheringToolNoUse()
    {
        SetActive(true, true, true);
    }
}
