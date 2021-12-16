using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataWeapon : DataItem
{
    public DataCharacter own;

    public WeaponTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as WeaponTableElem;
        }
    }
}
