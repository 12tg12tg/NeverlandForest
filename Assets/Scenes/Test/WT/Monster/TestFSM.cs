using System;
using UnityEngine;

[Obsolete("Do not call this method.")]
public enum TestMonsterState
{
    Idle,
    Patrol,
    Attack,
    Death
}

[Obsolete("Do not call this method.")]

public class TestFSM : FSM<TestMonsterState>
{
    private TestMonsterState curstate;
    public float startTime;
    public TestMonsterStats stats;
  
    public void Awake()
    {
        AddState(TestMonsterState.Idle, new Monster_IdleState());
        AddState(TestMonsterState.Patrol, new Monster_PatrolState());
        AddState(TestMonsterState.Attack, new Monster_AttackState());
        AddState(TestMonsterState.Death, new Monster_DeathState());

        curstate = TestMonsterState.Patrol;
        SetState(curstate);

    }
    public void Start()
    {
        stats = gameObject.GetComponent<TestMonsterStats>();
        stats.Init(stats.tilePos);
    }
    public override void Update()
    {
        base.Update();
    }

    public void OnGUI()
    {
        
    }

}
