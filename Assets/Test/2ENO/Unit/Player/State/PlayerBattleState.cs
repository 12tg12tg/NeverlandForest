using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBattleState
{
    Idle,
    Death,
    AttackReady,
}

// ĳ���� ü�»���, ���� �� �������, ���� ���� ��� ���� üũ�ϰ� �ش� ���¿� �°� ���� ����

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
