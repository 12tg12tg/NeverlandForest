using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayer : MonoBehaviour
{
    public CreateConsumScriptableObject consumableObj;
    public CreateArmorScriptableObject armorObj;
    public int hp;
    public int mp;
    public string armorName;
    public string consumName;
    public SerializeDictionary<string, ConsumableTableElem> aaa;

    private void Start()
    {
        DataTableManager.Init();
        var sdic = DataTableManager.dic.Values;

        var ddd = DataTableManager.GetTable<CreateConsumScriptableObject>().GetData<ConsumableTableElem>("");
        
        
        
    }
    public void OnGUI()
    {
        var consum = consumableObj.dicConsumObj["CON_0001"];
        var armor = armorObj.dicArmorObj["DEF_0001"];
        hp = consum.hp + armor.stat_con;
        mp = consum.mp;
        consumName = consum.name;
        armorName = armor.name;
    }
}
