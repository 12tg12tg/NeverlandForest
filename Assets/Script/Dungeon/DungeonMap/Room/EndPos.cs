using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isLastPos;
    public int roomNumber;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag is "Player")
        {
            var player = other.GetComponent<PlayerDungeonUnit>();
            if (isLastPos)
            {
                player.CurRoomNumber = 0;
                GameManager.Manager.Production.FadeIn(() => DungeonSystem.Instance.ChangeRoomEvent(true, true));
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
