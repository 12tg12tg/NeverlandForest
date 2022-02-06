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

        EditorGUILayout.LabelField("배틀 몬스터 웨이브 에디터", GUILayout.Width(150));

        cb.useCustomMode = EditorGUILayout.Toggle("에디터 적용", cb.useCustomMode, GUILayout.Width(80));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("전체 웨이브 수", GUILayout.Width(80));
        var index = cb.waveNum == 2 ? 0 : 1;
        index = EditorGUILayout.Popup(index, waveNums, GUILayout.Width(120));
        cb.waveNum = index == 0 ? 2 : 3;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("화살 수", GUILayout.Width(80));
        cb.arrowNum = EditorGUILayout.IntField(cb.arrowNum, GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("쇠화살 수", GUILayout.Width(80));
        cb.ironArrowNum = EditorGUILayout.IntField(cb.ironArrowNum, GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("오일 수", GUILayout.Width(80));
        cb.oilNum = EditorGUILayout.IntField(cb.oilNum, GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var lantern = cb.lanternCount;
        EditorGUILayout.LabelField("랜턴 밝기", GUILayout.Width(80));
        cb.lanternCount = (int)EditorGUILayout.Slider(lantern, 1, 18, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
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
                EditorGUILayout.LabelField("웨이브 1", GUILayout.Width(120));
            }
            else if (i == 1)
            { 
                list = cb.cwave2;
                haveMonster = cb.haveMonster2;
                EditorGUILayout.LabelField("웨이브 2", GUILayout.Width(120));
            }
            else
            { 
                list = cb.cwave3;
                haveMonster = cb.haveMonster3;
                EditorGUILayout.LabelField("웨이브 3", GUILayout.Width(120));
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
