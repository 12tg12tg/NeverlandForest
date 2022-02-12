using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
[DefaultExecutionOrder(12)]

public class MainRoomTutorial : MonoBehaviour
{
    public bool isMainRoomTutorial = false;
    public int TutorialStep { get; set; } = 0;

    public TutorialTool tutorialTool;

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

    private readonly int mainRoomCampButton = 1;
    private readonly int mainRoomUseButton = 3;

    [Header("포지션 타겟")]
    public RectTransform stamina;
    public RectTransform campBtn;
    public RectTransform campMenu;
    public RectTransform woodUseBtn;

    // 스토리에서 쓰고 있는 변수들
    private StoryManager stm;
    private MultiTouch multiTouch;
    private bool storyChapter4 = false;
    private bool storyChapter5 = false;
    private PlayerDungeonUnit boy;
    private PlayerDungeonUnit girl;

    private void Awake()
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

        //if(GameManager.Manager.TutoManager.mainTutorial.MainTutorialStage == MainTutorialStage.Camp)
        //{
        //    TutorialStep = 0;
        //    delay = 0f;
        //    StartCoroutine(CoTutorialEnd());
        //}
        
        // 스토리
        stm = GameManager.Manager.StoryManager;
        multiTouch = GameManager.Manager.MultiTouch;
        boy = DungeonSystem.Instance.dungeonPlayerBoy;
        girl = DungeonSystem.Instance.dungeonPlayerGirl;
    }

    private void Update()
    {
        if (isMainRoomTutorial && !storyChapter4 && !storyChapter5)
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

        // 스토리
        if (multiTouch.IsTap && (storyChapter4 || storyChapter5))
        {
            stm.isNext = true;
        }
    }
    public void EndTutorialExplain()
    {
        SetActive(false, true);
        var boxPos = new Vector2(canvasRt.width * 0.5f - boxWidth / 2, canvasRt.height * 0.8f);
        dialogBox.anchoredPosition = boxPos;
        dialogText.text = "튜토리얼 종료! 오른쪽으로 이동해서 다음맵으로 진행해보세요.";
    }

    public void TutorialTheEnd()
    {
        SetActive(false);
        Destroy(this);
    }

    public IEnumerator CoTutorialEnd()
    {
        isMainRoomTutorial = true;
        delay = 0f;
        yield return new WaitWhile(() => TutorialStep < 1);

        EndTutorialExplain();
        yield return new WaitWhile(() => TutorialStep < 2);

        isMainRoomTutorial = false;
        SetActive(false);
        yield return new WaitWhile(() => TutorialStep < 3);
        isMainRoomTutorial = true;
        #region 챕터 5
        storyChapter5 = true;
        // 챕터 5에서 필요 없는 것들 끄기
        var camera = Camera.main;
        var fov = camera.fieldOfView;
        camera.fieldOfView = 60;
        camera.GetComponent<MainCameraMove>().enabled = false;
        var canvas = stamina.root.gameObject;
        canvas.SetActive(false); //모든 UI 끄기

        // 플레이어 및 카메라 위치 잡기
        var cameraTrans = camera.transform;
        var op = cameraTrans.localPosition;
        var or = cameraTrans.localRotation;
        var boyp = boy.transform.localPosition;
        var boyr = boy.transform.localRotation;
        var girlp = girl.transform.localPosition;
        var girlr = girl.transform.localRotation;

        cameraTrans.localPosition = new Vector3(2.5f, 2.9f, -4.6f);
        cameraTrans.localRotation = Quaternion.Euler(new Vector3(21.153f, 0f, 0f));
        boy.transform.localPosition = new Vector3(0f, 0f, 1.5903f);
        boy.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        girl.transform.localPosition = new Vector3(5f, 0f, 1.59f);
        girl.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));

        var isNextChapter = false;
        stm.MessageBox.SetActive(true); // 대화창 오픈
        StartCoroutine(stm.CoStory(StoryType.Chapter5, () => isNextChapter = true));
        yield return new WaitWhile(() => !isNextChapter);
        // 초기화
        camera.fieldOfView = fov;
        camera.GetComponent<MainCameraMove>().enabled = true;
        cameraTrans.localPosition = op;
        cameraTrans.localRotation = or;
        boy.transform.localPosition = boyp;
        boy.transform.localRotation = boyr;
        girl.transform.localPosition = girlp;
        girl.transform.localRotation = girlr;
        canvas.SetActive(true);
        stm.MessageBox.SetActive(false);
        storyChapter5 = false;
        #endregion
        yield return new WaitWhile(() => TutorialStep < 4);

        isMainRoomTutorial = false;
        TutorialTheEnd();
    }

    public IEnumerator CoMainRoomTutorial()
    {
        isMainRoomTutorial = true;
        yield return new WaitForSeconds(1f);

        #region 챕터 4
        storyChapter4 = true;
        // 챕터 4에서 필요 없는 것들 끄기
        var camera = Camera.main;
        var fov = camera.fieldOfView;
        camera.fieldOfView = 60;
        camera.GetComponent<MainCameraMove>().enabled = false;
        var canvas = stamina.root.gameObject;
        canvas.SetActive(false); //모든 UI 끄기

        // 플레이어 및 카메라 위치 잡기
        var cameraTrans = camera.transform;
        var op = cameraTrans.localPosition;
        var or = cameraTrans.localRotation;
        var boyp = boy.transform.localPosition;
        var boyr = boy.transform.localRotation;
        var girlp = girl.transform.localPosition;
        var girlr = girl.transform.localRotation;

        cameraTrans.localPosition = new Vector3(2.5f, 2.9f, -4.6f);
        cameraTrans.localRotation = Quaternion.Euler(new Vector3(21.153f, 0f, 0f));
        boy.transform.localPosition = new Vector3(0f, 0f, 1.5903f);
        boy.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        girl.transform.localPosition = new Vector3(5f, 0f, 1.59f);
        girl.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));

        var isNextChapter = false;
        stm.MessageBox.SetActive(true); // 대화창 오픈
        StartCoroutine(stm.CoStory(StoryType.Chapter4, () => isNextChapter = true));
        yield return new WaitWhile(() => !isNextChapter);
        // 초기화
        camera.fieldOfView = fov;
        camera.GetComponent<MainCameraMove>().enabled = true;
        cameraTrans.localPosition = op;
        cameraTrans.localRotation = or;
        boy.transform.localPosition = boyp;
        boy.transform.localRotation = boyr;
        girl.transform.localPosition = girlp;
        girl.transform.localRotation = girlr;
        canvas.SetActive(true);
        stm.MessageBox.SetActive(false);
        storyChapter4 = false;
        #endregion

        tutorialTool.BlackPanelOn();

        StaminaExplain();
        yield return new WaitWhile(() => TutorialStep < 1);

        CampButtonExplain();
        yield return new WaitWhile(() => TutorialStep < 2);

        CampNeedMaterialUseExplain();
        yield return new WaitWhile(() => TutorialStep < 3);

        UseButtonExplain();
        yield return new WaitWhile(() => TutorialStep < 4);

        MainRoomTutorialEnd();
        tutorialTool.BlackPanelOff();
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
        dialogText.text = "일정 구간을 이동하거나 던전 내 행동을 할 때 마다 스태미나가 소비 돼.";
    }

    public void CampButtonExplain()
    {
        SetActive(true, true, true);
        target = campBtn;

        var button = target.GetComponent<Button>();
        button = tutorialTool.TutorialTargetButtonActivate(button);
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
        dialogText.text = "메인 스테이지에선 야영지를 설치 할 수 있어.";
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
        dialogText.text = "야영지를 설치 하기 위해선 다음과 같은 아이템이 필요해.";
    }

    public void UseButtonExplain()
    {
        SetActive(true, true, true);
        target = woodUseBtn;

        var button = target.GetComponent<Button>();
        button = tutorialTool.TutorialTargetButtonActivate(button);
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
        dialogText.text = "나무토막 3개를 소비하여 야영지를 설치 해 보자.";
    }




    public void MainRoomTutorialEnd()
    {
        SetActive(false);
        dialogBoxObj.down.SetActive(false);
        dialogBox.pivot = new Vector2(0f, 0.5f);
    }
}
