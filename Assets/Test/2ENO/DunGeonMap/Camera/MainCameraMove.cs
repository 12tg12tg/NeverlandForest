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
        var targetPos = new Vector3(player.transform.position.x + 15f, player.transform.position.y + 10f, player.transform.position.z + 2.3f);
        var targetRotate = new Vector3(28f, 270f, 0f);
        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(targetRotate);
    }
}
