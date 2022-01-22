using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataSkill
{
    public int itemId;
    protected DataTableElemBase skillTableElem;
    public bool skillDone;


    /*������, Invoke �� �����ϱ�*/
    public abstract IEnumerator CoSkillCoroutine();

}

