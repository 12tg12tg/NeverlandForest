using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isLastPos;

    private DungeonSystem dungeonSystem;

    void Start()
    {
        dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(isLastPos)
        {
            dungeonSystem.ChangeRoomEvent(true);
        }
        else
        {
            dungeonSystem.ChangeRoomEvent(false);
        }
    }

    //public Vector3 getPosition()
    //{
    //    return transform.position;
    //}
}
