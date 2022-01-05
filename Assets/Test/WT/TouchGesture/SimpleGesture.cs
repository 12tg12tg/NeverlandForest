using UnityEngine;

public class SimpleGesture : MonoBehaviour
{
    public Transform target;
    float previous;
    float current;

    private void Update()
    {
        CircleGestureRotate();
    }
    private void CircleGestureRotate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previous = 0;
            current = 0;
        }

        if (Input.GetMouseButton(0))
        {
            previous = current;
            current = GetAngle(target.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (previous != 0.0f)
            {
                float diffrence = current - previous;
                target.Rotate(0, 0, diffrence);
            }
        }
    }

    public float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }
}
