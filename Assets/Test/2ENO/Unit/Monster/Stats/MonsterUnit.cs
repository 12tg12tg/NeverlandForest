using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterUnit : UnitBase, IAttackable
{
    private int sheild;
    public int Sheild { get => sheild; set => sheild = value; }
    private int speed;
    public int Speed { get => speed; set => speed = value; }
    private MonsterType type;
    public MonsterType Type { get => type; }
    public void OnAttacked(UnitBase attacker)
    {
        var playerStats = attacker as PlayerStats;
        Debug.Log($"{Pos} 몬스터가 {playerStats.controller.playerType}에게 {attacker.Atk}의 피해를 받다.");
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
    /*행동결정함수 반환값은 MonsterCommand*/

    public abstract void PlayAttackAnimation();
    public abstract void PlayDeadAnimation();
    public abstract void PlayHitAnimation();
}
