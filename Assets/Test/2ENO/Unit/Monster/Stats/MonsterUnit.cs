using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterUnit : UnitBase, IAttackable
{
    // Vars
    private int sheild;
    private int speed;
    private MonsterType type;
    protected MonsterTableElem baseElem;

    // Property
    public int Sheild { get => sheild; set => sheild = value; }
    public int Speed { get => speed; set => speed = value; }
    public MonsterType Type { get => type; }
    public MonsterTableElem BaseElem { get => baseElem; }

    public void OnAttacked(UnitBase attacker)
    {
        var playerStats = attacker as PlayerStats;
        Debug.Log($"{Pos} ���Ͱ� {playerStats.controller.playerType}���� {attacker.Atk}�� ���ظ� �޴�.");
        Hp -= attacker.Atk;
        Debug.Log($"{Hp + attacker.Atk} -> {Hp}");
    }

    public void Init(MonsterTableElem elem)
    {
        Hp = elem.hp;
        Atk = elem.atk;
        sheild = elem.sheild;
        speed = elem.speed;
        type = elem.type;
    }
    /*�ൿ�����Լ� ��ȯ���� MonsterCommand*/

    public abstract void PlayAttackAnimation();
    public abstract void PlayDeadAnimation();
    public abstract void PlayHitAnimation();
}
