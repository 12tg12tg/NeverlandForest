using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("MonsterUnit�� ����ϼ���.")]
public class TestMonster : UnitBase, IAttackable
{
    public void OnAttacked(UnitBase attacker)
    {
        //Debug.Log($"{Pos} ���Ͱ� {attacker.Atk}�� ���ظ� �޴�.");
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
