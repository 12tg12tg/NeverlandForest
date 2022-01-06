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
    //�Ƿε��� ���������� �پ��� ��ġ
    public int ChangeableMaxStamina { get => Vars.maxStamina - Hunger; set { } }
    public int Hunger { get; set; } = 70;
    // public int Tiredness { get=> Vars.maxStamina - Tired; set { } }
    public int Tiredness { get; set; } 
    public int CurStamina { get =>Tiredness; set { } }
    public int CurIngameHour { get; set; } = 0;
    public int CurIngameMinute { get; set; } = 0;
    public int LanternCount = 18;
    public int date = 1;

    public float hunterHp = 100;
    public float herbalistHp = 100;

}
