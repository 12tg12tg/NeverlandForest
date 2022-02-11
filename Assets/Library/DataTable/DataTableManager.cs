using System;
using System.Collections.Generic;

[Serializable]
public static class DataTableManager
{
    public static Dictionary<System.Type, DataTableBase> tables = 
        new Dictionary<System.Type, DataTableBase>();
    private static bool inited = false;

    public static void Init()
    {
        tables.Clear();

        var itemTable = new ConsumableTable();
        itemTable.Load();
        tables.Add(typeof(ConsumableTable), itemTable);

        var armorTable = new ArmorTable();
        armorTable.Load();
        tables.Add(typeof(ArmorTable), armorTable);

        var weaponTable = new WeaponTable();
        weaponTable.Load();
        tables.Add(typeof(WeaponTable), weaponTable);

        var allItemTable = new AllItemDataTable();
        allItemTable.Load();
        tables.Add(typeof(AllItemDataTable), allItemTable);

        var recipeTable = new RecipeDataTable();
        recipeTable.Load();
        tables.Add(typeof(RecipeDataTable), recipeTable);

        var playerSkillTable = new PlayerSkillTable();
        playerSkillTable.Load();
        tables.Add(typeof(PlayerSkillTable), playerSkillTable);

        var monsterTable = new MonsterTable();
        monsterTable.Load();
        tables.Add(typeof(MonsterTable), monsterTable);

        var localizationTable = new LocalizationTable();
        localizationTable.Load();
        tables.Add(typeof(LocalizationTable), localizationTable);

        var randomEventTable = new RandomEventTable();
        randomEventTable.Load();
        tables.Add(typeof(RandomEventTable), randomEventTable);


        var craftTable = new CraftDataTable();
        craftTable.Load();
        tables.Add(typeof(CraftDataTable), craftTable);

        var stroyTable = new TutorialStoryDataTable();
        stroyTable.Load();
        tables.Add(typeof(TutorialStoryDataTable), stroyTable);

        var memoTable = new MemoTable();
        memoTable.Load();
        tables.Add(typeof(MemoTable), memoTable);
    }

    public static T GetTable<T>() where T : DataTableBase
    {
        if (!inited)
        {
            Init();
            inited = true;
        }

        if (!tables.ContainsKey(typeof(T)))
            return null;
        return (T)tables[typeof(T)];
    }
}