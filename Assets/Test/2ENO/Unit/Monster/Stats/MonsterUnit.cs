using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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


    public void OnAttacked(BattleCommand attacker)
    {
        var playerStats = attacker as PlayerCommand;
        var damage = playerStats.skill.SkillTableElem.damage;
        Debug.Log($"{Pos} ���Ͱ� {playerStats.type}���� {damage}�� ���ظ� �޴�.");
        Hp -= damage;
        Debug.Log($"{Hp + damage} -> {Hp}");
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
    public void Move()
    {
        State = MonsterState.Move;
        var moveCount = Random.Range(1, 4);
        Tiles fowardTile = null;
        int count = 0;
        while (!CurTile.TryGetFowardTile(out fowardTile, moveCount) && count < 200)
        {
            count++;
            moveCount = Random.Range(1, 4);
        }
        Debug.Log(count);
        BattleManager.Instance.PlaceUnitOnTile(fowardTile.index, this, () => State = MonsterState.Idle, true);
    }
    // ���� �ִϸ��̼� ������ �±� ����
    public void TargetAttack()
    {
        var list = TileMaker.Instance.UnitOnTile(curCommand.target);
        var targetList = list.Cast<PlayerBattleController>();
        foreach (var target in targetList)
        {
            target.Stats.OnAttacked(curCommand);
        }
        State = MonsterState.Idle;
    }

}
