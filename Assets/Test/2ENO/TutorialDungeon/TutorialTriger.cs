using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag is "Player")
        {
            // ���丮 é�� ������!
            Debug.Log("Ʈ���� �߻�!");
        }
    }
}
