using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ScreenFixed : MonoBehaviour
{
    public List<Camera> cameras;

    [Header("화면 해상도")]
    public readonly int fixedWidth = 3040;
    public readonly int fixedHeight = 1440;

    private void OnEnable() { }

    private void Awake()
    {
        SetResolution();
    }

    public void SetResolution()
    {
        // 디바이스의 가로, 세로 가져오기
        var deviceWidth = Screen.width;
        var deviceHeight = Screen.height;

        // 내가 정한 사이즈의 비율과 사용자의 디바이스 사이즈 비율
        var myRatioFixed = (float)fixedWidth / fixedHeight;
        var deviceRatioFixed = (float)deviceWidth / deviceHeight;

        Debug.Log((int)(((float)deviceHeight / deviceWidth) * fixedWidth));
        // 지정한 비율로 변경한다
        Screen.SetResolution(fixedWidth, (int)(((float)deviceHeight / deviceWidth) * fixedWidth), true);

        // 기기의 해상도가 큰지 내가 지정한 해상도가 큰지에 따라 보정해주는 작업
        if (myRatioFixed < deviceRatioFixed)
        {
            // 넓이 다시 계산
            float newWidth = myRatioFixed / deviceRatioFixed;

            // 카메라를 보정해주는 것으로 꽉 찬 상태로 만들어 주는 것
            // 카메라를 보정해주지 않으면 19:9 비율은 유지하지만 가로, 세로 어디든 꽉찬 상태가 되지 않는다
            var rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);

            // TODO : 카메라가 추가될 때 하단에 추가해야 함(else 부분도)
            SetCamera(rect);
        }
        else
        {
            // 높이 다시 계산
            float newHeight = deviceRatioFixed / myRatioFixed;
            var rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);

            SetCamera(rect);
        }
    }

    private void SetCamera(Rect rect)
    {
        if (Camera.main != null)
            Camera.main.rect = rect;

        if (cameras != null)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].rect = rect;
            }
        }

        var pc = GameManager.Manager.ProductionCamera;
        if (pc != null)
        {
            pc.GetComponent<Camera>().rect = rect;
            pc.enabled = false;
        }
    }

    private void OnPreCull() => GL.Clear(true, true, Color.black);
}
