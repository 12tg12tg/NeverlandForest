using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObjectDataBase)), CanEditMultipleObjects]
public class ScriptableDictionaryEditor : Editor
{
    bool isInit;
    List<bool> foldOutList = new List<bool>();
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        GUI.skin.button.hover.textColor = Color.black;

        EditorGUILayout.BeginVertical();

        var sc = target as ScriptableObjectDataBase;
        var list = sc.sc;

        if (!isInit)
            Init(list.Count);

        string name = string.Empty;
        var nmaeList = (from n in list
                       where n.TryGetValue("NAME", out name)
                       select name).ToArray();

        for (int i = 0; i < list.Count; i++)
        {
            if(string.IsNullOrEmpty(name))
                foldOutList[i] = EditorGUILayout.Foldout(foldOutList[i], $"{i + 1}번 째 요소");
            else
                foldOutList[i] = EditorGUILayout.Foldout(foldOutList[i], nmaeList[i]);

            if (foldOutList[i])
            {
                for (int j = 0; j < list[i].Keys.Count; j++)
                {
                    var key = list[i].Keys.ToArray()[j];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(key, GUILayout.Width(90));
                    if(!key.Equals("DESC"))
                        list[i][key] = EditorGUILayout.TextField(list[i][key]);
                    else
                        list[i][key] = EditorGUILayout.TextField(list[i][key], GUILayout.Height(50));
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void Init(int listCount)
    {
        isInit = true;
        for (int i = 0; i < listCount; i++)
        {
            foldOutList.Add(false);
        }
    }    
}
