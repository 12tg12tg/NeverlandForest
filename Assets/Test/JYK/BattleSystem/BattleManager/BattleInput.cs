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
        battleStartBut.gameObject.SetActive(true);
    }

    public void StartButton() // ��ư�� �Լ�
    {
        battleStartBut.gameObject.SetActive(false);

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
