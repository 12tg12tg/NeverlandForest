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
    private bool isWaitingTouch;
    private bool tutorialBonableItemCheck;
  
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
    private void Update()
    {
        if (isWaitingTouch)
        {
            if (GameManager.Manager.MultiTouch.TouchCount > 0)
            {
                tutorialBonableItemCheck = true;
                isWaitingTouch = false;
            }
        }
    }
    public IEnumerator CoCampTutorial()
    {
        GameManager.Manager.State = GameState.Tutorial;
        LongTouch(0.25f,0.6f,0.5f,0.6f,"�̰��� ������ �Ҽ��ִ� �����̾�. \n" +
            "������, ��ġ��,������� ����� �ִ°�����. \n" +
            "���� �������� �־�� �����Ұž�.",true); //craft 
        yield return new WaitUntil(() => tutorialCraftTouch);
        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���",false,true); //Craft xbutton
        tutorialCraftTouch = false;
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.68f, 0.5f, 0.5f, 0.5f, "�̰��� �丮�� �� �� �ִ� ���̾�. \n" +
            "���¹̳��� �������¹̳��� ȸ�� �� �� �ִ� ������. \n" +
            "���� �����ǰ� �־�� �丮�� ���� ���� �� �־�.",false,true); //recipe 
        yield return new WaitUntil(() => tutorialCookingTouch);
        tutorialCookingTouch = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���", false, true); //reipce xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.5f, 0.45f, 0.5f, 0.8f, "�̰��� �߿������� ���۰� �丮�� �� �� �ְ����ִ� ��ȭ�� \n" +
            "��ں��� �ð��� �˷��ְ� �Ʒ� �κ��丮���� \n" +
            "�¿� ���ִ� �������� �˷��ִ¹�ư�̾� ��ư�� ������ �¿�� �ִ� �������� ������ ��ȭ�ϰ� ����.",false,false,false,true); //bonfire 
        yield return new WaitUntil(() => tutorialBonfirecheckButtonClick);
        tutorialBonfirecheckButtonClick = false;
        isWaitingTouch = true;
        LongTouch(0.5f, 0.3f, 0.7f, 0.5f, "������ �� Ȯ��", false, false, false, true);
        yield return new WaitUntil(() => tutorialBonableItemCheck);
        tutorialBonableItemCheck = false;
        blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.5f, 0.7f, 0.5f, 0.8f, "����", false, false, false, true); 
        yield return new WaitUntil(() =>tutorialSleepingTouch);
        tutorialSleepingTouch = false;
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���", false, true); //sleeping xbutton
        yield return new WaitUntil(() => isquitbuttonClick);
        isquitbuttonClick = false;
        SetActive(false);
        yield return new WaitForSeconds(2.5f);
        LongTouch(0.8f, 0.6f, 0.5f, 0.8f, "ä��"); //gathering
        yield return new WaitUntil(() =>tutorialGahteringingTouch);
        tutorialGahteringingTouch = false;
        SetActive(false);
        yield return new WaitForSeconds(2f);
        LongTouch(0.81f, 0.9f, 0.6f, 0.8f, "x��ư�� ������ �������� �����غ���", false, true); //gathering xbutton
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

    public void LongTouch(float widthmultifle, float heightmultifle,float boxwidthmultifle,float boxheightmultifle, string description,
        bool left=false, bool right=false, bool up =false, bool bottom =false)
    {
        SetActive(true, true, true);

        dialogBoxObj.left.SetActive(left);
        dialogBoxObj.right.SetActive(right);
        dialogBoxObj.up.SetActive(up);
        dialogBoxObj.down.SetActive(bottom);

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
