using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInput : MonoBehaviour
{
    [SerializeField] private Button battleStartBut;

    // �÷��̾� �Է� ���
    public void WaitUntillSettingDone()
    {
        /* �������� ��ư Ȱ��ȭ
           �κ��丮���� Ʈ���� ��ų �׵θ� On && ������ ��Ȱ��ȭ */



        battleStartBut.gameObject.SetActive(true);
    }
    public void StartButton() // ��ư�� �Լ�
    {
        battleStartBut.gameObject.SetActive(false);

        var start = BattleManager.Instance.FSM.GetState(BattleState.Start) as BattleStart;
        start.IsReadyDone = true;
    }
}
