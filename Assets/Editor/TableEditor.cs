using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class TableEditor : EditorWindow
{
    private static readonly string[] tableName = { "ConsumDataTable", "DefDataTable", "WeaponDataTable",
        "AllItemDataTable", "RecipeDataTable", "PlayerSkillTable", "MonsterTable", "LocalizationTable", "RandomEventTable" };
    private static readonly string csvFilePath = "Tables/";

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
            var path = Path.Combine(csvFilePath, tableName[i]);
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
            case "ConsumDataTable":
                ViewConsumableData(tableList);
                break;
            case "DefDataTable":
                ViewArmorData(tableList);
                break;
            case "WeaponDataTable":
                break;
            case "AllItemDataTable":
                ViewAllItemData(tableList);
                break;
            case "PlayerSkillTable":
                ViewPlayerSkillData(tableList);
                break;
            case "MonsterTable":
                ViewMonsterData(tableList);
                break;
            case "LocalizationTable":
                ViewLocalizationData(tableList);
                break;
            case "RandomEventTable":
                ViewRandomEventData(tableList);
                break;
        }
        GUIButton(tableList, tableType[typeIndex]);
    }

    private void ViewAllItemData(List<Dictionary<string, string>> itemList)
    {
        var itemData = itemList[itemIndex];
        itemData["ID"] = EditorGUILayout.TextField("Id", itemData["ID"]);
        itemData["NAME"] = EditorGUILayout.TextField("Name", itemData["NAME"]);
        itemData["DESC"] = EditorGUILayout.TextField("Desc", itemData["DESC"]);
        itemData["TYPE"] = EditorGUILayout.TextField("Type", itemData["TYPE"]);
        itemData["PREFAB_ID"] = EditorGUILayout.TextField("Prefab_ID", itemData["PREFAB_ID"]);
        itemData["ICON_ID"] = EditorGUILayout.TextField("Icon_ID", itemData["ICON_ID"]);
        itemData["LIMIT"] = EditorGUILayout.TextField("Limit", itemData["LIMIT"]);
        itemData["EAT"] = EditorGUILayout.TextField("Eat", itemData["EAT"]);
        itemData["BURN"] = EditorGUILayout.TextField("Burn", itemData["BURN"]);
        itemData["BURN_RECOVERY"] = EditorGUILayout.TextField("Burn_recovery", itemData["BURN_RECOVERY"]);
        itemData["STAT_HP"] = EditorGUILayout.TextField("Stat_HP", itemData["STAT_HP"]);
        itemData["OBSTACLE_TYPE"] = EditorGUILayout.TextField("Obstacle_Type", itemData["OBSTACLE_TYPE"]);
        itemData["OBSTACLE_HP"] = EditorGUILayout.TextField("Obstacle_HP", itemData["OBSTACLE_HP"]);
        itemData["TRAP_DAMAGE"] = EditorGUILayout.TextField("Trap_Damage", itemData["TRAP_DAMAGE"]);
        itemData["DURATION"] = EditorGUILayout.TextField("Duration", itemData["DURATION"]);
        itemData["INSTALLATION_NUMBER"] = EditorGUILayout.TextField("Installation_Number", itemData["INSTALLATION_NUMBER"]);
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
    private void ViewPlayerSkillData(List<Dictionary<string, string>> skillList)
    {
        // 에디터에서 보여주는 데이터들
        var consumData = skillList[itemIndex];

        var id = int.Parse(consumData["ID"]);
        consumData["ID"] = EditorGUILayout.IntField("Id", id).ToString();

        consumData["ICON_ID"] = EditorGUILayout.TextField("IconID", consumData["ICON_ID"]);

        consumData["NAME"] = EditorGUILayout.TextField("Name", consumData["NAME"]);

        consumData["PLAYER"] = EditorGUILayout.TextField("Player", consumData["PLAYER"]);

        consumData["ANI TRIGGER"] = EditorGUILayout.TextField("Ani Trigger", consumData["ANI TRIGGER"]);

        var cost = int.Parse(consumData["COST"]);
        consumData["COST"] = EditorGUILayout.IntField("Cost", cost).ToString();

        var min_damage = int.Parse(consumData["MIN DAMAGE"]);
        consumData["MIN DAMAGE"] = EditorGUILayout.IntField("Min Damgae", min_damage).ToString();

        var max_damage = int.Parse(consumData["MAX DAMAGE"]);
        consumData["MAX DAMAGE"] = EditorGUILayout.IntField("Max Damgae", max_damage).ToString();

        var toggle = consumData["ADD LANTERN"].Equals("O") ? true : false;
        consumData["ADD LANTERN"] = EditorGUILayout.Toggle("Add Lantern", toggle) ? "O" : "X";

        var hit_count = int.Parse(consumData["HIT COUNT"]);
        consumData["HIT COUNT"] = EditorGUILayout.IntField("Max Damgae", hit_count).ToString();

        var rangeType = System.Enum.GetNames(typeof(SkillRangeType));
        int shapeIndex = ArrayUtility.IndexOf(rangeType, consumData["RANGE"]);
        EditorGUILayout.Popup("Range", shapeIndex, rangeType);
        consumData["RANGE"] = rangeType[shapeIndex];

        GUI.skin.textField.wordWrap = true;
        consumData["DESC"] = EditorGUILayout.TextField("Description", consumData["DESC"], GUILayout.Height(200));
    }

    private void ViewMonsterData(List<Dictionary<string, string>> monsterList)
    {
        var consumData = monsterList[itemIndex];

        var id = int.Parse(consumData["ID"]);
        consumData["ID"] = EditorGUILayout.IntField("Id", id).ToString();

        consumData["ICON_ID"] = EditorGUILayout.TextField("IconID", consumData["ICON_ID"]);

        consumData["NAME"] = EditorGUILayout.TextField("Name", consumData["NAME"]);

        consumData["LOCAL_ID"] = EditorGUILayout.TextField("Localization ID", consumData["LOCAL_ID"]);

        var grade = new string[] { "Normal", "Epic", "Elite" };
        int gradeIndex = ArrayUtility.IndexOf(grade, consumData["GRADE"]);
        EditorGUILayout.Popup("Grade", gradeIndex, grade);
        consumData["GRADE"] = grade[gradeIndex];

        var group = int.Parse(consumData["GROUP"]);
        consumData["GROUP"] = EditorGUILayout.IntField("Group", group).ToString();

        var type = new string[] { "Near", "Far" };
        int shapeIndex = ArrayUtility.IndexOf(type, consumData["TYPE"]);
        EditorGUILayout.Popup("TableType", shapeIndex, type);
        consumData["TYPE"] = type[shapeIndex];

        var atk = int.Parse(consumData["ATK"]);
        consumData["ATK"] = EditorGUILayout.IntField("ATK", atk).ToString();

        var sheild = int.Parse(consumData["SHEILD"]);
        consumData["SHEILD"] = EditorGUILayout.IntField("Sheild", sheild).ToString();

        var hp = int.Parse(consumData["HP"]);
        consumData["HP"] = EditorGUILayout.IntField("HP", hp).ToString();

        var speedMin = int.Parse(consumData["SPEED_MIN"]);
        consumData["SPEED_MIN"] = EditorGUILayout.IntField("Speed_Min", speedMin).ToString();

        var speedMax = int.Parse(consumData["SPEED_MAX"]);
        consumData["SPEED_MAX"] = EditorGUILayout.IntField("Speed_Max", speedMax).ToString();

        var dodge = int.Parse(consumData["DODGE"]);
        consumData["DODGE"] = EditorGUILayout.IntField("Dodge", dodge).ToString();
    } 
    
    private void ViewLocalizationData(List<Dictionary<string, string>> localizationList)
    {
        var consumData = localizationList[itemIndex];

        consumData["ID"] = EditorGUILayout.TextField("ID", consumData["ID"]);

        consumData["KOR"] = EditorGUILayout.TextField("Korean", consumData["KOR"]);

        consumData["ENG"] = EditorGUILayout.TextField("English", consumData["ENG"]);
    }

    private void ViewRandomEventData(List<Dictionary<string, string>> randomEventList)
    {
        var randomData = randomEventList[itemIndex];

        randomData["ID"] = EditorGUILayout.TextField("ID", randomData["ID"]);
        randomData["NAME"] = EditorGUILayout.TextField("NAME", randomData["NAME"]);
        randomData["EVENTDESC"] = EditorGUILayout.TextField("EVENTDESC", randomData["EVENTDESC"]);
        randomData["EVENTCONDITION"] = EditorGUILayout.TextField("EVENTCONDITION", randomData["EVENTCONDITION"]);
        randomData["EVENTFREQUENCY"] = EditorGUILayout.TextField("EVENTFREQUENCY", randomData["EVENTFREQUENCY"]);
        randomData["EVENTFREQUENCY2"] = EditorGUILayout.TextField("EVENTFREQUENCY2", randomData["EVENTFREQUENCY2"]);
        randomData["SELECT1NAME"] = EditorGUILayout.TextField("SELECT1NAME", randomData["SELECT1NAME"]);
        randomData["SUCESS1CHANCE"] = EditorGUILayout.TextField("SUCESS1CHANCE", randomData["SUCESS1CHANCE"]);
        randomData["SUCESS1TYPE"] = EditorGUILayout.TextField("SUCESS1TYPE", randomData["SUCESS1TYPE"]);
        randomData["SUCESS1ID"] = EditorGUILayout.TextField("SUCESS1ID", randomData["SUCESS1ID"]);
        randomData["SUCESS1VAL"] = EditorGUILayout.TextField("SUCESS1VAL", randomData["SUCESS1VAL"]);
        randomData["SUCESS1DESC"] = EditorGUILayout.TextField("SUCESS1DESC", randomData["SUCESS1DESC"]);
        randomData["FAIL1TYPE"] = EditorGUILayout.TextField("FAIL1TYPE", randomData["FAIL1TYPE"]);
        randomData["FAIL1ID"] = EditorGUILayout.TextField("FAIL1ID", randomData["FAIL1ID"]);
        randomData["FAIL1VAL"] = EditorGUILayout.TextField("FAIL1VAL", randomData["FAIL1VAL"]);
        randomData["FAIL1DESC"] = EditorGUILayout.TextField("FAIL1DESC", randomData["FAIL1DESC"]);
        randomData["SELECT2NAME"] = EditorGUILayout.TextField("SELECT2NAME", randomData["SELECT2NAME"]);
        randomData["SUCESS2CHANCE"] = EditorGUILayout.TextField("SUCESS2CHANCE", randomData["SUCESS2CHANCE"]);
        randomData["SUCESS2TYPE"] = EditorGUILayout.TextField("SUCESS2TYPE", randomData["SUCESS2TYPE"]);
        randomData["SUCESS2ID"] = EditorGUILayout.TextField("SUCESS2ID", randomData["SUCESS2ID"]);
        randomData["SUCESS2VAL"] = EditorGUILayout.TextField("SUCESS2VAL", randomData["SUCESS2VAL"]);
        randomData["SUCESS2DESC"] = EditorGUILayout.TextField("SUCESS2DESC", randomData["SUCESS2DESC"]);
        randomData["FAIL2TYPE"] = EditorGUILayout.TextField("FAIL2TYPE", randomData["FAIL2TYPE"]);
        randomData["FAIL2ID"] = EditorGUILayout.TextField("FAIL2ID", randomData["FAIL2ID"]);
        randomData["FAIL2VAL"] = EditorGUILayout.TextField("FAIL2VAL", randomData["FAIL2VAL"]);
        randomData["FAIL2DESC"] = EditorGUILayout.TextField("FAIL2DESC", randomData["FAIL2DESC"]);
        randomData["SELECT3NAME"] = EditorGUILayout.TextField("SELECT3NAME", randomData["SELECT3NAME"]);
        randomData["SUCESS3CHANCE"] = EditorGUILayout.TextField("SUCESS3CHANCE", randomData["SUCESS3CHANCE"]);
        randomData["SUCESS3TYPE"] = EditorGUILayout.TextField("SUCESS3TYPE", randomData["SUCESS3TYPE"]);
        randomData["SUCESS3ID"] = EditorGUILayout.TextField("SUCESS3ID", randomData["SUCESS3ID"]);
        randomData["SUCESS3VAL"] = EditorGUILayout.TextField("SUCESS3VAL", randomData["SUCESS3VAL"]);
        randomData["SUCESS3DESC"] = EditorGUILayout.TextField("SUCESS3DESC", randomData["SUCESS3DESC"]);
        randomData["FAIL3TYPE"] = EditorGUILayout.TextField("FAIL3TYPE", randomData["FAIL3TYPE"]);
        randomData["FAIL3ID"] = EditorGUILayout.TextField("FAIL3ID", randomData["FAIL3ID"]);
        randomData["FAIL3VAL"] = EditorGUILayout.TextField("FAIL3VAL", randomData["FAIL3VAL"]);
        randomData["FAIL3DESC"] = EditorGUILayout.TextField("FAIL3DESC", randomData["FAIL3DESC"]);
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
        var path = Path.Combine(csvFilePath, tableName);
        CSVWriter.Writer(path, firstElem, tableList);
    }
}