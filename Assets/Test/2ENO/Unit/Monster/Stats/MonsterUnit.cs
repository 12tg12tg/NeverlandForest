using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Idle, Attack, Move
}

public abstract class MonsterUnit : UnitBase, IAttackable
{
    // Vars
    private int sheild;
    private int speed;
    private MonsterType type;
    protected MonsterTableElem baseElem;
    public MonsterCommand curCommand;

    // Property
    public int Sheild { get => sheild; set => sheild = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool IsBind { get; set; }
    public MonsterState State { get; set; }
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

    public MonsterCommand SetActionCommand(Vector2 targetPos)
    {
        var command = new MonsterCommand(this);
        switch (type)
        {
            case MonsterType.Near:
                if(targetPos.y + 1 < Pos.y)
                    command.actionType = MonsterActionType.Move;
                else
                    command.actionType = MonsterActionType.Attack;
                break;
            case MonsterType.Far:
                if (targetPos.y + 2 < Pos.y)
                    command.actionType = MonsterActionType.Move;
                else
                    command.actionType = MonsterActionType.Attack;
                break;
        }
        if (IsBind)
            command.actionType = MonsterActionType.None;
        command.target = targetPos;
        curCommand = command;
        return command;
    }

    public abstract void PlayAttackAnimation();
    public abstract void PlayDeadAnimation();
    public abstract void PlayHitAnimation();
    public abstract void TargetAttack();
    public abstract void Move();

}
