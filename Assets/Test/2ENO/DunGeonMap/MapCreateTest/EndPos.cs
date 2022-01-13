using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isLastPos;
    public int roomNumber;
    private DungeonSystem dungeonSystem;

    void Start()
    {
    }
    private void OnEnable()
    {
        dungeonSystem = GameObject.FindWithTag("DungeonSystem").GetComponent<DungeonSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(dungeonSystem == null)
        {
            Debug.Log("dungeonSystem 변수 초기화 x 오류발생!! 코드수정필요");
            return;
        }

        if (other.tag is "Player")
        {
            var player = other.GetComponent<PlayerDungeonUnit>();
            if (isLastPos)
            {
                dungeonSystem.ChangeRoomEvent(true, true);
                player.CurRoomNumber = 0;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (dungeonSystem == null)
        {
            Debug.Log("dungeonSystem 변수 초기화 x 오류발생!! 코드수정필요");
            return;
        }

        if (other.tag is "Player")
        {
            var player = other.GetComponent<PlayerDungeonUnit>();
            if (!isLastPos)
            {
                bool isGoForward = (roomNumber - player.CurRoomNumber >= 0) ? true : false;
                if(player.transform.position.z > transform.position.z && isGoForward)
                {
                    player.CurRoomNumber = roomNumber + 1;
                    dungeonSystem.ChangeRoomEvent(false, isGoForward);
                }
                else if(player.transform.position.z < transform.position.z && !isGoForward)
                {
                    player.CurRoomNumber = roomNumber;
                    dungeonSystem.ChangeRoomEvent(false, isGoForward);
                }    
            }
        }
    }
}
