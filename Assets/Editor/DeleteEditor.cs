using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveDataDelete))]
public class DeleteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var data = (SaveDataDelete)target;
        GUILayout.Space(10);
        if (GUILayout.Button("세이브 삭제"))
            data.DeleteFile();
        GUILayout.Space(10);
        if (GUILayout.Button("세이브 전체 삭제"))
            data.AllSaveDataDelete();
        GUILayout.Space(10);
        if (GUILayout.Button("세이브 폴더 오픈"))
            data.OpenFolder();
    }
}