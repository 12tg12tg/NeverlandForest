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

    public float delay;

    private readonly int tutorialCraftTouch = 0;
    private readonly int tutorialCookingTouch = 2;
    private readonly int tutorialSleepingTouch = 5;
    private readonly int tutorialGahteringingTouch = 7;

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

    private void Update()
    {
        delay += Time.deltaTime;
        if (GameManager.Manager.MultiTouch.TouchCount > 0 &&
            delay >1f &&
            TutorialStep != tutorialCraftTouch&&
            TutorialStep != tutorialCookingTouch&&
            TutorialStep != tutorialSleepingTouch&&
            TutorialStep != tutorialGahteringingTouch
            )
        {
            delay = 0f;
            TutorialStep++;
            Debug.Log(TutorialStep);
        }
    }
    public IEnumerator CoCampTutorial()
    {
        GameManager.Manager.State = GameState.Tutorial;
        LongTouch(0.25f,0.6f,"제조"); //craft 
        yield return new WaitWhile(() => TutorialStep < 1);
        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.83f, 0.9f,"제조x버튼"); //Craft xbutton
        yield return new WaitWhile(() => TutorialStep < 2);
        SetActive(false);
        yield return new WaitForSeconds(0.5f);
        LongTouch(0.65f, 0.5f, "요리"); //recipe 
        yield return new WaitWhile(() => TutorialStep < 3);
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.83f, 0.9f, "레시피x버튼"); //reipce xbutton
        yield return new WaitWhile(() => TutorialStep < 4);
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(0.5f);
        LongTouch(0.5f, 0.45f, "모닥불"); //bonfire 
        yield return new WaitWhile(() => TutorialStep < 5);
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        LongTouch(0.5f, 0.7f, "수면"); //bonfire 
        yield return new WaitWhile(() => TutorialStep < 6);
        yield return new WaitForSeconds(2f);
        LongTouch(0.83f, 0.9f, "수면x버튼"); //sleeping xbutton
        yield return new WaitWhile(() => TutorialStep < 7);
        SetActive(false);
        yield return new WaitForSeconds(0.5f);
        LongTouch(0.8f, 0.6f, "채집"); //gathering
        yield return new WaitWhile(() => TutorialStep < 8);
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.83f, 0.9f, "채집x버튼"); //gathering xbutton
        CampTutorialEnd();
    }
    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }

    public void LongTouch(float widthmultifle, float heightmultifle,string description)
    {
        SetActive(true, true, true);

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(200f, 200f);

        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
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
