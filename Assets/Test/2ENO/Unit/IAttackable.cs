using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public abstract void OnAttacked(UnitBase attacker);
}
