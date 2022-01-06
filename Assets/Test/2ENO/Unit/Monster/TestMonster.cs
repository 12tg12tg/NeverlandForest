using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("MonsterUnit을 사용하세요.")]
public class TestMonster : UnitBase, IAttackable
{
    public void OnAttacked(BattleCommand attacker)
    {
        //Debug.Log($"{Pos} 몬스터가 {attacker.Atk}의 피해를 받다.");
        //Hp -= attacker.Atk;
        //Debug.Log($"{Hp + attacker.Atk} -> {Hp}");
    }

    //void Start()
    //{
    //    Hp = 1000;
    //}

    //void Update()
    //{
    //}
}
