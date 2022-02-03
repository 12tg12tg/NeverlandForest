using UnityEngine;

public class CircleGesture : MonoBehaviour
{
    public Transform target;
    float previous;
    float current;
    Vector3 touchStart = Vector3.zero;
    private bool fingerIsDown = false;

    private string touchPattern = string.Empty;
    private string touchPatternChain = string.Empty;
    private float deviationCheckDistance = 1.0f; //체크편차거리 5
    private Vector3 lastDeviationCheck = Vector3.zero; //마지막 편차 0

    private int clockCount = 0;
    private int counterclockCount = 0;
    private bool isclock = false;
    private bool iscounterclock = false;

    private string[] clockCircleChain = new string[2] { "32413", "34132" };
    private string[] counterCircleChain = new string[4] { "42314", "43142", "43241", "23142" };
    private Vector3 inputPosition;
    private void Update()
    {
        CircleGestureRotate();
        // touch start
        if (!fingerIsDown && Input.GetMouseButton(0))
        {
            fingerIsDown = true;
            touchStart.z = -1;
            lastDeviationCheck = touchStart;
        }
        // touch middle
        if (fingerIsDown && Input.GetMouseButton(0))
        {
            Vector3 touchCurrent = Camera.main.ScreenToWorldPoint(inputPosition);
            float diffX = Mathf.Abs(touchCurrent.x - lastDeviationCheck.x);
            float diffY = Mathf.Abs(touchCurrent.y - lastDeviationCheck.y);
            bool deviated = false;

            if (diffX > deviationCheckDistance)
            {
                //left ->right
                if (touchCurrent.x > lastDeviationCheck.x)
                {
                    RecordPattern("1");
                    deviated = true;
                    touchPatternChain += touchPattern;

                }
                // right ->left
                if (touchCurrent.x < lastDeviationCheck.x)
                {
                    RecordPattern("2");
                    deviated = true;
                    touchPatternChain += touchPattern;

                }
            }
            if (diffY > deviationCheckDistance)
            {
                // TOP -> BOTTOM
                if (touchCurrent.y < lastDeviationCheck.y)
                {
                    RecordPattern("3");
                    deviated = true;
                    touchPatternChain += touchPattern;
                }
                // BOTTOM -> TOP
                if (touchCurrent.y > lastDeviationCheck.y)
                {
                    RecordPattern("4");
                    deviated = true;
                    touchPatternChain += touchPattern;
                }
            }
            if (deviated)
            {
                lastDeviationCheck = touchCurrent;
            }

            foreach (var chain in clockCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;
                    Debug.Log("시계방향으로 돌고있다");
                    clockCount++;
                    isclock = true;
                    iscounterclock = false;
                }
            }
            foreach (var chain in counterCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;
                    Debug.Log("반시계방향으로 돌고있다");
                    counterclockCount++;
                    isclock = false;
                    iscounterclock = true;
                }
            }
        }
        if (fingerIsDown && Input.GetMouseButtonUp(0))
        {
            fingerIsDown = false;
            touchPatternChain = string.Empty;
            touchPattern = string.Empty;
            isclock = false;
            iscounterclock = false;
        }
    }
    void RecordPattern(string thisPattern)
    {
        if (touchPattern.Length == 0)
        {
            touchPattern += thisPattern;
        }
        else
        {
            if (touchPattern.Substring(touchPattern.Length - 1) != thisPattern) //마지막 인덱스와 비교해서 같지 않으면 더해라.
            {
                touchPattern += thisPattern;
            }
        }
    }
    private void CircleGestureRotate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previous = 0;
            current = 0;
            touchStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            previous = current;
            inputPosition = Input.mousePosition;
            inputPosition.z = 10f;
            current = GetAngle(target.position, Camera.main.ScreenToWorldPoint(inputPosition));
            if (previous != 0.0f)
            {
                float diffrence = current - previous;
                if (isclock)
                {
                    target.Rotate(0, 0, diffrence);
                }
                else if (iscounterclock)
                {
                    target.Rotate(0, 0, -diffrence);
                }
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