using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[DefaultExecutionOrder(10)]

public class MoveTutorial : MonoBehaviour
{
    public bool isMoveTutorial = false;
    public int TutorialStep { get; set; } = 0;

    public TutorialTool tutorialTool;
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

    public int CommandSucess { get; set; } = 0;

    [Header("������ Ÿ��")]
    public RectTransform time;
    public RectTransform lantern;
    private void Start()
    {
        dialogBox = tutorialTool.dialogBox;
        handIcon = tutorialTool.handIcon;
        blackout = tutorialTool.blackout;
        rect = tutorialTool.rect;
        circle = tutorialTool.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();

        dungeonCanvasRt = DungeonSystem.Instance.DungeonCanvas;

        //StartCoroutine(CoMoveTutorial());
    }

    private void Update()
    {
        if (isMoveTutorial)
        {
            delay += Time.deltaTime;
            if (GameManager.Manager.MultiTouch.TouchCount > 0 &&
                delay > 1f &&
                TutorialStep != tutorialStepMove
                )
            {
                delay = 0f;
                TutorialStep++;
                Debug.Log(TutorialStep);
            }
        }
    }

    public IEnumerator CoMoveTutorial()
    {
        isMoveTutorial = true;
        delay = 0f;
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

        LanternCostExplain();
        yield return new WaitWhile(() => TutorialStep < 6);

        MoveTutorialEnd();
        isMoveTutorial = false;
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
        blackout.sizeDelta = new Vector2(200f, 200f);

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        var scrPos = new Vector2(canvasRt.width * 0.75f, canvasRt.height * 0.5f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "�̵� ��� ����1";
    }

    public void LeftLongTouch()
    {
        SetActive(true, true, true);

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(200f, 200f);

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        var scrPos = new Vector2(canvasRt.width * 0.25f, canvasRt.height * 0.5f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "�̵� ��� ����2";
    }

    public void MoveTest()
    {
        SetActive(false, true);
        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "�̵� �ȳ� (������, ���� ������ ���� 1�ʰ� �̵��غ���)";
    }

    public void MoveTutorialEndExplain()
    {
        SetActive(true, true);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = Vector2.zero;

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition += blackout.anchoredPosition;
        blackout.anchoredPosition = Vector2.zero;

        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "�̵� ���� �Ϸ�!";
    }

    public void TimeCostExplain()
    {
        SetActive(true, true);
        target = time;
        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = target.sizeDelta;

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxWidth + arrowSize + target.rect.width / 2;

        dialogBoxObj.right.SetActive(true);

        var boxPos = new Vector2(pos.x - boxOffset, pos.y);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(pos.x, pos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = pos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "�ð� �ڽ�Ʈ ����";
    }

    public void LanternCostExplain()
    {
        SetActive(true, true);

        // Ÿ�� Ʈ�������� ��Ʈ��ġ�� sizeDelta ����!
        target = lantern;
        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = new Vector2(target.sizeDelta.x + 35f, target.sizeDelta.y * 2 + 6f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxHeight + arrowSize;
        var scrPos = new Vector2(pos.x - 15f, pos.y - target.sizeDelta.y / 2f);

        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBoxObj.right.SetActive(false);
        dialogBoxObj.up.SetActive(true);

        var boxPos = new Vector2(scrPos.x , scrPos.y - boxOffset);
        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "���� ���� ����";
    }

    public void MoveTutorialEnd()
    {
        SetActive(false);
        dialogBoxObj.up.SetActive(false);
        dialogBox.pivot = new Vector2(0f, 0.5f);
        Destroy(this);
    }
}
