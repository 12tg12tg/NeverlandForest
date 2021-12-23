using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    private bool fingerIsDown = false;
    private Vector3 touchStart;
    private Vector3 touchEnd;

    private float deviationCheckDistance = 2.0f; //체크편차거리 5
    private Vector3 lastDeviationCheck = Vector3.zero; //마지막 편차 0

    public float touchPatternResetTime = 5.0f;
    private float touchPatternTime = 0.0f;

    private string touchPattern = string.Empty;
    private string touchPatternChain = string.Empty;

    // pattern chains
   // private string[] slashUpChain = new string[5] { "4", "41", "42", "14", "24" };
    private string[] slashUpChain = new string[5] { "34", "41", "42", "14", "24" };
   // private string[] crossSlashChain = new string[6] { "13,23", "23,13", "32,31", "31,32", "13,32", "32,13" };
    private string[] sideSlashChain = new string[4] { "12", "21","1","2" };
    private string[] circleChain = new string[11]
    { "13241","14231", "23142", "24132" , "31423", "32413","34132","34231","42314","43142","43241" };

    // registered pattern listeners
    private List<GameObject> slashUpListeners = new List<GameObject>();
    private List<GameObject> crossSlashListeners = new List<GameObject>();
    private List<GameObject> sideSlashListeners = new List<GameObject>();
    private List<GameObject> circleListeners = new List<GameObject>();
    private int circleCount;
    void Update()
    {
        // touch start
        if (!fingerIsDown && Input.GetMouseButton(0))
        {   
            fingerIsDown = true;
            circleCount = 0;
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

        }

        // touch end
        if (fingerIsDown && Input.GetMouseButtonUp(0))
        {
            Debug.Log(touchPattern);
            fingerIsDown = false;
            touchEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchEnd.z = -1;

            // check patterns on mouse up
            if (touchPattern != string.Empty)
            {
                ProcessPattern(touchPattern);
            }
        }

        touchPatternTime += Time.deltaTime;
        // if we are over the pattern matching time limit the user did something else
        if (touchPatternTime > touchPatternResetTime)
        {
            //Debug.Log(touchPatternTime);
            ResetPattern();
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
        touchPatternTime = 0.0f;
        touchPattern = string.Empty;
    }

    void ResetPattern()
    {
        //touchPatternChain = string.Empty;
        ResetTouch();
    }

    void ProcessPattern(string pattern)
    {
        ResetTouch();

        if (touchPatternChain.Length > 0) 
        {
            /* touchPatternChain += "," + pattern;
             Debug.Log($"lengh>0 : {touchPatternChain}");
             Debug.Log($"lengh+pattern : {touchPatternChain}"); //x 모양 패턴 구현을 위해 있음
             if (touchPatternChain.Length > 2)
             {
                 touchPatternChain = string.Empty;
             }*/
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
                ProcessSlashUp();
                Debug.Log("slashup");

                return;
            }
        }

        // cross slash
       /* foreach (string chain in crossSlashChain)
        {
            if (touchPatternChain == chain)
            {
                touchPatternChain = string.Empty;
                ResetPattern();
                ProcessCrossSlash();
                Debug.Log("XCross");
                return;
            }
        }*/

        // side slash
        foreach (string chain in sideSlashChain)
        {
            if (touchPatternChain == chain)
            {
                touchPatternChain = string.Empty;
                ResetPattern();
                ProcessSideSlash();
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
                ProcessCircle();
                Debug.Log("Circle");
                return;
            }
            else if (touchPatternChain.Contains(chain))
            {   
                Debug.Log("CircleInclude");
                circleCount++;
                // 국자 처럼 돌릴꺼면 여기함수에서 뭔가를 해주면 된다.
            }
           
        }
        Debug.Log(circleCount);
    }

    void ProcessSlashUp()
    {
       /* for (int i = 0; i < slashUpListeners.Count; i++)
        {
            slashUpListeners[i].SendMessage("Activate", touchEnd);
        }*/
    }
/*
    void ProcessCrossSlash()
    {
        for (int i = 0; i < crossSlashListeners.Count; i++)
        {
            crossSlashListeners[i].SendMessage("ActivateCross");
        }
    }
*/
    void ProcessSideSlash()
    {
        /*Vector3[] sendParams = new Vector3[2] { touchStart, touchEnd };
        for (int i = 0; i < sideSlashListeners.Count; i++)
        {
            sideSlashListeners[i].SendMessage("Activate", sendParams);
        }*/
    }

    void ProcessCircle()
    {
       /* for (int i = 0; i < circleListeners.Count; i++)
        {
            circleListeners[i].SendMessage("ActivateCircle");
        }*/
    }
}
