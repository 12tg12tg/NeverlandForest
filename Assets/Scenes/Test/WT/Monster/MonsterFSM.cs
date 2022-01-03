using UnityEngine;

public enum MonsterState
{
    Idle,
    Patrol,
    Attack,
    Death
}

public class MonsterFSM : FSM<MonsterState>
{
    private MonsterState curstate;
    public float startTime;
    public MonsterStats stats;
  
    public void Awake()
    {
        //AddState(MonsterState.Idle, new Monster_IdleState());
        //AddState(MonsterState.Patrol, new Monster_PatrolState());
        //AddState(MonsterState.Attack, new Monster_AttackState());
        //AddState(MonsterState.Death, new Monster_DeathState());

        //curstate = MonsterState.Patrol;
        //SetState(curstate);

    }
    public void Start()
    {
        //stats = gameObject.GetComponent<MonsterStats>();
        //stats.Init(stats.tilePos);
    }
    public override void Update()
    {
        //base.Update();
    }
    public override void FixedUpdate()
    {
    }
    public override void LateUpdate()
    {
    }

    public void OnGUI()
    {
        
    }

}
