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
        var deviceWidth = Screen.width; // ��� �ʺ� ����
        var deviceHeight = Screen.height; // ��� ���� ����

        Debug.Log($"{deviceWidth} {deviceHeight}");

        var ratioFixed = (float)fixedWidth / fixedHeight;
        var deviceFixed = (float)deviceWidth / deviceHeight;

        Debug.Log(deviceFixed);

        // 16:9�� ������ �����Ѵ�
        Screen.SetResolution(fixedWidth, (int)(((float)deviceHeight / deviceWidth) * fixedWidth), true);

        if (ratioFixed < deviceFixed) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ratioFixed / deviceFixed; // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = deviceFixed / ratioFixed; // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }
}
