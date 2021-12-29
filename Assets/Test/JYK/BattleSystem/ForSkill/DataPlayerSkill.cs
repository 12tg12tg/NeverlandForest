using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataPlayerSkill : DataSkill
{
    public PlayerSkillTableElem SkillTableElem
    {
        get { return skillTableElem as PlayerSkillTableElem; }
    }

    /*부모에서 쿨타임, 턴제한, 횟수제한, Invoke 등 구현하기*/

    //public UnityEvent

    // 임시추가
    public bool CheckSkill(PlayerStats player)
    {
        foreach(var skill in player.SkillList)
        {
            if (skill.SkillTableElem.name == SkillTableElem.name)
                return true;
        }
        return false;
    }

    // 임시
    public int SkillDamage(UnitBase attacker)
    {
        return (attacker.Atk + SkillTableElem.damage);
    }
}
