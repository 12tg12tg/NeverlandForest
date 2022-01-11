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
    public AnimationFunc aniFunc;
    public Animator animator;
    public PlayerBattleController controller;

    private void Start()
    {
        var attackReady = new PlayerAction(controller, animator);
        var idleState = new PlayerIdle(controller, animator);
        var deathState = new PlayerDeath(controller, animator);

        AddState(CharacterBattleState.Action, attackReady);
        AddState(CharacterBattleState.Idle, idleState);
        AddState(CharacterBattleState.Death, deathState);

        aniFunc.actionState = attackReady;
    }

    public override void Update()
    {
        base.Update();
    }
}
