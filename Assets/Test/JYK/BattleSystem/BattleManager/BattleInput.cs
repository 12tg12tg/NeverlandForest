using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInput : MonoBehaviour
{
    [SerializeField] private Button battleStartBut;

    // 플레이어 입력 대기
    public void WaitUntillSettingDone()
    {
        /* 전투시작 버튼 활성화
           인벤토리에서 트랩류 스킬 테두리 On && 나머지 비활성화 */



        battleStartBut.gameObject.SetActive(true);
    }
    public void StartButton() // 버튼용 함수
    {
        battleStartBut.gameObject.SetActive(false);

        var start = BattleManager.Instance.FSM.GetState(BattleState.Start) as BattleStart;
        start.IsReadyDone = true;
    }
}
