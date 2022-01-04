using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBattleState
{
    Idle,
    Death,
    Action,
}

// ĳ���� ü�»���, ���� �� �������, ���� ���� ��� ���� üũ�ϰ� �ش� ���¿� �°� ���� ����

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
