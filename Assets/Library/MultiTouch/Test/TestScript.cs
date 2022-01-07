using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private MultiTouch multiTouch;
    private float maxZoom = 60f;
    private float minZoom = 100f;

    public MeshRenderer tap;
    public MeshRenderer longTap;
    public MeshRenderer DoubleTap;
    public MeshRenderer SwipeUp;
    public MeshRenderer SwipeDown;
    public MeshRenderer SwipeLeft;
    public MeshRenderer SwipeRight;
    public MeshRenderer touch1;
    public MeshRenderer touch2;

    private void Start()
    {
        multiTouch = MultiTouch.Instance;
    }

    private void Update()
    {
        if (multiTouch.IsTap)
        {
            Debug.Log("IsTap");
            tap.material.color = Color.red;
            StartCoroutine(CoResetColor(tap));
        }

        if (multiTouch.IsDoubleTap)
        {
            Debug.Log("IsDoubleTap");
            DoubleTap.material.color = Color.red;
            StartCoroutine(CoResetColor(DoubleTap));
        }

        if (multiTouch.IsLongTap)
        {
            Debug.Log("IsLongTap");
            longTap.material.color = Color.red;
            StartCoroutine(CoResetColor(longTap));
        }

        var swipe = multiTouch.Swipe;
        if (swipe.x != 0 && swipe.y != 0)
        {
            string str = "Swipe ";
            if (swipe.x > 0)
            {
                str += "Right ";
                SwipeRight.material.color = Color.red;
                StartCoroutine(CoResetColor(SwipeRight));
            }
            else
            {
                str += "Left ";
                SwipeLeft.material.color = Color.red;
                StartCoroutine(CoResetColor(SwipeLeft));
            }

            if (swipe.y > 0)
            {
                str += "Up!";
                SwipeUp.material.color = Color.red;
                StartCoroutine(CoResetColor(SwipeUp));
            }
            else
            {
                str += "Down!";
                SwipeDown.material.color = Color.red;
                StartCoroutine(CoResetColor(SwipeDown));
            }
            Debug.Log(str);
        }


        if(multiTouch.Zoom != 0f)
        {
            var view = Camera.main.fieldOfView;
            var change = view * (1 + multiTouch.Zoom);
            Camera.main.fieldOfView = (change > minZoom) ? minZoom : change;
            Camera.main.fieldOfView = (change < maxZoom) ? maxZoom : change;
        }

        if (multiTouch.RotateAngle != 0f)
        {
            transform.Rotate(0f, 0f, multiTouch.RotateAngle);
        }

        if(multiTouch.TouchCount == 1)
        {
            touch1.material.color = Color.red;
            StartCoroutine(CoResetColor(touch1));
        }

        if (multiTouch.TouchCount == 2)
        {
            touch2.material.color = Color.red;
            StartCoroutine(CoResetColor(touch2));
        }

    }

    private IEnumerator CoResetColor(MeshRenderer renderer)
    {
        yield return new WaitForSeconds(1f);
        renderer.material.color = Color.white;
    }
}
