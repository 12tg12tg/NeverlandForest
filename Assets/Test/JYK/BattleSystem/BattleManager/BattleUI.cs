using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public enum ProgressIcon
    {
        Boy, Girl, X, Potion
    }

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
    [SerializeField] private Sprite girl_Icon;
    [SerializeField] private Sprite boy_Icon;
    [SerializeField] private Sprite x_Icon;
    [SerializeField] private Sprite potion_Icon;

    [Header("카메라 이동 화살표 위치")]
    public Canvas uiCanvas;
    [SerializeField] public Button moveCameraRightButton;
    [SerializeField] public Button moveCameraLeftButton;

    [Header("턴 넘기기 버튼")]
    public GameObject turnSkipTrans;
    public Button turnSkipButton;
    public Button battleStartButton;

    [Header("보상창띄우기")]
    public GameObject rewardPopup;
    public DungeonRewardDiaryManager rewardDiaryManager;

    [Header("블루문세팅")]
    public Button backToCampButton;

    [Header("옵션")]
    public Button option;

    private List<Button> battleButtons = new List<Button>();

    private int progress;
    public int Progress { get => progress; }

    private void Start()
    {
        mt = MultiTouch.Instance;
        bm = BattleManager.Instance;
        tm = TileMaker.Instance;

        battleButtons.Add(turnSkipButton);
        battleButtons.Add(moveCameraRightButton);
        battleButtons.Add(moveCameraLeftButton);
        battleButtons.Add(battleStartButton);
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
        if(buttonForVeiwMonsterSide)
        {
            moveCameraRightButton.gameObject.SetActive(true);
            moveCameraLeftButton.gameObject.SetActive(false);
        }
        else
        {
            moveCameraRightButton.gameObject.SetActive(false);
            moveCameraLeftButton.gameObject.SetActive(true);
        }
    }

    public void HideArrow()
    {
        moveCameraRightButton.gameObject.SetActive(false);
        moveCameraLeftButton.gameObject.SetActive(false);
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCanvas.worldCamera, out Vector2 localPos);

        // 캔버스 내의 rect transform의 지역좌표 설정
        lantern_Image.localPosition = localPos;
    }


    // Progress
    public void UpdateProgress_Button() // 버튼 스킵용
    {
        if (progress == 0)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = true;
            progressImg[0].sprite = x_Icon;
            progressImg[1].sprite = x_Icon;
        }
        else if (progress == 1)
        {
            progressImg[1].enabled = true;
            progressImg[1].sprite = x_Icon;

        }

        progress = 2;
        bm.EndOfPlayerAction();
    }

    public void UpdateProgress(ProgressIcon type)
    {
        progress++;
        UpdateProgressUI(type);
    }

    public void ResetProgress()
    {
        progress = 0;
        progressImg[0].enabled = false;
        progressImg[1].enabled = false;
    }

    private void UpdateProgressUI(ProgressIcon type)
    {
        Sprite temp = null;
        switch (type)
        {
            case ProgressIcon.Boy:
                temp = boy_Icon;
                break;
            case ProgressIcon.Girl:
                temp = girl_Icon;
                break;
            case ProgressIcon.Potion:
                temp = potion_Icon;
                break;
            case ProgressIcon.X:
                temp = x_Icon;
                break;
        }

        int prog = bm.uiLink.Progress;
        if (prog == 0)
        {
            progressImg[0].enabled = false;
            progressImg[1].enabled = false;
        }
        else if (prog == 1)
        {
            progressImg[0].enabled = true;
            progressImg[0].sprite = temp;

        }
        else if (prog == 2)
        {
            progressImg[1].enabled = true;
            progressImg[1].sprite = temp;
        }
    }

    // Buttons Interactive
    public void AllButtonInteractive(bool interactive)
    {
        foreach (var button in battleButtons)
        {
            button.interactable = interactive;
        }
    }

    // Reward
    public void OpenRewardPopup()
    {
        rewardPopup.SetActive(true);
        rewardDiaryManager.OpenRewardsPopup(GameManager.Manager.reward.GetBattleRewards(bm.curMonstersNum));
    }

    // BlueMoon
    public void BackToCamp()
    {
        var trapPos = Vars.UserData.trapPos;
        var trapType = Vars.UserData.trapType;
        trapPos.Clear();
        trapType.Clear();

        var tiles = tm.TileList;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].obstacle == null)
                continue;
            trapPos.Add(tiles[i].index);
            trapType.Add(tiles[i].obstacle.type);
        }

        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
        GameManager.Manager.LoadScene(GameScene.Camp);
    }
}
