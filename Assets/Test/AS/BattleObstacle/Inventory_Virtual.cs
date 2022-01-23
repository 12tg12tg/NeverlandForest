using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Virtual : MonoBehaviour
{
    public static Inventory_Virtual instance;
    public GameObject[] obstaclePrefab;
    public bool isLasso = false;

    private void Awake()
    {
        instance = this;
    }
}
