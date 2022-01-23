using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSystem : MonoBehaviour
{
    public GameObject mainObj;
    public GameObject huntPrefab;


    public void Hunt()
    {
        huntPrefab.SetActive(true);
    }
}
