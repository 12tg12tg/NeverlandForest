using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUnit : UnitBase, IAttackable
{
    private int sheild;
    public int Sheild { get => sheild; set => sheild = value; }
    private int speed;
    public int Speed { get => speed; set => speed = value; }

    public void OnAttacked(UnitBase attacker)
    {
        var playerStats = attacker as PlayerStats;
        Debug.Log($"{Pos} ���Ͱ� {playerStats.controller.playerType}���� {attacker.Atk}�� ���ظ� �޴�.");
        Hp -= attacker.Atk;
        Debug.Log($"{Hp + attacker.Atk} -> {Hp}");
    }

    /*�ൿ�����Լ� ��ȯ���� MonsterCommand*/
}
