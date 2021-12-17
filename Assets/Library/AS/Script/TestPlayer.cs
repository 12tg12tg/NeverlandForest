using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayer : MonoBehaviour
{
    public int hp;
    public int mp;
    public string armorName;
    public string consumName;

    public void OnGUI()
    {
        var consumData = DataTableManager.GetTable<ConsumableTable>().GetData<ConsumableTableElem>("CON_0001");
        hp = consumData.hp;
        mp = consumData.mp;
        consumName = consumData.name;
    }
}
