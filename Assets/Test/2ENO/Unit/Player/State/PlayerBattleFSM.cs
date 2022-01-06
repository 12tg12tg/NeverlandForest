using UnityEngine;

public enum CharacterBattleState
{
    Idle,
    Death,
    Action,
}

// ĳ���� ü�»���, ���� �� �������, ���� ���� ��� ���� üũ�ϰ� �ش� ���¿� �°� ���� ����

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
