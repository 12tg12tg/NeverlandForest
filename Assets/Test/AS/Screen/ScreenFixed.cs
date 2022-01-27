using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFixed : MonoBehaviour
{
    public int fixedWidth = 1920;
    public int fixedHeight = 1080;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 100, 50), "SetScreen"))
            SetResolution();
    }

    public void SetResolution()
    {
        // ����̽��� ����, ���� ��������
        var deviceWidth = Screen.width;
        var deviceHeight = Screen.height;

        // ���� ���� �������� ������ ������� ����̽� ������ ����
        var myRatioFixed = (float)fixedWidth / fixedHeight;
        var deviceRatioFixed = (float)deviceWidth / deviceHeight;

        // 16:9�� ������ �����Ѵ�
        Screen.SetResolution(fixedWidth, (int)(((float)deviceHeight / deviceWidth) * fixedWidth), true);

        // ���ΰ� �� ū ���� ���ΰ� �� ū ��쿡 ���� �������ִ� ��(���ε� ���ε� �ϳ��� �� �� ���·�)
        if (myRatioFixed < deviceRatioFixed)
        {
            // ���� �ٽ� ���
            float newWidth = myRatioFixed / deviceRatioFixed;

            // ī�޶� �������ִ� ������ �� �� ���·� ����� �ִ� ��
            // ī�޶� ���������� ������ 16:9 ������ ���������� ����, ���� ���� ���� ���°� ���� �ʴ´�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            // ���� �ٽ� ���
            float newHeight = deviceRatioFixed / myRatioFixed;
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}
