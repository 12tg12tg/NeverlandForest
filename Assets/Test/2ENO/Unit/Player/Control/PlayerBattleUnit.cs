using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 사용자가 어떤 입력을 했을 때, 그에 해당하는 정보를 넘겨주거나, 동작을 실행시키거나 하는 클래스

// 배틀 시스템에 필요한 정보를 넘겨주고?, 배틀 시스템에서 캐릭터 유닛의 상태에 따라 턴 넘겨주고 등등

// 배틀 시스템이 실시간으로 플레이어 캐릭터 상태를 체크하는게 아닌 캐릭터에서 자신의 상태변화를 배틀에 알려주는방식

public class PlayerBattleUnit : UnitBase, IPointerClickHandler
{
    private List<UnitBase> targetUnit;

    // 캐릭터 전투상태 정보 ( 공격전 준비, 공격후 대기, 죽음 )
    private PlayerBattleState playerState;

    // 캐릭터 보유 스킬, 능력치(스텟, 버프디버프 여부 등), 착용 장비 등에 대한 정보 및 값 수정
    private PlayerStats playerStat;

    public BattleManager battleSystem;

    private void Start()
    {
        playerState = gameObject.GetComponent<PlayerBattleState>();
        playerStat = gameObject.GetComponent<PlayerStats>();
    }

    public void TurnInit()
    {
        // 플레이어 턴이 되었을 때, 공격 준비 상태 등 상태 초기화
        // 턴 시작에 필요한 부분들 정의
    }

    public void AttackTarget(List<UnitBase> targetList)
    {
        // 배틀 시스템으로부터 타겟 리스트를 받아와서 해당 타겟들에 현재 사용한 스킬을 적용
    }

    public void OnSkillUse(DataPlayerSkill playerSkill)
    {
        // 사용자가 스킬슬롯UI 클릭으로 스킬 사용시 해당하는 스킬정보를 받아오는 메소드

        // 이곳에서 사용한 스킬 정보에 대한 애니메이션 및 이펙트 실행 명령
        // 적 타겟 유닛에 대해 공격
        // 애니메이션 및 이펙트가 모두 종료되면 공격 종료로 판단해서 플레이어 상태 바꾼다
    }

    public void SendAttackEndMessage()
    {
        // 배틀시스템에 현재 캐릭터의 공격이 모두 종료되었음을 알려주는
    }

    public void SendSkillUseMessage()
    {
        // 배틀 시스템에 현재 사용한 스킬정보를 보내서 해당 스킬의 쿨타임 등을 적용하거나 할수있다.
    }

    public void SendSelectCharacterMessage()
    {
        // 캐릭터 선택시 선택된 캐릭터 정보 배틀 시스템에 보내주기
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerState.curState != CharacterBattleState.AttackReady)
            return;
        // 캐릭터 클릭했을 시 수행할 동작들

        // 배틀 시스템에 현재 선택된 캐릭터 정보 보내주기 PlayerStat
        // 선택된 캐릭터의 정보에 따라 스킬슬롯UI에 사용가능한 스킬을 배틀시스템이 띄워주고
        // 유저가 스킬을 사용하거나 할수 있음
    }
}
