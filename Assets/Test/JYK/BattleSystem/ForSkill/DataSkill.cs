using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataSkill
{
    public int itemId;
    protected DataTableElemBase skillTableElem;
    public bool skillDone;


    /*턴제한, Invoke 등 구현하기*/
    public abstract IEnumerator CoSkillCoroutine();

}

