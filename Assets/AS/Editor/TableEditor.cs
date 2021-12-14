using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.IO;

public class TableEditor : EditorWindow
{
    private int typeIndex;
    private int itemIndex;

    private DataTableElemBase dataTableElemBase;
    private static List<DataTableBase> dataList = new List<DataTableBase>();
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
        tableTypes.Clear();
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
            case ConsumableTable: // ������ ���̺�
                ConsumTableEditor(itemType);
                break;
            case ArmorTable: // �Ƹ� ���̺�
                ArmorTableEditor(itemType);
                break;
        }
    }

    private void ConsumTableEditor(DataTableBase itemTable)
    {
        // �����Ϳ��� �����ִ� �����͵�
        var consumData = dataTableElemBase as ConsumableTableElem;
        consumData.id = EditorGUILayout.TextField("Id", consumData.id);
        consumData.iconID = EditorGUILayout.TextField("IconID", consumData.iconID);
        consumData.prefabsID = EditorGUILayout.TextField("PrefabsID", consumData.prefabsID);
        consumData.name = EditorGUILayout.TextField("Name", consumData.name);
        consumData.description = EditorGUILayout.TextField("Description", consumData.description);
        consumData.cost = EditorGUILayout.IntField("Cost", consumData.cost);
        consumData.hp = EditorGUILayout.IntField("Hp", consumData.hp);
        consumData.mp = EditorGUILayout.IntField("Mp", consumData.mp);
        consumData.statStr = EditorGUILayout.IntField("statStr", consumData.statStr);
        consumData.duration = EditorGUILayout.FloatField("Duration", consumData.duration);
        
        GUILayout.Space(10f); // ���� �뵵
        if (GUILayout.Button("Change"))
        {
            OnChangeConsumData(itemTable);
        }
        GUILayout.Space(10f); // ���� �뵵
        if (GUILayout.Button("CreateScriptableObj"))
        {
            OnCreateConsumable(itemTable, consumData);
        }
    }
    private void OnCreateConsumable(DataTableBase itemTable, ConsumableTableElem consumableTableElem)
    {
        var itemData = ScriptableObject.CreateInstance<CreateConsumScriptableObject>();
        var path = $"Assets/Editor/{itemTable.GetType()}.asset";
        if (dataList.Exists(n => n.GetType() == itemTable.GetType()))
        {
            Debug.Log("����");
            dataList.Remove(itemTable);
            AssetDatabase.DeleteAsset(path);
        }

        foreach (var item in itemTable.data)
        {
            itemData.dicConsumObj.Add(item.Key, item.Value as ConsumableTableElem);
        }
        dataList.Add(itemTable);
        AssetDatabase.CreateAsset(itemData, path);
        AssetDatabase.Refresh();
    }
    private void OnChangeArmorData(DataTableBase dataTableBase)
    {
        var armor = dataTableBase as ArmorTable;
        armor.Save(dataTableBase);
    }


    private void ArmorTableEditor(DataTableBase itemTable)
    {
        // �����Ϳ��� �����ִ� �����͵�
        var armorData = dataTableElemBase as ArmorTableElem;
        armorData.id = EditorGUILayout.TextField("Id", armorData.id);
        armorData.name = EditorGUILayout.TextField("Name", armorData.name);
        armorData.description = EditorGUILayout.TextField("Description", armorData.description);
        armorData.iconID = EditorGUILayout.TextField("IconID", armorData.iconID);
        armorData.prefabsID = EditorGUILayout.TextField("PrefabsID", armorData.prefabsID);
        armorData.type = EditorGUILayout.TextField("Name", armorData.type);
        armorData.cost = EditorGUILayout.IntField("Cost", armorData.cost);
        armorData.defence = EditorGUILayout.IntField("Defence", armorData.defence);
        armorData.weight = EditorGUILayout.IntField("Weight", armorData.weight);
        armorData.stat_str = EditorGUILayout.IntField("Str", armorData.stat_str);
        armorData.stat_con = EditorGUILayout.IntField("Con", armorData.stat_con);
        armorData.stat_int = EditorGUILayout.IntField("Int", armorData.stat_int);
        armorData.stat_luk = EditorGUILayout.IntField("Luk", armorData.stat_luk);
        armorData.evade = EditorGUILayout.IntField("Evade", armorData.evade);
        armorData.block = EditorGUILayout.IntField("Block", armorData.block);

        GUILayout.Space(10f); // ���� �뵵
        if (GUILayout.Button("Change"))
        {
            OnChangeArmorData(itemTable);
        }
        GUILayout.Space(10f); // ���� �뵵
        if (GUILayout.Button("CreateScriptableObj"))
        {
            OnCreateArmor(itemTable, armorData);
        }
    }
    private void OnCreateArmor(DataTableBase itemTable, ArmorTableElem armorTableElem)
    {
        var itemData = ScriptableObject.CreateInstance<CreateArmorScriptableObject>();
        var path = $"Assets/Editor/{itemTable.GetType()}.asset";
        if (dataList.Exists(n => n.GetType() == itemTable.GetType()))
        {
            Debug.Log("����");
            dataList.Remove(itemTable);
            AssetDatabase.DeleteAsset(path);
        }

        foreach (var item in itemTable.data)
        {
            itemData.dicArmorObj.Add(item.Key, item.Value as ArmorTableElem);
        }

        dataList.Add(itemTable);
        AssetDatabase.CreateAsset(itemData, $"Assets/Editor/{itemTable.GetType()}.asset");
        AssetDatabase.Refresh();
    }
    private void OnChangeConsumData(DataTableBase dataTableBase)
    {
        var consum = dataTableBase as ConsumableTable;
        consum.Save(dataTableBase);
    }
}