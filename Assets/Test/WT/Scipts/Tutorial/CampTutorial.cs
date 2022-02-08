using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CampTutorial : MonoBehaviour
{
    public int TutorialStep { get; set; } = 0;
    private RectTransform target;

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

    private readonly int tutorialStepMove = 2;
    private readonly int tutorialStepMoveEnd = 3;
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
            TutorialStep != tutorialStepMove &&
            TutorialStep != tutorialStepMoveEnd
            )
        {
            delay = 0f;
            TutorialStep++;
            Debug.Log(TutorialStep);
        }
    }
    public IEnumerator CoCampTutorial()
    {
        LongTouch();
        yield return new WaitWhile(() => TutorialStep < 1);

        yield return new WaitWhile(() => TutorialStep < 2);

        yield return new WaitWhile(() => TutorialStep < 3);

        yield return new WaitWhile(() => TutorialStep < 4);

        yield return new WaitWhile(() => TutorialStep < 5);

        yield return new WaitWhile(() => TutorialStep < 6);
    }
    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }

    public void LongTouch()
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

}
