using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : UnitBase, IAttackable
{
    public void OnAttacked(UnitBase attacker)
    {
        Debug.Log($"{Pos} ��ġ�� ���Ͱ� ���ݹ޾ҽ��ϴ�, ü��{Hp}");
        Hp -= attacker.Atk;
        Debug.Log($"���ݹ��� �� ü�� {Hp}");
    }

    void Start()
    {
        Hp = 1000;
    }

    void Update()
    {
    }
}
