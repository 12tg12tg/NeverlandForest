using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoomCtrl : MonoBehaviour
{
    public GameObject SpawnPos;
    public GameObject endPos;

    public DungeonRoom curRoom;

    void Start()
    {
        var childEnd = gameObject.GetComponentsInChildren<EndPos>();

        for (int i = 0; i < childEnd.Length; i++)
        {
            if (i == childEnd.Length - 1)
            {
                childEnd[i].isLastPos = true;
            }
        }
    }
}
