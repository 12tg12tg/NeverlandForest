using System.Collections.Generic;

public class UserData
{
    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!      �������� �ٸ� ������       !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    //Item Info
    public List<DataConsumable> ConsumableItemList { get; set; } = new List<DataConsumable>();

    //World Info
    public List<MapNodeStruct_0> WorldMapNodeStruct { get; set; } = new List<MapNodeStruct_0>();
    public WorldMapPlayerData WorldMapPlayerData { get; set; }

    //Dungeon Info
    public DungeonRoom[] DungeonMapData { get; set; }
    public DungeonData CurAllDungeonData { get; set; } = new DungeonData();

    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();

    //??? - Vars�� �̻簡�� �Ǵ°�?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    //ConsumeManager
    public int maxStamina { get; set; } = 100;
    public int curStamina { get => baseStamina + hunger + tiredness; set { } }
    public int baseStamina { get; set; } = 20; //�ӽ��� ���¹̳� �⺻��ġ
    public int hunger { get; set; } = 40;
    public int tiredness { get; set; } = 40;
    public int curIngameHour { get; set; } = 0;
    public int curIngameMinute { get; set; } = 0;

    public int date = 1;
}
