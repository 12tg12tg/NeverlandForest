using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class TableEditor : EditorWindow
{
    private int typeIndex;
    private int itemIndex;

    private static DataTableElemBase dataTableElemBase;

    private List<ItemTableElem> dataList = new List<ItemTableElem>();
    private static Dictionary<string, Type> tableTypes = new Dictionary<string, Type>();

    [MenuItem("Window/Table Editor")]
    public static void OpenTableWindow() // 얘는 Window-Table Editor로 킬 때 딱 한번 호출 된다
    {
        var window = GetWindow<TableEditor>();
        var title = new GUIContent();
        title.text = "Table Editor";
        window.titleContent = title;
        DataTableManager.Init();

        var fullTable = DataTableManager.tables;
        foreach (var key in fullTable.Keys)
        {
            tableTypes.Add(key.ToString(), key);
        }
    }
    private void OnGUI()
    {
        var tableType = DataTableManager.tables.Keys.Select(n => n.ToString()).ToArray();
        typeIndex = EditorGUILayout.Popup("TableType", typeIndex, tableType);

        var itemType = DataTableManager.tables[tableTypes[tableType[typeIndex]]];
        var itemTypeList = itemType.data.Keys.Select(n => n.ToString()).ToArray();
        itemIndex = EditorGUILayout.Popup("ItemList", itemIndex, itemTypeList); // 해당 타입의 아이템 리스트
        
        dataTableElemBase = itemType.data[itemTypeList[itemIndex]];
        
        switch (itemType) // 아이템 출력 부분
        {
            case ItemTable: // 아이템 테이블
                ConsumTable(itemType);
                break;
            case ArmorTable: // 아머 테이블
                break;
        }
    }

    private void ConsumTable(DataTableBase itemTable)
    {
        // 에디터에서 보여주는 데이터들
        var consumData = dataTableElemBase as ItemTableElem;
        consumData.name = EditorGUILayout.TextField("Name", consumData.name);
        consumData.description = EditorGUILayout.TextField("Description", consumData.description);
        consumData.hp = EditorGUILayout.IntField("Hp", consumData.hp);
        consumData.mp = EditorGUILayout.IntField("Mp", consumData.mp);
        consumData.statStr = EditorGUILayout.IntField("statStr", consumData.statStr);
        consumData.duration = EditorGUILayout.FloatField("Duration", consumData.duration);
        consumData.cost = EditorGUILayout.IntField("Cost", consumData.cost);

        if (GUILayout.Button("Change"))
        {
            // ??? csv 새롭게 쓰는게 목표
        }

        if (GUILayout.Button("CreateScriptableObj"))
        {
            OnCreate(itemTable, consumData);
        }
    }

    private void OnCreate(DataTableBase itemTable, ItemTableElem itemTableElem)
    {
        //if (dataList.Exists(n => (n as ItemTableElem).name == itemTableElem.name))
        //{
        //    var data = dataList.Find(n => (n as ItemTableElem).name == itemTableElem.name) as ItemTableElem;
        //    data.name = itemTableElem.name;
        //    data.hp = itemTableElem.hp;
        //    data.mp = itemTableElem.mp;
        //    return;
        //}

        var itemData = ScriptableObject.CreateInstance<CreateScriptableObject>();
        
        itemData.dicScriptableObj = itemTable.data;
        var elem = itemData.dicScriptableObj["CON_0001"] as ItemTableElem;
        Debug.Log(elem.hp);
        //var dictionary = new SerializeDictionary<string, int>();
        //var list = new List<TestList>();
        //var test = new TestList();
        //test.id = "하나";
        //test.num = 1;
        //list.Add(test);
        //dictionary.Add("하나", 1);
        //dictionary.Add("둘", 2);
        //dictionary.Add("셋", 3);
        //itemData.dic = dictionary;
        //itemData.testList = list;

        //itemData.id = itemTableElem.id;
        //itemData.hp = itemTableElem.hp;
        //itemData.mp = itemTableElem.mp;

        //foreach (var elem in itemData.dicScriptableObj)
        //{
        //    Debug.Log($"{elem.Key} {elem.Value}");
        //}

        //Debug.Log(itemData.dicScriptableObj.Count);
        //var itemDataElem = itemData.dicScriptableObj[consumData.id] as ItemTableElem;
        //itemDataElem.name = consumData.name;
        //itemDataElem.hp = consumData.hp;
        //itemDataElem.mp = consumData.mp;

        //dataList.Add(itemData.dicScriptableObj[consumData.id]);
        //dataList.Add(itemTableElem);
        AssetDatabase.CreateAsset(itemData, $"Assets/Editor/{itemTable.GetType()}.asset");
        AssetDatabase.Refresh();
    }
}
