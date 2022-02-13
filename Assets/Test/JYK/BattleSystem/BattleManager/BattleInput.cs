using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInput : MonoBehaviour
{
    private BattleManager bm;
    [SerializeField] private Button battleStartBut;
    public bool isLastInputDrag; // �ִϸ��̼��Լ����� Ÿ���� ���� �� �巡�׹���̾����� ��ġ����̾����� Ȯ���� �� ���.

    private void Start()
    {
        bm = BattleManager.Instance;
    }

    public void WaitUntillSettingDone()    // �÷��̾� �Է� ���
    {
        //�κ��丮���� Ʈ���� ��ų �׵θ� On && ������ ��Ȱ��ȭ
        BottomUIManager.Instance.ItemListInit();

        if (bm.isBluemoonSet)
        {
            // ��繮 �غ��� ���

        }
        else
        {
            // �������� ��ư Ȱ��ȭ
            SetActivateStartButton(true);

            // ȭ��ǥ �� ��ġ�� ����
            BattleManager.Instance.uiLink.ShowArrow(true);

            // ���� UI ī�޶� ����ٴϵ���
            BattleManager.Instance.waveLink.SetAllMonsterFollowUI(true);
        }
    }

    public void SetActivateStartButton(bool isShow)
    {
        battleStartBut.gameObject.SetActive(isShow);
    }

    public void StartButton() // ��ư�� �Լ�
    {
        if (bm.isTutorial && !bm.tutorial.tu_05_BattleStart)
            bm.tutorial.tu_05_BattleStart = true;

        // ���� ��ư ����
        SetActivateStartButton(false);
        bm.uiLink.progressTrans.SetActive(true);

        // ī�޶� ��ȯ Arrow ����
        bm.uiLink.HideArrow();

        // ���� UI ����
        bm.waveLink.SetAllMonsterFollowUI(false);

        var start = bm.FSM.GetState(BattleState.Start) as BattleStart;
        start.IsReadyDone = true;
    }

    public void EnableStartButton()
    {
        battleStartBut.interactable = true;
    }

    public void DisableStartButton()
    {
        battleStartBut.interactable = false;
    }
}
