using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private MultiTouch mt;
    private BattleManager bm;
    private TileMaker tm;

    [Header("UI 연결")]
    public BattleMessage message;
    public BattleMessage cautionMessage;
    public Image dragSlot;

    [Header("랜턴 범위 표시 이미지 연결")]
    public Transform lanternUI_Target;
    public RectTransform lantern_Image;

    [Header("드래그 중 인지 확인")]
    public bool isDrag;

    [Header("전투시에만 활성화할 플레이어 진행도 연결")]
    public GameObject progressTrans;
    [SerializeField] private List<Image> progressImg;

    [Header("카메라 이동 화살표 위치")]
    [SerializeField] private Transform arrowPos;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform arrowButton;

    [Header("턴 넘기기 버튼")]
    public GameObject turnSkipButton;

    private int progress;
    public int Progress { get => progress; }

    private void Start()
    {
        mt = MultiTouch.Instance;
        bm = BattleManager.Instance;
        tm = TileMaker.Instance;
    }

    private void Update()
    {
        if (isDrag)
        {
            dragSlot.transform.position = mt.TouchPos;
        }
    }

    // Message
    public void PrintMessage(string message, float time, UnityAction action)
    {
        this.message.PrintMessage(message, time, action);
    }

    public void PrintCaution(string message, float time, float fadeDelay, UnityAction action)
    {
        cautionMessage.PrintMessageFadeOut(message, 0.7f, 0.5f, action);
    }

    // Arrow Image
    public void ShowArrow(bool buttonForVeiwMonsterSide)
    {
        arrowButton.gameObject.SetActive(true);

        // 월드 카메라에서의 좌표 전환 - 스크린 좌표 얻기
        var screenPos = Camera.main.WorldToScreenPoint(arrowPos.position);

        if (screenPos.z < 0.0f)
            screenPos *= -1.0f;

        // 캔버스 기준으로 스크린 좌표 재해석
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);

        // 캔버스 내의 rect transform의 지역좌표 설정
        arrowButton.localPosition = localPos;

        // 화살표 방향 결정 및 연결 함수 관리
        if (!buttonForVeiwMonsterSide)
        {
            arrowButton.rotation = Quaternion.Euler(0f, 0f, 180f);
            arrowButton.GetComponent<Button>().onClick.RemoveAllListeners();
            arrowButton.GetComponent<Button>().onClick.AddListener(bm.directingLink.ShowBattleSide);
        }
        else
        {
            arrowButton.rotation = Quaternion.identity;
            arrowButton.GetComponent<Button>().onClick.RemoveAllListeners();
            arrowButton.GetComponent<Button>().onClick.AddListener(bm.directingLink.ShowMonsterSide);
        }
    }

    public void HideArrow()
    {
        arrowButton.gameObject.SetActive(false);
    }

    // Lantern Image
    public void ShowLanternRange()
    {
        lantern_Image.gameObject.SetActive(true);
        UpdateLanternRange();
    }

    public void HideLanternRange()
    {
        lantern_Image.gameObject.SetActive(false);
    }

    public void UpdateLanternRange()
    {
        // 타겟 움직여놓기
        tm.MoveLanternRange();

        // 월드 카메라에서의 좌표 전환 - 스크린 좌표 얻기
        var screenPos = Camera.main.WorldToScreenPoint(lanternUI_Target.position);

        if (screenPos.z < 0.0f)
            screenPos *= -1.0f;

        // 캔버스 기준으로 스크린 좌표 재해석
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);

        // 캔버스 내의 rect transform의 지역좌표 설정
        lantern_Image.localPosition = localPos;
    }


    // Progress
    public void UpdateProgress_Button() // 버튼 스킵용
    {
        progress++;
        UpdateProgressUI();
        bm.EndOfPlayerAction();
    }

    public void UpdateProgress()
    {
        progress++;
        UpdateProgressUI();
    }

    public void ResetProgress()
    {
        progress = 0;
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        int prog = bm.uiLink.Progress;
        if (prog == 0)
        {
            progressImg[0].enabled = false;
            progressImg[1].enabled = false;
        }
        else if (prog == 1)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = false;
        }
        else if (prog == 2)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = true;
        }
    }
}
