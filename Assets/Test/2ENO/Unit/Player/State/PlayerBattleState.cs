using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBattleState
{
    Idle,
    Death,
    Action,
}

// 캐릭터 체력상태, 현재 턴 진행상태, 유저 공격 명령 등을 체크하고 해당 상태에 맞게 상태 변경

public class PlayerBattleState : FSM<CharacterBattleState>
{
    private Animator playerAnimation;
    private PlayerBattleUnit playerUnit;


    private void Start()
    {
        playerUnit = gameObject.GetComponent<PlayerBattleUnit>();
        playerAnimation = gameObject.GetComponent<Animator>();

        var attackReady = new PlayerAction();
        var idleState = new PlayerIdle();
        var deathState = new PlayerDeath();

        attackReady.SetPlayerAnimation(playerAnimation);
        attackReady.SetPlayerUnit(playerUnit);
        idleState.SetPlayerAnimation(playerAnimation);
        idleState.SetPlayerUnit(playerUnit);

        AddState(CharacterBattleState.Action, attackReady);
        AddState(CharacterBattleState.Idle, idleState);
        AddState(CharacterBattleState.Death, deathState);
    }

    public override void Update()
    {
        base.Update();
    }
}
