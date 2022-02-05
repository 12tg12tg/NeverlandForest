using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomBattle)), CanEditMultipleObjects]
public class CustomBattleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cb = target as CustomBattle;
        
        var names = System.Enum.GetNames(typeof(MonsterPoolTag));
        var waveNums = new string[] { "2", "3" };

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("��Ʋ ���� ���̺� ������", GUILayout.Width(150));

        cb.useCustomMode = EditorGUILayout.Toggle("������ ����", cb.useCustomMode, GUILayout.Width(80));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("��ü ���̺� ��", GUILayout.Width(80));
        var index = cb.waveNum == 2 ? 0 : 1;
        index = EditorGUILayout.Popup(index, waveNums, GUILayout.Width(120));
        cb.waveNum = index == 0 ? 2 : 3;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 3; i++)
        {
            EditorGUILayout.BeginVertical();

            List<MonsterPoolTag> list;
            List<bool> haveMonster;
            if (i == 0)
            {
                list = cb.cwave1;
                haveMonster = cb.haveMonster1;
                EditorGUILayout.LabelField("���̺� 1", GUILayout.Width(120));
            }
            else if (i == 1)
            { 
                list = cb.cwave2;
                haveMonster = cb.haveMonster2;
                EditorGUILayout.LabelField("���̺� 2", GUILayout.Width(120));
            }
            else
            { 
                list = cb.cwave3;
                haveMonster = cb.haveMonster3;
                EditorGUILayout.LabelField("���̺� 3", GUILayout.Width(120));
            }

            if (list.Count == 0)
            {
                for (int k = 0; k < 3; k++)
                {
                    list.Add(MonsterPoolTag.Mushbae);
                }
            }

            if (haveMonster.Count == 0)
            {
                for (int k = 0; k < 3; k++)
                {
                    haveMonster.Add(false);
                }
            }

            for (int j = 2; j >= 0; j--)
            {
                EditorGUILayout.BeginVertical();

                haveMonster[j] = EditorGUILayout.Toggle(haveMonster[j], GUILayout.Width(120));
                if (haveMonster[j])
                {
                    index = (int)list[j];
                    list[j] = (MonsterPoolTag)EditorGUILayout.Popup(index, names, GUILayout.Width(120));
                }
                else
                {
                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
