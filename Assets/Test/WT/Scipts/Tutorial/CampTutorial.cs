using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CampTutorial : MonoBehaviour
{
    public int TutorialStep { get; set; } = 0;
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
    private bool  tutorialCraftTouch;
    public bool TutorialCraftTouch
    {
        get => tutorialCraftTouch;
        set { tutorialCraftTouch = value; }
    }
    private bool  tutorialCookingTouch;
    public bool TutorialCookingTouch
    {
        get => tutorialCookingTouch;
        set { tutorialCookingTouch = value; }
    }
    private bool tutorialBonfirecheckButtonClick;
    public bool TutorialBonfirecheckButtonClick
    {
        get => tutorialBonfirecheckButtonClick;
        set { tutorialBonfirecheckButtonClick = value; }
    }
    private bool  tutorialSleepingTouch;
    public bool TutorialSleepingTouch
    {
        get => tutorialSleepingTouch;
        set { tutorialSleepingTouch = value; }
    }
    private bool  tutorialGahteringingTouch;
    public bool TutorialGahteringingTouch
    {
        get => tutorialGahteringingTouch;
        set { tutorialGahteringingTouch = value; }
    }
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
    }

    public IEnumerator CoCampTutorial()
    {
        GameManager.Manager.State = GameState.Tutorial;
        LongTouch(0.25f,0.6f,0.5f,0.8f,"제조"); //craft 
        yield return new WaitUntil(() => tutorialCraftTouch);
        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.5f, 0.8f, "제조x버튼"); //Craft xbutton
        tutorialCraftTouch = false;
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.68f, 0.5f, 0.5f, 0.8f, "요리"); //recipe 
        yield return new WaitUntil(() => tutorialCookingTouch);
        tutorialCookingTouch = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.5f, 0.8f, "레시피x버튼"); //reipce xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.5f, 0.45f, 0.5f, 0.8f, "모닥불"); //bonfire 
        yield return new WaitUntil(() => tutorialBonfirecheckButtonClick);
        tutorialBonfirecheckButtonClick = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.5f, 0.7f, 0.5f, 0.8f, "수면"); //bonfire 
        yield return new WaitUntil(() =>tutorialSleepingTouch);
        tutorialSleepingTouch = false;
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.5f, 0.8f, "수면x버튼"); //sleeping xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.8f, 0.6f, 0.5f, 0.8f, "채집"); //gathering
        yield return new WaitUntil(() =>tutorialGahteringingTouch);
        tutorialGahteringingTouch = false;
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.5f, 0.8f, "채집x버튼"); //gathering xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        CampTutorialEnd();
    }
    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }

    public void LongTouch(float widthmultifle, float heightmultifle,float boxwidthmultifle,float boxheightmultifle, string description)
    {
        SetActive(true, true, true);

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
    public void CampTutorialEnd()
    {
        SetActive(false);
        dialogBoxObj.up.SetActive(false);
        Destroy(this);
    }
}
