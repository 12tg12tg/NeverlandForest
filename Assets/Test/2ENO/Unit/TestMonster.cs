using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : UnitBase, IAttackable
{
    public void OnAttacked(UnitBase attacker)
    {
        Debug.Log($"{Pos} 위치의 몬스터가 공격받았습니다, 체력{Hp}");
        Hp -= attacker.Atk;
        Debug.Log($"공격받은 후 체력 {Hp}");
    }

    void Start()
    {
        Hp = 1000;
    }

    void Update()
    {
    }
}
