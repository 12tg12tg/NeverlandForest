using UnityEngine;

public enum CharacterBattleState
{
    Idle,
    Death,
    Action,
}

// 캐릭터 체력상태, 현재 턴 진행상태, 유저 공격 명령 등을 체크하고 해당 상태에 맞게 상태 변경

public class PlayerBattleFSM : FSM<CharacterBattleState>
{
    private Animator playerAnimation;
    private PlayerBattleController playerUnit;

    private void Awake()
    {
        playerUnit = gameObject.GetComponent<PlayerBattleController>();
        playerAnimation = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        var attackReady = new PlayerAction(playerUnit, playerAnimation);
        var idleState = new PlayerIdle(playerUnit, playerAnimation);
        var deathState = new PlayerDeath(playerUnit, playerAnimation);

        AddState(CharacterBattleState.Action, attackReady);
        AddState(CharacterBattleState.Idle, idleState);
        AddState(CharacterBattleState.Death, deathState);
    }

    public override void Update()
    {
        base.Update();
    }
}
