using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isLastPos;
    public int roomNumber;

    void Start()
    {
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag is "Player")
        {
            var player = other.GetComponent<PlayerDungeonUnit>();
            if (isLastPos)
            {
                DungeonSystem.Instance.ChangeRoomEvent(true, true);
                player.CurRoomNumber = 0;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag is "Player")
        {
            var player = other.GetComponent<PlayerDungeonUnit>();
            if (!isLastPos)
            {
                bool isGoForward = (roomNumber - player.CurRoomNumber >= 0) ? true : false;
                if(player.transform.position.x > transform.position.x && isGoForward)
                {
                    player.CurRoomNumber = roomNumber + 1;
                    DungeonSystem.Instance.ChangeRoomEvent(false, isGoForward);
                }
                else if(player.transform.position.x < transform.position.x && !isGoForward)
                {
                    player.CurRoomNumber = roomNumber;
                    DungeonSystem.Instance.ChangeRoomEvent(false, isGoForward);
                }    
            }
        }
    }
}
