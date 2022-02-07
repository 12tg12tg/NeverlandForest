using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HuntTutorial : MonoBehaviour
{
    public int TutorialStep { get; set; } = 0;
    
    public HuntPlayer huntPlayers;
    public HuntTile tile;

    public RectTransform target;
    public Button huntButton;

    private Rect canvasRt;
    private RectTransform handIcon;
    private RectTransform dialogBox;
    private TMP_Text dialogText;
    private RectTransform blackout;

    private Sprite rect;
    private Sprite circle;

    private DialogBoxObject dialogBoxObj;

    private float delay;

    private readonly float arrowSize = 50f;
    private readonly float boxWidth = 250f;
    private readonly float boxHeight = 100f;

    private readonly int TutorialStepTile = 1;
    private readonly int TutorialStepMove = 2;
    private readonly int TutorialStepHunt = 4;
    private readonly int TutorialStepSuccess = 5;
    private readonly int TutorialStepGuide = 6;

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
            delay > 1f &&
            TutorialStep != TutorialStepTile &&
            TutorialStep != TutorialStepMove &&
            TutorialStep != TutorialStepHunt &&
            TutorialStep != TutorialStepSuccess)
        {
            delay = 0f;
            TutorialStep++;
            Debug.Log(TutorialStep);
        }
    }

    public IEnumerator CoHuntTutorial()
    {
        // ���� ��⿡ ���� ��� ���� �� �߰��� ����(�ƹ��� ��ġ�� �Ѿ)
        LanternExplain();
        yield return new WaitWhile(() => TutorialStep < 1);

        // �̵� ��� ����(�̵� �ؾ��ϴ� ���� ��ġ�ؾ� �Ѿ(1,2))
        delay = 0f;
        MoveExplain();
        huntPlayers.tutorialTile = tile; // �ν��� �ִ� ���� ��ġ ���� �� step�� �������� �̵� ��Ų��(Ÿ�Ͽ��� ������Ŵ)
        yield return new WaitWhile(() => TutorialStep < 2);
        
        delay = 0f;
        huntPlayers.TutorialMove(() => {
            TutorialStep++;
            Debug.Log(TutorialStep);
            });
        yield return new WaitWhile(() => TutorialStep < 3);

        // �ν� ����(�ƹ��� ��ġ�� �Ѿ)
        delay = 0f;
        BushExplain();
        yield return new WaitWhile(() => TutorialStep < 4);

        // ��� Ȯ�� ��� ����(��� ��ư ������ �Ѿ)
        delay = 0f;
        HuntingExplain();
        yield return new WaitWhile(() => TutorialStep < 5);

        // ���(�� ������ �Ѿ)
        delay = 0f;
        HuntSuccessExplain();
        yield return new WaitWhile(() => TutorialStep < 6);

        // ��� ���� �˾� ����(����)
        delay = 0f;
        HuntHelpExplain();
        yield return new WaitWhile(() => TutorialStep < 7);

        HuntTutorialEndExplain();
        huntPlayers.IsTutorialClear =
            GameManager.Manager.tm.contentsTutorial.contentsTutorialProceed.Hunt = true;
    }
    public void SetActive(bool isBlackoutActive, bool isDialogActive = false, bool isHandActive = false)
    {
        blackout.gameObject.SetActive(isBlackoutActive);
        dialogBox.gameObject.SetActive(isDialogActive);
        handIcon.gameObject.SetActive(isHandActive);
    }
    public void LanternExplain()
    {
        SetActive(true, true);
        dialogBoxObj.up.SetActive(true);
        dialogBox.anchoredPosition = new Vector2(canvasRt.width / 2 - boxWidth / 2, canvasRt.height / 2 + 100f);
        dialogText.text = "���� ��� ����";
    }
    public void MoveExplain()
    {
        SetActive(true, true, true);
        var sizeX = tile.GetComponent<MeshRenderer>().bounds.size.x;

        blackout.GetComponent<Image>().sprite = circle;
        blackout.sizeDelta = new Vector2(120f, 120f);

        dialogBoxObj.up.SetActive(false);
        dialogBoxObj.left.SetActive(true);

        var offset = new Vector3(sizeX, 0f, 0f);

        var boxPos = Camera.main.WorldToViewportPoint(tile.transform.position - offset);
        var scrPos = Camera.main.WorldToViewportPoint(tile.transform.position);
        
        scrPos.x *= canvasRt.width;
        scrPos.y *= canvasRt.height;

        boxPos.x *= canvasRt.width;
        var tileOffset = (int)tile.index.x > 0 ? (int)tile.index.x > 1 ? 0.6f : 0.7f : 0.8f;
        boxPos.x += arrowSize + boxWidth * tileOffset;
        boxPos.y *= canvasRt.height;

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(scrPos.x, scrPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = scrPos;
        handIcon.anchoredPosition = scrPos;
        dialogBox.anchoredPosition = boxPos;

        dialogText.text = "�̵� ��� ����";
    }
    public void BushExplain()
    {
        SetActive(true, true);
        blackout.sizeDelta = Vector2.zero;
        dialogText.text = "�ν� ����";
    }
    public void HuntingExplain()
    {
        SetActive(true, true, true);
        huntButton.interactable = true;
        dialogBoxObj.left.SetActive(false);
        dialogBoxObj.down.SetActive(true);

        var uiCamera = GameManager.Manager.cm.uiCamera;
        var target = GetComponentInChildren<RepositionUI>().GetComponent<RectTransform>();
        var viewPos = uiCamera.WorldToViewportPoint(target.position);

        blackout.GetComponent<Image>().sprite = rect;
        var offset = new Vector2(10f, 10f);
        blackout.sizeDelta = target.sizeDelta + offset;

        viewPos.x *= canvasRt.width;
        viewPos.y *= canvasRt.height;
        viewPos.y += target.rect.height / 2;
        handIcon.anchoredPosition = viewPos;

        var blackBg = blackout.GetChild(0).GetComponent<RectTransform>();
        blackBg.anchoredPosition -= new Vector2(viewPos.x, viewPos.y) - blackout.anchoredPosition;
        blackout.anchoredPosition = viewPos;

        viewPos.x -= boxWidth / 2;
        viewPos.y += arrowSize + boxHeight - target.rect.height / 2;
        dialogBox.anchoredPosition = viewPos;

        dialogText.text = "��� Ȯ�� ��� ����";
    }
    public void HuntSuccessExplain()
    {
        SetActive(true, false, true);
        blackout.sizeDelta = Vector2.zero;

        var uiCamera = GameManager.Manager.cm.uiCamera;
        var viewPos = uiCamera.WorldToViewportPoint(target.position);
        Debug.Log(viewPos);
        viewPos.x *= canvasRt.width;
        viewPos.y *= canvasRt.height;
        Debug.Log(viewPos);

        handIcon.anchoredPosition = viewPos;

        //dialogText.text = "��� ���� ����";
    }
    public void HuntHelpExplain()
    {
        SetActive(true, true);
        dialogBoxObj.down.SetActive(false);
        var sizeUp = dialogBox.sizeDelta * 3;
        dialogBox.sizeDelta += sizeUp;
        dialogBox.pivot = new Vector2(0.5f, 0.5f);
        dialogBox.anchoredPosition = canvasRt.size / 2;
        dialogBoxObj.guide.gameObject.SetActive(true);
        dialogBoxObj.guide.sizeDelta += sizeUp;
        dialogText.text = "��� ���� �˾� ����";
    }
    public void HuntTutorialEndExplain()
    {
        SetActive(false);
        Destroy(this);
    }
}
