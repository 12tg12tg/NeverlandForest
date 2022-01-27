using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFixed : MonoBehaviour
{
    public int fixedWidth = 1920;
    public int fixedHeight = 1080;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 5, 100, 50), "SetScreen"))
            SetResolution();
    }

    public void SetResolution()
    {
        var deviceWidth = Screen.width; // 기기 너비 저장
        var deviceHeight = Screen.height; // 기기 높이 저장

        Debug.Log($"{deviceWidth} {deviceHeight}");

        var ratioFixed = (float)fixedWidth / fixedHeight;
        var deviceFixed = (float)deviceWidth / deviceHeight;

        Debug.Log(deviceFixed);

        // 16:9의 비율로 변경한다
        Screen.SetResolution(fixedWidth, (int)(((float)deviceHeight / deviceWidth) * fixedWidth), true);

        if (ratioFixed < deviceFixed) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ratioFixed / deviceFixed; // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = deviceFixed / ratioFixed; // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
}
