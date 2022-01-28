using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFixed : MonoBehaviour
{
    public Camera uiCamera;
    public Camera uiCamera2;

    [Header("ȭ�� �ػ�")]
    public readonly int fixedWidth = 3040;
    public readonly int fixedHeight = 1440;

    private void Start()
    {
        SetResolution();
    }

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

        // ������ ������ �����Ѵ�
        Screen.SetResolution(fixedWidth, (int)(((float)deviceHeight / deviceWidth) * fixedWidth), true);

        // ���ΰ� �� ū ���� ���ΰ� �� ū ��쿡 ���� �������ִ� ��(���ε� ���ε� �ϳ��� �� �� ���·�)
        if (myRatioFixed < deviceRatioFixed)
        {
            // ���� �ٽ� ���
            float newWidth = myRatioFixed / deviceRatioFixed;

            // ī�޶� �������ִ� ������ �� �� ���·� ����� �ִ� ��
            // ī�޶� ���������� ������ 16:9 ������ ���������� ����, ���� ���� ���� ���°� ���� �ʴ´�
            var rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);

            // TODO : ī�޶� �߰��� �� �ϴܿ� �߰��ؾ� ��(else �κе�)
            Camera.main.rect = rect;
            if (uiCamera != null)
                uiCamera.rect = rect;
            if (uiCamera2 != null)
                uiCamera2.rect = rect;
        }
        else
        {
            // ���� �ٽ� ���
            float newHeight = deviceRatioFixed / myRatioFixed;
            var rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);

            Camera.main.rect = rect;
            if (uiCamera != null)
                uiCamera.rect = rect;
            if (uiCamera2 != null)
                uiCamera2.rect = rect;
        }

        Debug.Log("ȭ�� ������ �Ϸ�");
    }

    public void OnPreCull()
    {
        GL.Clear(true, true, Color.red);
    }
}
