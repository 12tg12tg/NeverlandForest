using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInput : MonoBehaviour
{
    private BattleManager bm;
    [SerializeField] private Button battleStartBut;
    public bool isLastInputDrag; // 애니메이션함수에서 타겟을 정할 때 드래그방식이었는지 터치방식이었는지 확인할 때 사용.

    private void Start()
    {
        bm = BattleManager.Instance;
    }

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
        if (bm.isTutorial && !bm.tutorial.tu_05_BattleStart)
            bm.tutorial.tu_05_BattleStart = true;

        // 시작 버튼 숨김
        SetActivateStartButton(false);
        bm.uiLink.progressTrans.SetActive(true);

        // 카메라 전환 Arrow 숨김
        bm.uiLink.HideArrow();

        // 몬스터 UI 고정
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
