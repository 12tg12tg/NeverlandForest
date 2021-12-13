using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CreateScriptableObject prefab;
    public int hp;
    public int mp;
    private void Start()
    {
        Debug.Log(prefab.dicScriptableObj.Count); // 0

        foreach (var elem in prefab.dicScriptableObj)
        {
            Debug.Log($"{elem.Key} {elem.Value}");
        }
    }
    public void OnGUI()
    {
        var elem = prefab.dicScriptableObj["CON_0001"] as ItemTableElem;
        hp = elem.hp;
        mp = elem.mp;
    }
}
