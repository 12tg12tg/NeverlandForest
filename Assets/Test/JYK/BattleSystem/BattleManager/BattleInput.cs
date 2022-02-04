using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInput : MonoBehaviour
{
    [SerializeField] private Button battleStartBut;

    public void WaitUntillSettingDone()    // �÷��̾� �Է� ���
    {
        //�κ��丮���� Ʈ���� ��ų �׵θ� On && ������ ��Ȱ��ȭ
        BottomUIManager.Instance.ItemListInit();

        // �������� ��ư Ȱ��ȭ
        SetActivateStartButton(true);

        // ȭ��ǥ �� ��ġ�� ����
        BattleManager.Instance.directLink.ShowArrow(true);

        // ���� UI ī�޶� ����ٴϵ���
        BattleManager.Instance.waveLink.SetAllMonsterFollowUI(true);
    }

    public void SetActivateStartButton(bool isShow)
    {
        battleStartBut.gameObject.SetActive(isShow);
    }

    public void StartButton() // ��ư�� �Լ�
    {
        // ���� ��ư ����
        SetActivateStartButton(false);

        // ī�޶� ��ȯ Arrow ����
        BattleManager.Instance.directLink.HideArrow();

        // ���� UI ����
        BattleManager.Instance.waveLink.SetAllMonsterFollowUI(false);

        var start = BattleManager.Instance.FSM.GetState(BattleState.Start) as BattleStart;
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
