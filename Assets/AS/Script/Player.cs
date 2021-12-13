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
            Debug.Log(elem.Key);
        }
    }
    public void OnGUI()
    {
        


        //hp = (prefab.dicScriptableObj["CON_0001"] as ItemTableElem).hp;
        //mp = (prefab.dicScriptableObj["CON_0001"] as ItemTableElem).mp;
    }
}
