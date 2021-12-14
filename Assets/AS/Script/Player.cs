using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public CreateConsumScriptableObject consumableObj;
    public CreateArmorScriptableObject armorObj;
    public Text text;
    public int hp;
    public int mp;
    public string armorName;
    public string consumName;
    public void OnGUI()
    {
        //prefab.dicScriptableObj.OnBeforeSerialize();
        var consum = consumableObj.dicConsumObj["CON_0001"];
        var armor = armorObj.dicArmorObj["DEF_0001"];
        hp = consum.hp + armor.stat_con;
        mp = consum.mp;
        consumName = consum.name;
        armorName = armor.name;
        text.text = $"{hp}\n{mp}\n{consumName}\n{armorName}";
    }
}
