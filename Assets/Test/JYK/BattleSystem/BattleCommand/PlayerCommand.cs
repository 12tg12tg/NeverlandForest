using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCommand : BattleCommand
{
    private bool isUpdate;
    public PlayerBattleController attacker;
    public Vector2 target;
    public DataPlayerSkill skill;
    public DataAllItem item;
    public ActionType actionType;
    public PlayerType type;
    public PlayerCommand(PlayerBattleController pUnit, PlayerType type)
    {
        attacker = pUnit;
        this.type = type;
    }
    public bool IsUpdated { get => isUpdate; }

    public void Clear()
    {
        isUpdate = false;
        target = Vector2.zero;
        skill = null;
        item = null;
    }

    public void Create(Vector2 target, DataPlayerSkill skill)
    {
        if (isUpdate)
            Clear();
        this.target = target;
        this.skill = skill;
        isUpdate = true;
        actionType = ActionType.Skill;
    }

    public void Create(Vector2 target, DataAllItem item)
    {
        if (isUpdate)
            Clear();
        this.target = target;
        this.item = item;
        isUpdate = true;
        actionType = ActionType.Item;
    }
}