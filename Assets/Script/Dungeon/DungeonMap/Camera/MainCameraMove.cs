using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraMove : MonoBehaviour
{
    public PlayerDungeonUnit player;
    void Start()
    {
        var cam = GetComponent<Camera>();
        cam.fieldOfView = 20.2f;
    }

    void Update()
    {
        var targetPos = new Vector3(player.transform.position.x, player.transform.position.y + 3.77f, player.transform.position.z - 28.46f);
        var targetRotate = new Vector3(4.876f, 0, 0f);
        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(targetRotate);
    }
}
