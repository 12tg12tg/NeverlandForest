using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoObjectPool : Singleton<InfoObjectPool>
{
    public int count;
    public int capacity;

    private void OnGUI()
    {
        GUILayout.TextField($"Count : {count} / Capacity : {capacity}");

        if (GUILayout.Button("Go Next Scene"))
        {
            SceneManager.LoadScene(1);
        }
    }
}
