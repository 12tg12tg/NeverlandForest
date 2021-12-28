using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPlayerSkill : DataSkill
{
    public PlayerSkillTableElem SkillTableElem
    {
        get { return SkillTableElem as PlayerSkillTableElem; }
    }

    /*부모에서 쿨타임, 턴제한, 횟수제한, Invoke 등 구현하기*/
}
