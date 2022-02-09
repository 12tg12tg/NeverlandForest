using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MainRoomTutorial : MonoBehaviour
{
    public bool isMainRoomTutorial = false;
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

    private TutorialManager tutorialManager;

    public float delay;

    private readonly int mainRoomCampButton = 1;
    private readonly int mainRoomUseButton = 3;

    [Header("������ Ÿ��")]
    public RectTransform stamina;
    public RectTransform campBtn;
    public RectTransform campMenu;
    public RectTransform woodUseBtn;

    private void Start()
    {
        tutorialManager = GameManager.Manager.TutoManager;
        tutorialManager.mainTutorial.tutorialMainRoom = this;
        dialogBox = tutorialManager.dialogBox;
        handIcon = tutorialManager.handIcon;
        blackout = tutorialManager.blackout;
        rect = tutorialManager.rect;
        circle = tutorialManager.circle;

        dialogBoxObj = dialogBox.GetComponent<DialogBoxObject>();
        canvasRt = blackout.transform.parent.GetComponent<RectTransform>().rect;
        dialogText = dialogBox.GetComponentInChildren<TMP_Text>();

        dungeonCanvasRt = DungeonSystem.Instance.DungeonCanvas;
    }

    private void Update()
    {
        if (isMainRoomTutorial)
        {
            delay += Time.deltaTime;
            if (GameManager.Manager.MultiTouch.TouchCount > 0 &&
                delay > 1f &&
                TutorialStep != mainRoomCampButton &&
                TutorialStep != mainRoomUseButton
                )
            {
                delay = 0f;
                TutorialStep++;
                Debug.Log(TutorialStep);
            }
        }
    }

    public IEnumerator CoMainRoomTutorial()
    {
        isMainRoomTutorial = true;
        tutorialManager.BlackPanelOn();

        StaminaExplain();
        yield return new WaitWhile(() => TutorialStep < 1);

        CampButtonExplain();
        yield return new WaitWhile(() => TutorialStep < 2);

        CampNeedMaterialUseExplain();
        yield return new WaitWhile(() => TutorialStep < 3);

        UseButtonExplain();
        yield return new WaitWhile(() => TutorialStep < 4);

        MainRoomTutorialEnd();
        tutorialManager.BlackPanelOff();
        isMainRoomTutorial = false;
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

    public void StaminaExplain()
    {
        SetActive(true, true);
        target = stamina;

        blackout.GetComponent<Image>().sprite = rect;
        var size = target.sizeDelta;
        blackout.sizeDelta = size + new Vector2(10f, 10f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x, pos.y + boxOffset);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBoxObj.down.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "���׹̳� ���� ����";
    }

    public void CampButtonExplain()
    {
        SetActive(true, true, true);
        target = campBtn;

        var button = target.GetComponent<Button>();
        button = tutorialManager.TutorialTargetButtonActivate(button);
        ButtonAddOneUseStepPlus(button);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;
        var scrPos = new Vector2(pos.x, pos.y);

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x, pos.y + boxOffset);

        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBoxObj.down.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "ķ�� ���� ��ư ����";
    }

    public void CampNeedMaterialUseExplain()
    {
        SetActive(true, true);
        target = campMenu;

        blackout.GetComponent<Image>().sprite = rect;
        var size = target.sizeDelta;
        blackout.sizeDelta = size + new Vector2(5f, 5f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;

        var boxOffset = boxWidth + arrowSize;
        var boxPos = new Vector2(pos.x - boxOffset, pos.y);
        var scrPos = new Vector2(pos.x, pos.y);

        dialogBox.pivot = new Vector2(1f, 0.5f);
        dialogBoxObj.down.SetActive(false);
        dialogBoxObj.right.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "ķ�� �ʿ� ��� ����";
    }

    public void UseButtonExplain()
    {
        SetActive(true, true, true);
        target = woodUseBtn;

        var button = target.GetComponent<Button>();
        button = tutorialManager.TutorialTargetButtonActivate(button);
        ButtonAddOneUseStepPlus(button);

        blackout.GetComponent<Image>().sprite = rect;
        blackout.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);

        var uiCam = GameManager.Manager.CamManager.uiCamera;
        var pos = uiCam.WorldToViewportPoint(target.position);
        pos.x *= canvasRt.width;
        pos.y *= canvasRt.height;
        var scrPos = new Vector2(pos.x, pos.y);

        var boxOffset = boxHeight + arrowSize;
        var boxPos = new Vector2(pos.x, pos.y + boxOffset);

        dialogBox.pivot = new Vector2(0.5f, 0.5f);

        dialogBoxObj.right.SetActive(false);
        dialogBoxObj.down.SetActive(true);

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "��� ��� ��ư ����";
    }

    public void MainRoomTutorialEnd()
    {
        SetActive(false);
        dialogBoxObj.down.SetActive(false);
        dialogBox.pivot = new Vector2(0f, 0.5f);
        Destroy(this);
    }
}
