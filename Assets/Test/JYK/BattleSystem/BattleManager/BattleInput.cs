using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInput : MonoBehaviour
{
    [SerializeField] private Button battleStartBut;

    public void WaitUntillSettingDone()    // 플레이어 입력 대기
    {
        //인벤토리에서 트랩류 스킬 테두리 On && 나머지 비활성화
        BottomUIManager.Instance.ItemListInit();

        // 전투시작 버튼 활성화
        SetActivateStartButton(true);

        // 화살표 제 위치에 생성
        BattleManager.Instance.uiLink.ShowArrow(true);

        // 몬스터 UI 카메라 따라다니도록
        BattleManager.Instance.waveLink.SetAllMonsterFollowUI(true);
    }

    public void SetActivateStartButton(bool isShow)
    {
        battleStartBut.gameObject.SetActive(isShow);
    }

    public void StartButton() // 버튼용 함수
    {
        // 시작 버튼 숨김
        SetActivateStartButton(false);
        BattleManager.Instance.uiLink.progressTrans.SetActive(true);

        // 카메라 전환 Arrow 숨김
        BattleManager.Instance.uiLink.HideArrow();

        // 몬스터 UI 고정
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
