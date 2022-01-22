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

    public override IEnumerator CoSkillCoroutine()
    {
        yield return null;
    }

    /*부모에서 턴제한, Invoke 등 구현하기*/


}
