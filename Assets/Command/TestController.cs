using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public enum Direction { Left =-1, RIght =1}

    private float distance = 1f;

    public void Turn(Direction direction)
    {
        if (direction == Direction.Left)
            transform.Translate(Vector3.left * distance);

        if (direction == Direction.RIght)
            transform.Translate(Vector3.right * distance);
    }
    public void ResetPosition()
    {
        transform.position = new Vector3(0f, 0f, 0f);
    }
}
