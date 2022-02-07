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

    [Header("ī�޶� �̵� ȭ��ǥ ��ġ")]
    [SerializeField] private Transform arrowPos;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform arrowButton;

    [Header("�� �ѱ�� ��ư")]
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

        // ���� ī�޶󿡼��� ��ǥ ��ȯ - ��ũ�� ��ǥ ���
        var screenPos = Camera.main.WorldToScreenPoint(arrowPos.position);

        if (screenPos.z < 0.0f)
            screenPos *= -1.0f;

        // ĵ���� �������� ��ũ�� ��ǥ ���ؼ�
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);

        // ĵ���� ���� rect transform�� ������ǥ ����
        arrowButton.localPosition = localPos;

        // ȭ��ǥ ���� ���� �� ���� �Լ� ����
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
