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

    private List<DataTableElemBase> dataList = new List<DataTableElemBase>();
    private static Dictionary<string, Type> tableTypes = new Dictionary<string, Type>();

    [MenuItem("Window/Table Editor")]
    public static void OpenTableWindow() // ��� Window-Table Editor�� ų �� �� �ѹ� ȣ�� �ȴ�
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
        itemIndex = EditorGUILayout.Popup("ItemList", itemIndex, itemTypeList); // �ش� Ÿ���� ������ ����Ʈ

        dataTableElemBase = itemType.data[itemTypeList[itemIndex]];

        switch (itemType) // ������ ��� �κ�
        {
            case ItemTable: // ������ ���̺�
                ConsumTable(itemType as ItemTable);
                break;
            case ArmorTable: // �Ƹ� ���̺�
                break;
        }
    }

    private void ConsumTable(ItemTable itemTable)
    {
        // �����Ϳ��� �����ִ� �����͵�
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
            // ??? csv ���Ӱ� ���°� ��ǥ
        }

        if (GUILayout.Button("CreateScriptableObj"))
        {
            OnCreate(itemTable);
        }
    }

    private void OnCreate(ItemTable itemTable)
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
        //foreach (var elem in itemTable.data)
        //{
        //    itemData.dicScriptableObj.Add(elem.Key, elem.Value);
        //    id[count++] = elem.Key;
        //}

        
        foreach (var elem in itemData.dicScriptableObj)
        {
            Debug.Log($"{elem.Key} {elem.Value}");
        }

        //Debug.Log(itemData.dicScriptableObj.Count);
        //var itemDataElem = itemData.dicScriptableObj[consumData.id] as ItemTableElem;
        //itemDataElem.name = consumData.name;
        //itemDataElem.hp = consumData.hp;
        //itemDataElem.mp = consumData.mp;

        //dataList.Add(itemData.dicScriptableObj[consumData.id]);

        AssetDatabase.CreateAsset(itemData, $"Assets/Editor/{itemTable.data}.asset");
        AssetDatabase.Refresh();
    }
}
