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

public class PlayerBattleFSM : FSM<CharacterBattleState>
{
    public PlayerBattleUnit playerUnit;
    private Animator playerAnimation;
    private PlayerStats playerStat;

    private PlayerAttackReady attackReady;

    private void Start()
    {
        playerStat = gameObject.GetComponent<PlayerStats>();
        playerAnimation = gameObject.GetComponent<Animator>();
        playerUnit = gameObject.GetComponent<PlayerBattleUnit>();

        attackReady = new PlayerAttackReady();
        var idleState = new PlayerIdle();
        var deathState = new PlayerDeath();

        attackReady.SetPlayerAnimation(playerAnimation);
        attackReady.SetPlayerStat(playerStat);
        idleState.SetPlayerAnimation(playerAnimation);
        idleState.SetPlayerStat(playerStat);
        idleState.SetPlayerUnit(playerUnit);

        AddState(CharacterBattleState.AttackReady, attackReady);
        AddState(CharacterBattleState.Idle, idleState);
        AddState(CharacterBattleState.Death, deathState);
    }

    public void SkillAttack()
    {
        if (curState == CharacterBattleState.AttackReady)
            StartCoroutine(attackReady.AttackAnimation());
    }
}
