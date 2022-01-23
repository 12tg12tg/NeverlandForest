using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraMove : MonoBehaviour
{
    public PlayerDungeonUnit player;
    void Start()
    {
        
    }

    void Update()
    {
        var targetPos = new Vector3(player.transform.position.x, player.transform.position.y + 9f, player.transform.position.z - 13f);
        var targetRotate = new Vector3(30f, 0, 0f);
        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(targetRotate);
    }
}
