using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCommand : BattleCommand
{
    public MonsterUnit attacker;
    public Vector2 target;
    public MonsterActionType actionType;
    public int Ordering { get; set; }

    public MonsterCommand(MonsterUnit mUnit)
    {
        attacker = mUnit;
        Ordering = mUnit.Speed;
    }
    public void Clear()
    {
        target = Vector2.zero;
        actionType = MonsterActionType.None;
    }
}