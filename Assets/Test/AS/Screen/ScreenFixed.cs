using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ScreenFixed : MonoBehaviour
{
    public List<Camera> cameras;

    [Header("ȭ�� �ػ�")]
    public readonly int fixedWidth = 3040;
    public readonly int fixedHeight = 1440;

    private void OnEnable() { }

    private void Awake()
    {
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
            SetCamera(rect);
        }
        else
        {
            // ���� �ٽ� ���
            float newHeight = deviceRatioFixed / myRatioFixed;
            var rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);

            SetCamera(rect);
        }

        Debug.Log("ȭ�� ������ �Ϸ�");
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
            Debug.Log($"Ȱ��ȭ {pc.enabled}");
        }
    }

    private void OnPreCull() => GL.Clear(true, true, Color.black);
}
