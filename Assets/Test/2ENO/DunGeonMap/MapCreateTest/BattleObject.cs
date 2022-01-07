using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag is "Player")
        {
            SceneManager.LoadScene("JYK_Test_Battle");
        }
    }
}
