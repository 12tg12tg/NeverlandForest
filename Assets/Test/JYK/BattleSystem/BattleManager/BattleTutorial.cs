using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTutorial : MonoBehaviour
{
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;
    private BottomUIManager bottomUI;

    [Header("����ũ ��� ����")]
    [SerializeField] private Sprite rect;
    [SerializeField] private Sprite circle;

    [Header("����ũ & ���")]
    [SerializeField] private RectTransform mask;
    [SerializeField] private Image maskImg;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform fingerImg;
    [SerializeField] private Transform descriptionBox;

    private bool tu_01_CamRightButton;
    private bool isWaitingTouch;
    private bool isTouched;

    private void Start()
    {
        bottomUI = BottomUIManager.Instance;
    }
    public void Init()
    {
        tu_01_CamRightButton = false;
    }

    public void StartDutorial()
    {
        Init();
        StartCoroutine(CoBattleTutorial());
    }

    private void Update()
    {
        if(isWaitingTouch)
        {
            isWaitingTouch = false;
            isTouched = GameManager.Manager.MultiTouch.IsTap;
        }
    }

    public void EndDutorial()
    {
        GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial();
    }

    private IEnumerator CoBattleTutorial()
    {
        // ���̺� ����
        bm.TutorialInit();

        // ���̺� �̸����� ��ư Ŭ�� ���� & ���� ����
        bottomUI.ButtonInteractive(false);
        bm.uiLink.moveCameraRightButton.interactable = true;
        SetFirstCommand();    
        yield return new WaitUntil(() => tu_01_CamRightButton);

        // ���̺� �̸����� ��� Ŭ�� ���� & ���� ����

        yield return null;

        // �κ��丮 ����Ʈ�� Ŭ�� ���� & ���� ����
        yield return null;

        // (Ư��) Ÿ�ϼ��� Ŭ��
        yield return null;

        // �������۹�ư ��ġ & ����
        yield return null;

        // ����(��ɲ� �ɷ�)
        yield return null;

        // ��ɲ� ��ų ��� ����
        yield return null;

        // ���� �ǵ� ����
        yield return null;

        // ����(�������� �ɷ�)
        yield return null;

        // �������� ��ų ��� ����
        yield return null;

        // ���� ��� ���� ����
        yield return null;

        // ���� �ൿ(������ξ���)


        // ���� ���� ����
        yield return null;

        // ����(�������� �ɷ�)
        yield return null;

        // ���� ���� ������ �ϱ�(����)


        yield return null; /*�¸� ? �й�? ��ٸ���*/

        if(true )
        {
            //�¸� - ����
        }
        else
        {
            //�й� - â���� ���⼭���� �ٽ�
        }

        EndDutorial();
    }

    private void SetFirstCommand()
    {
        mask.anchoredPosition = null;
        maskImg.sprite = circle;
        //background;
        //fingerImg;
        //descriptionBox;
    }
}
