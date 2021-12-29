using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture_v2 : MonoBehaviour
{
    public GameObject cube;
    private float cubleroateSpeed = 10.0f;
    private float count = 0f;
    private bool fingerIsDown = false;
    private Vector3 touchStart;
    private string touchPattern = string.Empty;
    private string touchPatternChain = string.Empty;
    private float deviationCheckDistance = 1.0f; //체크편차거리 5
    private Vector3 lastDeviationCheck = Vector3.zero; //마지막 편차 0
    private bool isclock = false;
    private bool iscounterclock = false;

    private string[] clockCircleChain = new string[2] { "32413", "34132" };
    private string[] counterCircleChain = new string[4] { "42314", "43142", "43241", "23142" };
    private string clcokString = string.Empty;
    private string counterclockString = string.Empty;
    public void Update()
    {
     
        // touch start
        if (!fingerIsDown && Input.GetMouseButton(0))
        {
            fingerIsDown = true;
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchStart.z = -1;
            lastDeviationCheck = touchStart;
        }
        // touch middle
        if (fingerIsDown && Input.GetMouseButton(0))
        {
            if (isclock && !iscounterclock)
            {
                cubleroateSpeed -= 0.3f * count;
            }
            else if (!isclock && iscounterclock)
            {
                cubleroateSpeed += 0.3f * count;
            }
            else 
            {
                count -= 0.05f;
            }

            var inputPosition = Input.mousePosition;
            inputPosition.z = 10f;
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
            //touchPattern = string.Empty;
           
            cube.transform.rotation = Quaternion.Euler(
                new Vector3(0f, 0f, cubleroateSpeed));

            foreach (var chain in clockCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;
                    Debug.Log("시계방향으로 돌고있다");
                    count += 0.1f;
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
                    count += 0.1f;
                    iscounterclock = true;
                    isclock = false;
                }
              
            }
        }

        /*if (clockCircleChain.FindAll((x)=>x.Equals(clcokString)).Count ==0)
        {
            clockCircleChain.Add(clcokString);
        }
        if (counterCircleChain.FindAll((x) => x.Equals(counterclockString)).Count == 0)
        {
            counterCircleChain.Add(counterclockString);
        }*/
        // touch end
        if (fingerIsDown && Input.GetMouseButtonUp(0))
        {
            touchPatternChain = string.Empty;
            touchPattern = string.Empty;
            count = 0f;
        }

        if (count > 20f) //회전하는 속도 조절.
        {
            count = 20f;
        }
        if (count<0f)
        {
            count = 0f;
        }


        if (touchPattern.Length>6 )
        {
            iscounterclock = false;
            isclock = false;
        }

    }
    void RecordPattern(string thisPattern)
    {
        if (touchPattern.Length ==0)
        {
            touchPattern += thisPattern;
        }
        else
        {
            Debug.Log($"if문이전에 touchPattern : {touchPattern}");
            if (touchPattern.Substring(touchPattern.Length - 1) != thisPattern) //마지막 인덱스와 비교해서 같지 않으면 더해라.
            {
                touchPattern += thisPattern;
                Debug.Log($"if문이후에 touchPattern : {touchPattern}");
            }
        }
    }
}
