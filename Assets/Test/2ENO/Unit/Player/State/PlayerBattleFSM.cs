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
