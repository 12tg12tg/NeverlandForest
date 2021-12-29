using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBattleState
{
    Idle,
    Death,
    AttackReady,
}

// 캐릭터 체력상태, 현재 턴 진행상태, 유저 공격 명령 등을 체크하고 해당 상태에 맞게 상태 변경

public class PlayerBattleState : FSM<CharacterBattleState>
{
    private Animator playerAnimation;
    private PlayerStats playerStat;

    private void Start()
    {
        playerStat = gameObject.GetComponent<PlayerStats>();
        playerAnimation = gameObject.GetComponent<Animator>();

        var attackReady = new PlayerAttackReady();
        var idleState = new PlayerIdle();
        var deathState = new PlayerDeath();

        attackReady.SetPlayerAnimation(playerAnimation);
        attackReady.SetPlayerStat(playerStat);

        AddState(CharacterBattleState.AttackReady, attackReady);
        AddState(CharacterBattleState.Idle, idleState);
        AddState(CharacterBattleState.Death, deathState);
    }

}
