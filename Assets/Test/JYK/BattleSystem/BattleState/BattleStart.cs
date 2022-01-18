using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : State<BattleState>
{
    private BattleManager manager;
    private float startDelay = 2.5f;
    private bool isAnimationDone;
    private bool isTrapSet;

    public bool IsTrapDone { get => isTrapSet; set => isTrapSet = value; } 

    public BattleStart(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        isAnimationDone = false;
        isTrapSet = false;

        // 흐름
        //  전투방 입장 → 캐릭터 두명 클로즈업 상태 → 두려운 애니메이션 → 화살 대기 애니메이션 
        //  → 원래 구도로 카메라 이동 → 타일 열리기 → 전투 준비단계 메세지 → 함정 설치
        //  → (설치 완료 입력시) → FSM.ChangeState(BattleState.Player);

        //  연출 부분 [전투방 입장 → ... → 타일 열리기] 까지는 코루틴으로 한번에 하나의 함수로 차례로 진행.
        //  그게 끝난 시점에서 [전투 준비단계 메세지] 부터 아래 코드 실행.

        manager.PrintMessage("전투 준비 단계", startDelay, () => manager.ActivateTrapSetUI());
    }

    public override void Release()
    {
        Debug.Log("Battle Start Release");
    }

    public override void Update()
    {
        /*메세지 띄운 후 시작 연출*/
        if (isTrapSet)
        {
            isTrapSet = false;
            FSM.ChangeState(BattleState.Player);
            manager.UpdateWave();
        }
    }

    public override void LateUpdate()
    {
    }

    public override void FixedUpdate()
    {
    }
}
