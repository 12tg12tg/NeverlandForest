using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPlayerSkill : DataSkill
{
    public DataPlayerSkill(DataTableElemBase skillElem)
    {
        skillTableElem = skillElem;
    }

    public PlayerSkillTableElem SkillTableElem
    {
        get { return skillTableElem as PlayerSkillTableElem; }
    }

    /*부모에서 쿨타임, 턴제한, 횟수제한, Invoke 등 구현하기*/
}
