using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class TableEditor : EditorWindow
{
    private static string[] tableName = { "CharacterDataTable", "CharacterLevelTable", "ConsumDataTable", "DefDataTable", "WeaponDataTable", "AllItemDataTable", "RecipeDataTable" };
    private static string csvFilePath = "Tables/";

    private int typeIndex;
    private int itemIndex;

    private static Dictionary<string, string[]> tableFirstElem = new Dictionary<string, string[]>();
    private static Dictionary<string, List<Dictionary<string, string>>> tableTypes = new Dictionary<string, List<Dictionary<string, string>>>();

    private static List<List<Dictionary<string, string>>> dataList = new List<List<Dictionary<string, string>>>();

    [MenuItem("Window/Table Editor")]
    private static void OpenTableWindow()
    {
        Init();
        var window = GetWindow<TableEditor>();
        var title = new GUIContent();
        title.text = "Table Editor";
        window.titleContent = title;
    }
    private static void Init()
    {
        tableTypes.Clear();
        tableFirstElem.Clear();
        for (int i = 0; i < tableName.Length; i++)
        {
            var path = csvFilePath + tableName[i];
            var tableElemList = CSVReader.Read(path);
            tableTypes.Add(tableName[i], tableElemList);
            tableFirstElem.Add(tableName[i], tableElemList.First().Keys.ToArray());
        }
    }
    private void OnGUI()
    {
        var tableType = tableTypes.Keys.Select(n => n.ToString()).ToArray();
        typeIndex = EditorGUILayout.Popup("TableType", typeIndex, tableType);
        var tableList = tableTypes[tableType[typeIndex]];
        var itemTypeList = tableList.Select(n => n["ID"]).ToArray();
        itemIndex = EditorGUILayout.Popup("TableElemList", itemIndex, itemTypeList);

        switch (tableType[typeIndex])
        {
            case "CharacterDataTable":
                break;
            case "CharacterLevelTable":
                break;
            case "ConsumDataTable":
                ViewConsumableData(tableList);
                break;
            case "DefDataTable":
                ViewArmorData(tableList);
                break;
            case "WeaponDataTable":
                break;
            case "AllItemDataTable":
                break;
        }
        GUIButton(tableList, tableType[typeIndex]);
    }

    private void ViewConsumableData(List<Dictionary<string, string>> consumList)
    {
        // 에디터에서 보여주는 데이터들
        var consumData = consumList[itemIndex];
        consumData["ID"] = EditorGUILayout.TextField("Id", consumData["ID"]);
        consumData["ICON_ID"] = EditorGUILayout.TextField("IconID", consumData["ICON_ID"]);
        consumData["PREFAB_ID"] = EditorGUILayout.TextField("PrefabsID", consumData["PREFAB_ID"]);
        consumData["NAME"] = EditorGUILayout.TextField("Name", consumData["NAME"]);
        consumData["DESC"] = EditorGUILayout.TextField("Description", consumData["DESC"]);
        consumData["COST"] = EditorGUILayout.TextField("Cost", consumData["COST"]);
        consumData["STAT_HP"] = EditorGUILayout.TextField("Hp", consumData["STAT_HP"]);
        consumData["STAT_MP"] = EditorGUILayout.TextField("Mp", consumData["STAT_MP"]);
        consumData["STAT_STR"] = EditorGUILayout.TextField("statStr", consumData["STAT_STR"]);
        consumData["DURATION"] = EditorGUILayout.TextField("Duration", consumData["DURATION"]);
    }
    private void ViewArmorData(List<Dictionary<string, string>> armorList)
    {
        // 에디터에서 보여주는 데이터들
        var armorData = armorList[itemIndex];
        armorData["ID"] = EditorGUILayout.TextField("Id", armorData["ID"]);
        armorData["ICON_ID"] = EditorGUILayout.TextField("IconID", armorData["ICON_ID"]);
        armorData["PREFAB_ID"] = EditorGUILayout.TextField("PrefabsID", armorData["PREFAB_ID"]);
        armorData["NAME"] = EditorGUILayout.TextField("Name", armorData["NAME"]);
        armorData["DESC"] = EditorGUILayout.TextField("Description", armorData["DESC"]);
        armorData["TYPE"] = EditorGUILayout.TextField("Type", armorData["TYPE"]);
        armorData["DEF"] = EditorGUILayout.TextField("Def", armorData["DEF"]);
        armorData["COST"] = EditorGUILayout.TextField("Cost", armorData["COST"]);
        armorData["WEIGHT"] = EditorGUILayout.TextField("Weight", armorData["WEIGHT"]);
        armorData["STR"] = EditorGUILayout.TextField("Str", armorData["STR"]);
        armorData["INT"] = EditorGUILayout.TextField("Int", armorData["INT"]);
        armorData["LUK"] = EditorGUILayout.TextField("Luk", armorData["LUK"]);
        armorData["EVADE"] = EditorGUILayout.TextField("Evade", armorData["EVADE"]);
        armorData["BLOCK"] = EditorGUILayout.TextField("Block", armorData["BLOCK"]);
    }

    private void GUIButton(List<Dictionary<string, string>> tableList, string tableName)
    {
        GUILayout.Space(10f); // 간격 용도
        if (GUILayout.Button("CSVChange"))
        {
            OnChangeCSV(tableName, tableList);
        }
        GUILayout.Space(10f); // 간격 용도
        if (GUILayout.Button("ScriptableObjCreateOrChange"))
        {
            OnCreateScriptableObject(tableName, tableList);
        }
    }
    private void OnCreateScriptableObject(string tableName, List<Dictionary<string, string>> tableList)
    {
        var itemData = ScriptableObject.CreateInstance<ScriptableObjectDataBase>();
        var path = $"Assets/Resources/{tableName}.asset";
        var assetPath = AssetDatabase.GetAssetPath(Resources.Load<ScriptableObjectDataBase>(tableName));
        if (assetPath.Equals(path))
        {
            Debug.Log("있음");
            dataList.Remove(tableList);
            AssetDatabase.DeleteAsset(path);
        }
        foreach (var dic in tableList)
        {
            var serialDic = new SerializeDictionary<string, string>(dic);
            itemData.sc.Add(serialDic);
        }
        dataList.Add(tableList);

        AssetDatabase.CreateAsset(itemData, path);
        AssetDatabase.Refresh();
    }
    private void OnChangeCSV(string tableName, List<Dictionary<string, string>> tableList)
    {
        var firstElem = tableFirstElem[tableName];
        CSVWriter.Writer(csvFilePath + tableName, firstElem, tableList);
    }
}