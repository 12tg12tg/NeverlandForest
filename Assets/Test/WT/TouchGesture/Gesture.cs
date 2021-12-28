using System.Collections.Generic;
using UnityEngine;
// RIGHT -> LEFT 1
// LEFT -> RIGHT 2
// TOP -> BOTTOM 3
// BOTTOM -> TOP 4

/*
 Spits out a string based on gesture directions regardless of size or location.
  Example:
  TOP LEFT  -> BOTTOM RIGHT SLASH   = 13
  TOP RIGHT -> BOTTOM LEFT SLASH    = 32
  COUNTER CLOCKWISE CIRCLE FROM TOP = 23142 - > 이거 왜 count임 시계방향 아닌가숫자만 보면 - >13241
  CLOCKWISE CIRCLE FROM TOP         = 13241 ->23142
*/

public class Gesture : MonoBehaviour
{
    public GameObject cube;

    private float cubleroateSpeed = 10.0f;
    private float count = 0.1f;
    private float curSpeed = 0f;

    private bool fingerIsDown = false;
    private Vector3 touchStart;
    private Vector3 touchEnd;

    private float deviationCheckDistance = 2.0f; //체크편차거리 5
    private Vector3 lastDeviationCheck = Vector3.zero; //마지막 편차 0

    private float touchPatternResetTime = 5f;
    private float touchPatternTime = 0.0f;

    private string touchPattern = string.Empty;
    private string touchPatternChain = string.Empty;

    // pattern chains
    // private string[] slashUpChain = new string[5] { "4", "41", "42", "14", "24" };
    private string[] slashUpChain = new string[5] { "34", "41", "42", "14", "24" };
    // private string[] crossSlashChain = new string[6] { "13,23", "23,13", "32,31", "31,32", "13,32", "32,13" };
    private string[] sideSlashChain = new string[4] { "12", "21", "1", "2" };
    private string[] circleChain = new string[11]
    { "13241","14231", "23142", "24132" , "31423", "32413","34132","34231","42314","43142","43241" };
    private string[] clockCircleChain = new string[2] { "32413", "34132" };
    private string[] counterCircleChain = new string[4] { "42314", "43142", "43241", "23142" };


    // registered pattern listeners
    private List<GameObject> slashUpListeners = new List<GameObject>();
    private List<GameObject> crossSlashListeners = new List<GameObject>();
    private List<GameObject> sideSlashListeners = new List<GameObject>();
    private List<GameObject> circleListeners = new List<GameObject>();
    void Update()
    {
        Debug.Log(touchPatternChain);

        // touch start
        if (!fingerIsDown && Input.GetMouseButton(0))
        {
            fingerIsDown = true;
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchStart.z = -1;
            lastDeviationCheck = touchStart;
            touchPatternTime = 0.0f; // reset touch pattern time
        }

        // touch middle
        if (fingerIsDown && Input.GetMouseButton(0))
        {
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
                }
                // right ->left
                if (touchCurrent.x < lastDeviationCheck.x)
                {
                    RecordPattern("2");
                    deviated = true;
                }
            }

            if (diffY > deviationCheckDistance)
            {
                // TOP -> BOTTOM
                if (touchCurrent.y < lastDeviationCheck.y)
                {
                    RecordPattern("3");
                    deviated = true;
                }
                // BOTTOM -> TOP
                if (touchCurrent.y > lastDeviationCheck.y)
                {
                    RecordPattern("4");
                    deviated = true;
                }
            }

            if (deviated)
            {
                lastDeviationCheck = touchCurrent;
            }
            touchPatternChain += touchPattern;
            //touchPattern = string.Empty;


            foreach (var chain in circleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    //Debug.Log("원을 그리고 있다");
                }
            }

            cube.transform.rotation = Quaternion.Euler(
                new Vector3(0f, 0f, cubleroateSpeed));

            foreach (var chain in clockCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    touchPatternChain = string.Empty;
                    Debug.Log($"chain:{chain}");
                    Debug.Log("시계방향으로 돌고있다");
                    count += 1f;
                    cubleroateSpeed -= 0.3f * count;

                }
            }
            foreach (var chain in counterCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    touchPatternChain = string.Empty;
                    Debug.Log($"chain:{chain}");
                    Debug.Log("반시계방향으로 돌고있다");
                    count += 1f;
                    cubleroateSpeed += 0.3f * count;

                }
            }
           
            touchPatternTime += Time.deltaTime;
        }

        // touch end
        if (fingerIsDown && Input.GetMouseButtonUp(0))
        {
            touchPatternChain = string.Empty;
            touchPattern = string.Empty;
            count = 0f;
            touchPatternTime = 0f;
        }

        // if we are over the pattern matching time limit the user did something else
        if (touchPatternTime > touchPatternResetTime)
        {
            touchPatternChain = string.Empty;
            curSpeed = cubleroateSpeed;
            ResetPattern();
           
            if (count > 0)
            {
                Debug.Log($"count값은 :{count}");
                count -= 0.6f;
                Debug.Log($"cubleroateSpeed :{cubleroateSpeed}");
            }
            else if (count > -1 && count < 0)
            {
                count = 0;
                cubleroateSpeed = Mathf.Lerp(curSpeed, 1f, 1 * Time.deltaTime);
                touchPatternTime = 0;
            }
           
        }
       
    }

    void RecordPattern(string thisPattern)
    {
        switch (touchPattern.Length)
        {
            case 0:
                touchPattern += thisPattern;
                break;
            case 1:
                if (thisPattern != touchPattern)
                {
                    touchPattern += thisPattern;
                }
                break;
            default:
                string lastCheck1 = touchPattern.Substring(touchPattern.Length - 2, 1);
                string lastCheck2 = touchPattern.Substring(touchPattern.Length - 2, 2);
                if (thisPattern != lastCheck1 && lastCheck1 + thisPattern != lastCheck2)
                {
                    touchPattern += thisPattern;
                }
                break;
        }
    }

    void ResetTouch()
    {
        touchPattern = string.Empty;
    }

    void ResetPattern()
    {
        ResetTouch();
    }

    void ProcessPattern(string pattern)
    {
        ResetTouch();

        if (touchPatternChain.Length > 0)
        {
            touchPatternChain = string.Empty;
            touchPatternChain += pattern;
        }
        else
        {
            touchPatternChain = pattern;
            Debug.Log($"else : {touchPatternChain}");
        }

        // slash up
        foreach (string chain in slashUpChain)
        {
            if (touchPatternChain == chain)
            {
                touchPatternChain = string.Empty;
                ResetPattern();
                Debug.Log("slashup");

                return;
            }
        }
        // side slash
        foreach (string chain in sideSlashChain)
        {
            if (touchPatternChain == chain)
            {
                touchPatternChain = string.Empty;
                ResetPattern();
                Debug.Log("sideslash");

                return;
            }
        }

        // circle
        foreach (string chain in circleChain)
        {
            if (touchPatternChain == chain)
            {
                touchPatternChain = string.Empty;
                ResetPattern();
                Debug.Log("Circle");
                return;
            }
            else if (touchPatternChain.Contains(chain))
            {
                Debug.Log("CircleInclude");
                // 국자 처럼 돌릴꺼면 여기함수에서 뭔가를 해주면 된다.
            }
        }
    }

}
