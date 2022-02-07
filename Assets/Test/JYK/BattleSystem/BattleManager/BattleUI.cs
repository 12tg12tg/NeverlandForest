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
    public Camera uiCamera;
    public Canvas uiCanvas;
    [SerializeField] private Button moveCameraRightButton;
    [SerializeField] private Button moveCameraLeftButton;

    [Header("�� �ѱ�� ��ư")]
    public GameObject turnSkipTrans;
    public Button turnSkipButton;

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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);

        // ĵ���� ���� rect transform�� ������ǥ ����
        lantern_Image.localPosition = localPos;
    }


    // Progress
    public void UpdateProgress_Button() // ��ư ��ŵ��
    {
        progress++;
        UpdateProgressUI(ProgressIcon.X);
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
}
