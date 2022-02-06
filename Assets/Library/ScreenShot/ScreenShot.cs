using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    public new Camera camera;       //보여지는 카메라.

    string path;

    // Use this for initialization
    void Start()
    {
        path = Application.dataPath + "/ScreenShot/";
        Debug.Log(path);
    }

    public void ClickScreenShot()
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(path);
        }
        string name;
        name = path + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;

        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(name, bytes);
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(100f, 100f, 100f, 50f), "스크린샷"))
            ClickScreenShot();
    }
}
