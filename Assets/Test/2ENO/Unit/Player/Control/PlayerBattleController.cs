using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 사용자가 어떤 입력을 했을 때, 그에 해당하는 정보를 넘겨주거나, 동작을 실행시키거나 하는 클래스
// 배틀 시스템에 필요한 정보를 넘겨주고, 배틀 시스템에서 캐릭터 유닛의 상태에 따라 턴 넘겨주고 등등
// 배틀 시스템이 실시간으로 플레이어 캐릭터 상태를 체크하는게 아닌 캐릭터에서 자신의 상태변화를 배틀에 알려주는방식

public class PlayerBattleController : MonoBehaviour, IDropHandler
{
    /* 캐릭터 보유 스킬, 능력치(스텟, 버프디버프 여부 등), 착용 장비 등에 대한 정보 및 값 수정
    캐릭터 전투상태 정보 ( 공격전 준비, 공격후 대기, 죽음 ) */


    // Instance
    public BattleManager manager;
    public TileMaker tileMaker;
    private List<UnitBase> targetUnit;

    // Component
    public PlayerStats stats; 
    public PlayerBattleFSM FSM;

    // Vars
    public PlayerCommand command;
    public PlayerType playerType;    // 혹시 애니메이션이 문제가 된다면 인스펙터 초기화를 확인할 것.

    // Property
    public PlayerStats Stats { get => stats; }

    private void Start()
    {
        tileMaker = TileMaker.Instance;
    }

    // Animation
    public void PlayIdleAnimation()
    {
        FSM.animator.SetTrigger("Idle");
    }
    public void PlayHitAnimation()
    {
        SoundManager.Instance.Play(SoundType.Se_Character_hitted);
        FSM.animator.SetTrigger("Hit");
    }

    public void PlayDeadAnimation()
    {
        FSM.animator.SetTrigger("Dead");
    }

    public void PlayWinAnimation()
    {
        FSM.animator.SetTrigger("Win");
    }

    // 자신의 스킬 사용
    public void TurnInit(ActionType action, bool isDrag)
    {
        FSM.ChangeState(CharacterBattleState.Action); // Action Init에서 공격 애니메이션 재생 시작
        manager.IsDuringPlayerAction = true;

        if (action == ActionType.Skill)
        {
            //스킬사용
            StartCoroutine(CoActionCommand(isDrag));
        }
        else
        {
            //아이템사용
        }
    }

    private IEnumerator CoActionCommand(bool isDrag)
    {
        // 입력 방식 갱신
        manager.inputLink.isLastInputDrag = isDrag;

        // 스킵버튼 잠시 비활성화
        manager.uiLink.turnSkipButton.interactable = false;

        //스킬 범위 바닥 타일 표시
        var tiles = tileMaker.GetSkillRangedTiles(command.target, command.skill.SkillTableElem.range);
        foreach (var tile in tiles)
        {
            if(isDrag)
                tile.ConfirmAsTarget(command.type, manager.dragLink.lastDragWorldPos, command.skill.SkillTableElem.range);
            else
                tile.ConfirmAsTarget(command.type, tileMaker.LastClickPos, command.skill.SkillTableElem.range);
        }
        tileMaker.SetAllTileSoftClear();

        if (command.skill.SkillTableElem.id == manager.costLink.skillID_chargeOil) // 오일 충전
        {
            // 랜턴 내려놓기
            manager.directingLink.LandDownLantern();

            // 충전 애니 끝나고 PlayerAction의 MotionEnd 켜질 때 까지 기다리기.
            var playerActionState = FSM.GetState(CharacterBattleState.Action) as PlayerAction;
            yield return new WaitUntil(() => playerActionState.isAttackMotionEnd);
            playerActionState.isAttackMotionEnd = false;
            yield return new WaitForSeconds(1.5f);

            // 랜턴 다시 들기
            manager.directingLink.HoldLantern();
        }
        else
        {
            //몬스터 맞을 준비. 콜라이더 켜기
            manager.waveLink.SetAllMonsterInfoColliderEnable(false);
            var monsterList = TileMaker.Instance.GetTargetList(command.target, command.skill.SkillTableElem.range, isDrag);
            var neverChangeMonsterList = new List<MonsterUnit>(monsterList);
            foreach (var target in neverChangeMonsterList)
            {
                target.triggerLinker.EnableHitTrigger();
            }

            //몬스터 맞고 PlayerAction의 MotionEnd 켜질 때 까지 기다리기.
            var playerActionState = FSM.GetState(CharacterBattleState.Action) as PlayerAction;
            yield return new WaitUntil(() => playerActionState.isAttackMotionEnd);
            playerActionState.isAttackMotionEnd = false;

            // 모든타겟 OnAttacked 실행 -> 이때, OnAttacked에 시간이 걸리는 동작이 필요할경우 기다렸다 다음 진행하는 방식 고려
            if (command.skill.SkillTableElem.id != manager.costLink.skillID_focusAttack) // 집중공격아닐 때
            {
                foreach (var target in neverChangeMonsterList)
                {
                    target.PlayHitAnimation();
                    target.OnAttacked(command);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }

        // 소모 값 소모
        if (playerType == PlayerType.Boy)
        {
            manager.costLink.CostArrow(command.skill);
        }
        else
        {
            manager.costLink.CostLanternOrOil(command.skill);
        }
        BottomUIManager.Instance.UpdateCostInfo();

        // 발판색 삭제 및 배틀상태전환 및 일일히 승리 확인.
        foreach (var tile in tiles)
            tile.CancleConfirmTarget(playerType);

        FSM.ChangeState(CharacterBattleState.Idle); // 이 unit의 상태가 바뀌면 배틀상태의 업데이트에서 체크하다가 다음진행
        if(!manager.isTutorial || (manager.isTutorial && !manager.tutorial.lockSkillButtonClick))
            manager.uiLink.turnSkipButton.interactable = true;
        manager.EndOfPlayerAction();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {stats.Pos} Tile! ");
        TileMaker.Instance.LastDropPos = stats.Pos;
    }
}
