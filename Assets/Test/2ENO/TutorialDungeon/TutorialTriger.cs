using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag is "Player")
        {
            // 스토리 챕터 마지막!
            DungeonSystem.Instance.mainRoomTutorial.NextTutorialStep();
        }
    }
}
