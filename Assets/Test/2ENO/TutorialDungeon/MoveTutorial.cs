using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveTutorial : MonoBehaviour
{
    public int TutorialStep { get; set; } = 0;

    public RectTransform target;

    private GameObject dungeonCanvasRt;

    private Rect canvasRt;
    private RectTransform handIcon;
    private RectTransform dialogBox;
    private TMP_Text dialogText;
    private RectTransform blackout;
    private Button nextStepButton;

    private Sprite rect;
    private Sprite circle;

    private readonly float arrowSize = 50f;
    private readonly float boxWidth = 250f;
    private readonly float boxHeight = 100f;

    private DialogBoxObject dialogBoxObj;

    private float delay;

    private readonly int tutorialStepMove = 2;
    private readonly int tutorialStepEnd = 3;

    public int CommandSucess { get; set; } = 0;

    private void Awake()
    {
        var tm = GameManager.Manager.TutoManager;
        dialogBox = tm.dialogBox;
        handIcon = tm.handIcon;
        blackout = tm.blackout;
        rect = tm.rect;
        circle = tm.circle;
        nextStepButton = tm.nextStepButton;

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
            TutorialStep != tutorialStepMove &&
            TutorialStep != tutorialStepEnd
            )
        {
            delay = 0f;
            TutorialStep++;
            Debug.Log(TutorialStep);
        }
    }

    public IEnumerator CoMoveTutorial()
    {
        RightLongTouch();
        yield return new WaitWhile(() => TutorialStep < 1);

        LeftLongTouch();
        yield return new WaitWhile(() => TutorialStep < 2);

        MoveTest();
        yield return new WaitWhile(() => TutorialStep < 3);

        MoveTutorialEndExplain();
        yield return new WaitWhile(() => TutorialStep < 4);

        TimeCostExplain();
        yield return new WaitWhile(() => TutorialStep < 5);

        yield return new WaitWhile(() => TutorialStep < 6);

        MoveTutorialEnd();
    }

    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }

    public void RightLongTouch()
    {
        SetActive(true, true, true);

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(200f, canvasRt.height * 0.4f);

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        var scrPos = new Vector2(canvasRt.width * 0.75f, canvasRt.height * 0.5f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "이동 방법 설명1";
    }

    public void LeftLongTouch()
    {
        SetActive(true, true, true);

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(200f, canvasRt.height * 0.4f);

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        var scrPos = new Vector2(canvasRt.width * 0.25f, canvasRt.height * 0.5f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "이동 방법 설명2";
    }

    public void MoveTest()
    {
        SetActive(false, true);
        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "이동 안내 (오른쪽, 왼쪽 순서로 각각 1초간 이동해보기)";
    }

    public void MoveTutorialEndExplain()
    {
        nextStepButton.gameObject.SetActive(true);
        SetActive(true, true, true);

        var btnRect = nextStepButton.GetComponent<RectTransform>();
        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = btnRect.sizeDelta + new Vector2(10f, 10f);

        var btnPos = GameManager.Manager.CamManager.uiCamera.WorldToViewportPoint(btnRect.position);

        btnPos.x *= canvasRt.width;
        btnPos.y *= canvasRt.height;

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(btnPos.x, btnPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = btnPos;
        handIcon.anchoredPosition = btnPos;

        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "이동 연습 완료!";
        nextStepButton.GetComponentInChildren<TMP_Text>().text = "이동 완료";
    }

    public void TimeCostExplain()
    {
        SetActive(true, true, true);
        target = dungeonCanvasRt.transform.GetChild(0).GetComponent<RectTransform>();

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.rect.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxWidth + arrowSize;

        var boxPos = new Vector3(pos.x - boxOffset, pos.y);


    }

    public void LanternCostExplain()
    {

    }

    public void MoveTutorialEnd()
    {
        SetActive(false);
        Destroy(this);
    }
}
