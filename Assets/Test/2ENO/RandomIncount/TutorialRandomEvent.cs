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

    [Header("Æ÷Áö¼Ç Å¸°Ù")]
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
                TutorialStep != randomEventEndClose
                )
            {
                delay = 0f;
                TutorialStep++;
                Debug.Log(TutorialStep);
            }
        }
    }

    public IEnumerator CoRandomEventTutorial()
    {
        isRandomEventTutorial = true;

        yield return new WaitWhile(() => TutorialStep < 1);

        isRandomEventTutorial = false;
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

    }

    public void RandomEventSelectButtonsExplain()
    {

    }

    public void RandomEventSelectInfoExplain()
    {

    }

    public void RandomEventSelectedInfoExplain()
    {

    }

    public void RandomEventSelectedResultExplain()
    {

    }

    public void RandomEventSelectedRewardExplain()
    {

    }

    public void RandomEventFastClose()
    {

    }

    public void RandomEventRemainItemExplain()
    {

    }

    public void RandomEventReturnButton()
    {

    }

    public void RandomEventAllItemGet()
    {

    }

    public void RandomEventCloseBtn()
    {

    }

    public void RandomEventEnd()
    {

    }
}
