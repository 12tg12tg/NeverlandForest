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

    /*�θ𿡼� ��Ÿ��, ������, Ƚ������, Invoke �� �����ϱ�*/

    //public UnityEvent

    // �ӽ��߰�
    public bool CheckSkill(PlayerStats player)
    {
        foreach(var skill in player.SkillList)
        {
            if (skill.SkillTableElem.name == SkillTableElem.name)
                return true;
        }
        return false;
    }

    // �ӽ�
    public int SkillDamage(UnitBase attacker)
    {
        return (attacker.Atk + SkillTableElem.damage);
    }
}
