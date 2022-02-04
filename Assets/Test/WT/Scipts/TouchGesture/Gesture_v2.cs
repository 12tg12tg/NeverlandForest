using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture_v2 : MonoBehaviour
{
    private float cubleroateSpeed = 10.0f;
    public bool fingerIsDown = false;
    private Vector3 touchStart;
    private string touchPattern = string.Empty;
    private string touchPatternChain = string.Empty;
    private float deviationCheckDistance = 1.0f; //체크편차거리 5
    private Vector3 lastDeviationCheck = Vector3.zero; //마지막 편차 0
    private bool isclock = false;
    private bool iscounterclock = false;

    private string[] clockCircleChain = new string[2] { "32413", "34132" };
    private string[] counterCircleChain = new string[3] { "42314", "43142", "23142" };
    private float timer =0f;
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
            if (isclock)
            {
                cubleroateSpeed -= 0.003f;
            }
            else if (iscounterclock)
            {
                cubleroateSpeed += 0.003f;
            }
            var inputPosition = Input.mousePosition;
            inputPosition.z = 10f;
            Vector3 touchCurrent = Camera.main.ScreenToWorldPoint(inputPosition);
            float diffX = Mathf.Abs(touchCurrent.x - lastDeviationCheck.x);
            float diffY = Mathf.Abs(touchCurrent.y - lastDeviationCheck.y);
            bool deviated = false;
            var previoustouch = touchCurrent;
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
          
            if (touchCurrent== previoustouch) //멈춰있다
            {
                //Debug.Log("같다");
                timer += Time.deltaTime;
                //Debug.Log(timer);
            }
            if (timer > 5f)
            {
                iscounterclock = false;
                isclock = false;
                if (cubleroateSpeed > 0f)
                {
                    cubleroateSpeed--;
                    if ( cubleroateSpeed < 0f)
                    {
                        cubleroateSpeed = 0f;
                        timer = 0f;
                    }
                   
                }
                else if (cubleroateSpeed < 0f)
                {
                    cubleroateSpeed++;
                    if (cubleroateSpeed > 0f)
                    {
                        cubleroateSpeed = 0f;
                        timer = 0f;
                    }
                }

            }

            foreach (var chain in clockCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    //Debug.Log($"{chain} in {touchPatternChain} 시계방향으로 돌고있다");
                    Debug.Log("☆시계방향으로 돌고있다");
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;

                    isclock = true;
                    iscounterclock = false;
                    timer = 0f;
                }

            }
            foreach (var chain in counterCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    //Debug.Log($"{chain} in {touchPatternChain} 반시계방향으로 돌고있다");
                    Debug.Log($"★반시계방향으로 돌고있다");
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;

                    iscounterclock = true;
                    isclock = false;
                    timer = 0f;
                }
              
            }
        }

        // touch end
        if (fingerIsDown && Input.GetMouseButtonUp(0))
        {
            touchPatternChain = string.Empty;
            touchPattern = string.Empty;
            iscounterclock = false;
            isclock = false;
            fingerIsDown = false;
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
            //Debug.Log($"if문이전에 touchPattern : {touchPattern}");
            if (touchPattern.Substring(touchPattern.Length - 1) != thisPattern) //마지막 인덱스와 비교해서 같지 않으면 더해라.
            {
                touchPattern += thisPattern;
                //Debug.Log($"if문이후에 touchPattern : {touchPattern}");
            }
        }
    }
}
