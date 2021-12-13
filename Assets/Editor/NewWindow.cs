using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewWindow : EditorWindow
{
    private string m_itemString = "";
    private int m_stringSelect;

    [MenuItem("Window/Test Window")]
    public static void OpenTestWindow()
    {
        var window = GetWindow<NewWindow>();

        var title = new GUIContent();
        title.text = "ABCDE Window";
        window.titleContent = title;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("DRopdown Button : ");
        if (EditorGUILayout.DropdownButton(new GUIContent(m_itemString), FocusType.Keyboard))
        {
            var alls = new string[4] { "A", "B", "C", "D" };
            GenericMenu _menu = new GenericMenu();
            foreach (var item in alls)
            {
                if(string.IsNullOrEmpty(item))
                {
                    continue;
                }

                _menu.AddItem(new GUIContent(item), m_itemString.Equals(item), OnValueSelected, item);
            }
            _menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        m_stringSelect = EditorGUILayout.Popup("Popup", m_stringSelect, new string[] { "A", "B", "C", "D", "E" });

        Debug.Log(Type.GetType("NewWindow"));
        Debug.Log(Type.GetType("MultiTouch"));

        //Dictionary<string, Type> tableTypes = new Dictionary<string, Type>();
        //var fullTable = DataTableManager.tables;
        //foreach (var key in fullTable.Keys)
        //{
        //    tableTypes.Add(key.ToString(), key);
        //}
        var type = typeof(NewWindow);

        EditorGUILayout.EndVertical();
    }
    void OnValueSelected(object value)
    {
        m_itemString = value.ToString();
    }
}
