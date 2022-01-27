using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapUICamera : MonoBehaviour
{
    private Vector3 startPos;
    private float startX;

    private readonly float distance = 50f;
    private readonly float maxDistance = 200f;

    private void Awake()
    {
        startPos = transform.position;
        startX = startPos.x - distance * 0.3f;
    }
    private void Update()
    {
        var touch = MultiTouch.Instance;

        if (touch.TouchCount > 0)
        {
            var pos = Camera.main.ScreenToViewportPoint(touch.PrimaryStartPos - touch.PrimaryPos);

            transform.position = new Vector3(pos.x, 0f, 0f) * distance + startPos;
        }
        else
        {
            if (transform.position.x < startX)
            {
                transform.position = new Vector3(startX, transform.position.y, transform.position.z);
            }
            else if (transform.position.x > startX + maxDistance)
            {
                transform.position = new Vector3(startX + maxDistance, transform.position.y, transform.position.z);
            }
            startPos = transform.position;
        }
    }
}
