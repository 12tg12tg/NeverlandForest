using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 사용자가 어떤 입력을 했을 때, 그에 해당하는 정보를 넘겨주거나, 동작을 실행시키거나 하는 클래스
// 배틀 시스템에 필요한 정보를 넘겨주고, 배틀 시스템에서 캐릭터 유닛의 상태에 따라 턴 넘겨주고 등등
// 배틀 시스템이 실시간으로 플레이어 캐릭터 상태를 체크하는게 아닌 캐릭터에서 자신의 상태변화를 배틀에 알려주는방식

public class PlayerBattleController : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    /* 캐릭터 보유 스킬, 능력치(스텟, 버프디버프 여부 등), 착용 장비 등에 대한 정보 및 값 수정
    캐릭터 전투상태 정보 ( 공격전 준비, 공격후 대기, 죽음 ) */


    // Instance
    public BattleManager manager;
    private List<UnitBase> targetUnit;

    // Component
    public PlayerStats stats; 
    public PlayerBattleFSM FSM;

    // Vars
    public PlayerCommand command;
    public PlayerType playerType;    // 혹시 애니메이션이 문제가 된다면 인스펙터 초기화를 확인할 것.

    // Property
    public PlayerStats Stats { get => stats; }

    public void TurnInit()
    {
        FSM.ChangeState(CharacterBattleState.Action);
        manager.IsDuringPlayerAction = true;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager.FSM.curState != BattleState.Player)
            return;

        if (command.IsUpdated || manager.IsDuringPlayerAction)
            return;

        // 캐릭터 클릭했을 시 수행할 동작들
        manager.OpenSkillUI(playerType);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {stats.Pos} Tile! ");
        TileMaker.Instance.LastDropPos = stats.Pos;
    }
}
