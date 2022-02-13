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

    [Header("UI ����")]
    public BattleMessage message;
    public BattleMessage cautionMessage;
    public Image dragSlot;

    [Header("���� ���� ǥ�� �̹��� ����")]
    public Transform lanternUI_Target;
    public RectTransform lantern_Image;

    [Header("�巡�� �� ���� Ȯ��")]
    public bool isDrag;

    [Header("�����ÿ��� Ȱ��ȭ�� �÷��̾� ���൵ ����")]
    public GameObject progressTrans;
    [SerializeField] private List<Image> progressImg;
    [SerializeField] private Sprite girl_Icon;
    [SerializeField] private Sprite boy_Icon;
    [SerializeField] private Sprite x_Icon;
    [SerializeField] private Sprite potion_Icon;

    [Header("ī�޶� �̵� ȭ��ǥ ��ġ")]
    public Canvas uiCanvas;
    [SerializeField] public Button moveCameraRightButton;
    [SerializeField] public Button moveCameraLeftButton;

    [Header("�� �ѱ�� ��ư")]
    public GameObject turnSkipTrans;
    public Button turnSkipButton;
    public Button battleStartButton;

    [Header("����â����")]
    public GameObject rewardPopup;
    public DungeonRewardDiaryManager rewardDiaryManager;

    [Header("��繮����")]
    public Button backToCampButton;

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
        // Ÿ�� ����������
        tm.MoveLanternRange();

        // ���� ī�޶󿡼��� ��ǥ ��ȯ - ��ũ�� ��ǥ ���
        var screenPos = Camera.main.WorldToScreenPoint(lanternUI_Target.position);

        if (screenPos.z < 0.0f)
            screenPos *= -1.0f;

        // ĵ���� �������� ��ũ�� ��ǥ ���ؼ�
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCanvas.worldCamera, out Vector2 localPos);

        // ĵ���� ���� rect transform�� ������ǥ ����
        lantern_Image.localPosition = localPos;
    }


    // Progress
    public void UpdateProgress_Button() // ��ư ��ŵ��
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
