using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUnit : Observer, IAttackable
{
    public int shield;
    public MonsterStats monsterStat;
    public MonsterFSM monsterState;
    public override void Notify(ObservablePublisher publisher)
    {
    }

    public void OnAttacked(UnitBase attacker)
    {
    }

    void Start()
    {
        monsterStat = gameObject.GetComponent<MonsterStats>();
        monsterState = gameObject.GetComponent<MonsterFSM>();
    }

    void Update()
    {
    }
}
