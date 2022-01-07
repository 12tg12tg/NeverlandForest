using System.Collections.Generic;
using UnityEngine;

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

    // ������ ������, ���̺� �ε�
    public DungeonRoom[] DungeonMapData { get; set; }
    // �ΰ��� ���� ���߿� ���� �ϳ��� ��ġ�� ���� �ʿ�
    public Dictionary<Vector2, DungeonData> CurAllDungeonData { get; set; } = new Dictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;

    public Dictionary<Vector2, DungeonRoom[]> curLevelDungeonMaps = new Dictionary<Vector2, DungeonRoom[]>();

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

    // ���� ��� ����� �����ε� ������ �������� ���� ���� �̸� ��� ����, �κ��丮�� ���

    public List<DataAllItem> HaveAllItemList { get; set; } = new List<DataAllItem>();

    public Dictionary<string, DataAllItem> HaveAllItemList2 { get; set; } = new Dictionary<string, DataAllItem>();

    public int date = 1;
    public float hunterHp = 100;
    public float herbalistHp = 100;
}
