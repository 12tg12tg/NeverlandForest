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

    /*�θ𿡼� ��Ÿ��, ������, Ƚ������, Invoke �� �����ϱ�*/
}
