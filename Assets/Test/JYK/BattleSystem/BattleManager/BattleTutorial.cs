using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject mastGo;
    [SerializeField] private RectTransform maskRt;
    [SerializeField] private Image maskImg;
    [SerializeField] private GameObject background;
    [SerializeField] private RectTransform fingerImg;
    [SerializeField] private RectTransform dialogBoxRt;
    [SerializeField] private DialogBoxObject dialogBox;

    public bool tu_01_CamRightButton;
    public bool tu_02_CamLeftButton;
    public bool tu_03_TrapClick1;
    public bool tu_03_TrapClick2;
    public bool tu_04_TileClick;
    public bool tu_05_BattleStart;
    public bool tu_07_BoySkill1;
    public bool tu_07_BoySkill2;
    public bool tu_10_GirlSkill1;
    public bool tu_10_GirlSkill2;
    public bool tu_12_GirlSkill1;
    public bool tu_12_GirlSkill2;
    public bool isWin;
    public bool isLose;

    private bool isWaitingTouch;
    private bool isTouched;
    public bool lockSkillButtonClick;
    public bool lockSkillButtonDrag;
    public bool lockTileClick;
    public bool lockAutoBattleStateChange;

    private void Init()
    {
        bottomUI = BottomUIManager.Instance;
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
        lockSkillButtonClick = true;
        lockTileClick = true;
        lockAutoBattleStateChange = true;
        lockSkillButtonDrag = true;
    }

    public void StartDutorial()
    {
        gameObject.SetActive(true);
        Init();
        StartCoroutine(CoBattleTutorial());
    }

    private void Update()
    {
        if (isWaitingTouch)
        {
            if (GameManager.Manager.MultiTouch.IsTap)
            {
                isWaitingTouch = false;
                isTouched = true;
            }
        }
    }

    public void EndDutorial()
    {
        bm.uiLink.AllButtonInteractive(true);
        GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial();
    }

    private IEnumerator CoBattleTutorial()
    {
        // ���̺� ����
        bm.TutorialInit();
        bottomUI.ButtonInteractive(false);
        bm.uiLink.AllButtonInteractive(false);
        yield return new WaitForSeconds(3f);

        // �Է� 1. ���̺� �̸����� ��ư Ŭ�� ���� & ���� ����
        bm.uiLink.moveCameraRightButton.interactable = true;
        UISet_1st_Enter();
        yield return new WaitUntil(() => tu_01_CamRightButton);
        UISet_1st_End();
        yield return new WaitForSeconds(1.5f);

        // �Է� 2. ���̺� �̸����� ��� Ŭ�� ���� & ���� ����
        bm.uiLink.moveCameraLeftButton.interactable = true;
        UISet_2nd_Enter();
        yield return new WaitUntil(() => tu_02_CamLeftButton);
        UISet_2nd_End();
        bm.uiLink.AllButtonInteractive(false);
        yield return new WaitForSeconds(1.5f);

        // �Է� 3. �κ��丮 ����Ʈ�� Ŭ�� ���� & ���� ����
        bottomUI.ButtonInteractive(false);
        var targetButton = bottomUI.itemButtons[3].GetComponent<Button>();
        targetButton.interactable = true;
        UISet_3rd_01_Enter();
        yield return new WaitUntil(() => tu_03_TrapClick1);
        targetButton.interactable = false;

        var popupButtons = bottomUI.popUpWindow.GetComponentsInChildren<Button>();
        popupButtons[0].interactable = true;
        popupButtons[1].interactable = false;
        UISet_3rd_02_Enter();
        yield return new WaitUntil(() => tu_03_TrapClick2);
        popupButtons[0].interactable = true;
        popupButtons[1].interactable = true;
        UISet_3rd_End();


        // �Է� 4. Ư�� Ÿ�ϼ��� Ŭ��
        UISet_4th_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_04_TileClick);
        lockTileClick = true;
        UISet_4th_End();
        yield return new WaitForSeconds(1.5f);

        // �Է� 5. �������۹�ư ��ġ & ����
        bottomUI.ButtonInteractive(false);
        bm.uiLink.AllButtonInteractive(false);
        targetButton = bm.uiLink.battleStartButton;
        targetButton.interactable = true;
        UISet_5th_Enter();
        yield return new WaitUntil(() => tu_05_BattleStart);
        UISet_5th_End();
        yield return new WaitForSeconds(1f);
        bottomUI.ButtonInteractive(false);
        bm.uiLink.AllButtonInteractive(false);
        yield return new WaitForSeconds(2.2f);


        // �Է� 6. ����(��ɲ� �ɷ�)
        bottomUI.SkillButtonInit();
        UISet_6th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_6th_02_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_6th_End();
        yield return new WaitForSeconds(0.5f);


        // �Է� 7. ��ɲ� ��ų ��� ����
        UISet_7th_01_Enter();
        lockSkillButtonClick = false;
        bottomUI.skillButtons[2].ownButton.interactable = true;
        yield return new WaitUntil(() => tu_07_BoySkill1);

        lockSkillButtonClick = true;
        bottomUI.skillButtons[2].ownButton.interactable = false;

        UISet_7th_02_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_07_BoySkill2);
        UISet_7th_End();
        lockTileClick = true;
        yield return new WaitForSeconds(3.5f);

        // �Է� 8. ���� �ǵ� ����
        UISet_8th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_8th_02_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_8th_End();
        yield return new WaitForSeconds(0.7f);

        // �Է� 9. ����(�������� �ɷ�)
        UISet_9th_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_9th_End();
        yield return new WaitForSeconds(0.5f);

        // �Է� 10. �������� ��ų ��� ����
        UISet_10th_01_Enter();
        lockSkillButtonClick = false;
        bottomUI.skillButtons[8].ownButton.interactable = true;
        yield return new WaitUntil(() => tu_10_GirlSkill1);
        lockSkillButtonClick = true;
        bottomUI.skillButtons[8].ownButton.interactable = false;

        UISet_10th_02_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_10_GirlSkill2);
        UISet_10th_End();
        lockTileClick = true;
        yield return new WaitForSeconds(2f);

        // �Է� 11. ���� ��� ���� ����
        UISet_11th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_11th_02_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_11th_03_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_11th_End();

        // ... ���� �ൿ(������ξ���)
        bm.FSM.ChangeState(BattleState.Monster);
        bm.uiLink.turnSkipTrans.SetActive(false);
        lockAutoBattleStateChange = false;
        yield return new WaitForSeconds(6.5f);

        // �Է� 12. ���� ���� ����
        UISet_12th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_12th_02_Enter();
        bottomUI.skillButtons[11].ownButton.interactable = true;
        lockSkillButtonClick = false;
        yield return new WaitUntil(() => tu_12_GirlSkill1);
        lockSkillButtonClick = true;
        bottomUI.skillButtons[11].ownButton.interactable = false;

        UISet_12th_03_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_12_GirlSkill2);
        lockTileClick = true;
        UISet_12th_End();
        yield return new WaitForSeconds(3.5f);

        // �Է� 13. ���� ���� Ȯ�� Ŭ��
        UISet_13th_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_13th_End();

        // �Է� 14. ���� ���� ������ �ϱ�(����)
        UISet_14th_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_14th_End();

        lockTileClick = false;
        lockSkillButtonClick = false;
        bottomUI.ButtonInteractive(true);
        bm.uiLink.AllButtonInteractive(true);
        bottomUI.UpdateSkillInteractive();
        lockAutoBattleStateChange = false;
        lockSkillButtonDrag = false;

        while (!isWin)
        {
            yield return new WaitUntil(() => isWin || isLose); /*�¸� ? �й�? ��ٸ���*/

            if (isWin)
                break;

            // �޼���â �� �缳��
            isLose = false;
            UISet_15th_Enter();
            lockAutoBattleStateChange = true;
            lockSkillButtonDrag = true;
            lockTileClick = true;
            lockSkillButtonClick = true;

            bottomUI.ButtonInteractive(false);
            bm.uiLink.AllButtonInteractive(false);
            bottomUI.UpdateSkillInteractive();

            isWaitingTouch = true;
            yield return new WaitUntil(() => isTouched);
            isTouched = false;
            UISet_15th_End();

            lockTileClick = false;
            lockSkillButtonClick = false;
            lockAutoBattleStateChange = false;
            lockSkillButtonDrag = false;
            bottomUI.ButtonInteractive(true);
            bm.uiLink.AllButtonInteractive(true);
            bottomUI.UpdateSkillInteractive();

            RestartBattle();
        }
        
        //�¸� - ���� (����â�� ��������, ���� ���� �Ѿ ������)
        EndDutorial();
    }

    // ====================================================================================
    private void UISet_1st_Enter() // ���̺� �̸����� ��ư Ŭ�� ���� & ���� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(534f, -0.01f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(805, 300);
        dialogBox.text.text = "�� ��ư�� ������ ���� ���̺긦 �̸� ���캾�ϴ�.";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(1164, 285);
    }
    private void UISet_1st_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_2nd_Enter() // ���̺� �̸����� ��� Ŭ�� ���� & ���� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-533f, -0.01f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(214, 300);
        dialogBox.text.text = "���Ͱ� ���� �θ��� ���̱���?\n�ٽ� ���ư��� ������ �غ��սô�!";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(126, 275);
    }
    private void UISet_2nd_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_3rd_01_Enter() // �κ��丮 ����Ʈ�� Ŭ�� ���� & ���� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(292f, -198);
        maskRt.sizeDelta = new Vector2(65f, 65f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(816, 335);
        dialogBox.text.text = "��ġ���� ��ġ�ؼ� ������ ������ ���� �� �ֽ��ϴ�.\n����Ʈ���� ��ġ�غ��ô�!";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(946, 94);
    }
    private void UISet_3rd_02_Enter()
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(291f, -75f);
        maskRt.sizeDelta = new Vector2(91f, 36f);
        maskImg.sprite = rect;

        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(965, 216);
    }
    private void UISet_3rd_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_4th_Enter() // (Ư��) Ÿ�ϼ��� Ŭ��
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, 24f);
        maskRt.sizeDelta = new Vector2(147f, 84f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(478f, 324f);
        dialogBox.text.text = "�Ʊ� Ȯ���� ���̺� ������ ��������\n�߾Ӷ��ο� Ʈ���� ��ġ�صӽô�.";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(887f, 317f);
    } 
    private void UISet_4th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_5th_Enter() // �������۹�ư ��ġ & ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(0f, 159f);
        maskRt.sizeDelta = new Vector2(197f, 50f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(true);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 336f);
        dialogBox.text.text = "���� ������ �����غ����?";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(714f, 466f);
    } 
    private void UISet_5th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_6th_01_Enter() // ����(��ɲ� �ɷ�)
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-313f, 95f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(447f, 378f);
        dialogBox.text.text = "��ɲ��� ȭ���� ���� ��Ÿ� ���� ���� ������ �մϴ�.\n";

        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(false);
    }
    private void UISet_6th_02_Enter() // ����(��ɲ� �ɷ�)
    {
        dialogBox.text.text = "ȭ�쿡 ���� ���ʹ� Hp ���ظ� ������ �ӹڵ˴ϴ�.\n�ӹڿ� �ɸ� ���� �������� ���մϴ�.";
    }
    private void UISet_6th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_7th_01_Enter() // ��ɲ� ��ų ��� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, -197f);
        maskRt.sizeDelta = new Vector2(61f, 59f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(741, 228);
        dialogBox.text.text = "��ɲ��� ��ų�� ����غ��ô�.\n��ư�� ���� ��ų�� Ȱ��ȭ��Ű����.";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(876, 89);
    }
    private void UISet_7th_02_Enter() // ��ɲ� ��ų ��� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(334f, 36f);
        maskRt.sizeDelta = new Vector2(145f, 105f);
        maskImg.sprite = circle;

        // ��ȭ���� ����
        dialogBoxRt.gameObject.SetActive(false);

        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(986f, 321f);
    }
    private void UISet_7th_End()
    {
        maskRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_8th_01_Enter() // ���� �ǵ� ����
    {
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(651f, 351f);
        dialogBox.text.text = "��� ���̳���?\n ȭ���� �¾Ҵµ� �������� 0�̶��..\n";
    }
    private void UISet_8th_02_Enter() // ���� �ǵ� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(353f, 55f);
        maskRt.sizeDelta = new Vector2(31f, 5f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBox.text.text = "���͵��� ���������� ����ϴ� �ǵ带 ������ �ֽ��ϴ�.\n������ ��ɲ��� ������ �ǵ��ġ��ŭ ���ҵ� �������� ����˴ϴ�.";
    }
    private void UISet_8th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
    }

    private void UISet_9th_Enter() // ����(�������� �ɷ�)
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-388f, 0f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(371f, 300f);
        dialogBox.text.text = "���������� ���ϰ������� �ǵ带 ������ų �� �ֽ��ϴ�!";

        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(false);
    }
    private void UISet_9th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_10th_01_Enter() // �������� ��ų ��� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, -261.8f);
        maskRt.sizeDelta = new Vector2(61f, 59f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(741f, 159f);
        dialogBox.text.text = "���������� ��ų�� ����غ��ô�.\n��ư�� ���� ��ų�� Ȱ��ȭ��Ű����.";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(876f, 40f);
    }
    private void UISet_10th_02_Enter() // ��ɲ� ��ų ��� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(334f, 36f);
        maskRt.sizeDelta = new Vector2(145f, 105f);
        maskImg.sprite = circle;

        // ��ȭ���� ����
        dialogBoxRt.gameObject.SetActive(false);

        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(986f, 321f);
    }
    private void UISet_10th_End()
    {
        maskRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_11th_01_Enter() // ���� ��� ���� ����
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(349f, 53f);
        maskRt.sizeDelta = new Vector2(39f, 12f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(636f, 363f);
        dialogBox.text.text = "���������� �������� ������ �ǵ带 ���� ������Ű��\n���� �������� Hp�� ���ҽ�ŵ�ϴ�.";
    }
    private void UISet_11th_02_Enter()
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-11.4f, 248f);
        maskRt.sizeDelta = new Vector2(325f, 92f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(true);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(506f, 395f);
        dialogBox.text.text = "���������� ��ų�� ����������, ���� ��⸦ �Ҹ��մϴ�.\n���� ���Ϲ��� ��ų�� ������ �������� ������ �ݴϴ�.";
    }
    private void UISet_11th_03_Enter()
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(441f, -105f);
        maskRt.sizeDelta = new Vector2(75f, 75f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(749f, 193f);
        dialogBox.text.text = "�������� ��ų�� �ִ������ �� �������� ǥ�õ� �ٱ��� �Դϴ�.";
    }
    private void UISet_11th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_12th_01_Enter() // ���� ���� ��ų ���
    {
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 300f);
        dialogBox.text.text = "�������ڿ��Դ� ������ �����ϴ� ��ų�� �ֽ��ϴ�.";
    }
    private void UISet_12th_02_Enter()
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(426f, -262f);
        maskRt.sizeDelta = new Vector2(61f, 59f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(940f, 186f);
        dialogBox.text.text = "��ġ�Ͽ� \"���� ����\" ��ų�� Ȱ��ȭ�ϼ���.";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(1080f, 47f);
    }
    private void UISet_12th_03_Enter()
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-375f, -12f);
        maskRt.sizeDelta = new Vector2(134f, 134f);
        maskImg.sprite = circle;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(368f, 284f);
        dialogBox.text.text = "�������ڸ� Ŭ���Ͽ� ������ �����մϴ�.";
        // �հ��� ��ġ
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(261f, 278f);
    }
    private void UISet_12th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_13th_Enter()
    {
        // ����ũ ��ġ & ũ�� & ����
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-11.4f, 248f);
        maskRt.sizeDelta = new Vector2(325f, 92f);
        maskImg.sprite = rect;
        // ����ڽ� ��ġ & ���� & ȭ��ǥ
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(true);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(506f, 395f);
        dialogBox.text.text = "������ �Ҹ�Ǿ� ���� �������� �����Ǿ����ϴ�.";
    }
    private void UISet_13th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_14th_Enter()
    {
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 300f);
        dialogBox.text.text = "�پ��� ��ų�� Ȱ���Ͽ� �������� �¸��ϼ���!";
    }
    private void UISet_14th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_15th_Enter()
    {
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 300f);
        dialogBox.text.text = "Ʃ�丮���� �����ְ� �������ּ���!";
    }
    private void UISet_15th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }


    private void RestartBattle()
    {
        var list = new List<MonsterUnit>(bm.monsters);
        list.AddRange(bm.waveLink.wave1);
        list.AddRange(bm.waveLink.wave2);
        list.AddRange(bm.waveLink.wave3);
        bm.waveLink.wave1.Clear();
        bm.waveLink.wave2.Clear();
        bm.waveLink.wave3.Clear();
        tm.TileClear();
        list.ToList().ForEach(n => { if (n != null) n.Release(); });

        bm.TutorialInit(true);
    }
}
