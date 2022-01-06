using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!      유저마다 다른 변수들       !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    //Item Info
    public List<DataConsumable> ConsumableItemList { get; set; } = new List<DataConsumable>();

    //World Info
    public List<MapNodeStruct_0> WorldMapNodeStruct { get; set; } = new List<MapNodeStruct_0>();
    public WorldMapPlayerData WorldMapPlayerData { get; set; }

    // 던전맵 데이터, 세이브 로드
    public DungeonRoom[] DungeonMapData { get; set; }
    // 인게임 동작 나중에 위랑 하나로 합치는 정리 필요
    public Dictionary<Vector2, DungeonData> CurAllDungeonData { get; set; } = new Dictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;

    public Dictionary<Vector2, DungeonRoom[]> curLevelDungeonMaps = new Dictionary<Vector2, DungeonRoom[]>();

    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();

    //??? - Vars로 이사가도 되는가?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    //ConsumeManager
    //피로도는 실질적으로 줄어드는 수치
    public int ChangeableMaxStamina { get => Vars.maxStamina - Hunger; set { } }
    public int Hunger { get; set; } = 70;
    // public int Tiredness { get=> Vars.maxStamina - Tired; set { } }
    public int Tiredness { get; set; } 
    public int CurStamina { get =>Tiredness; set { } }
    public int CurIngameHour { get; set; } = 0;
    public int CurIngameMinute { get; set; } = 0;
    public int LanternCount = 18;

    // 위랑 사실 비슷한 내용인데 보는쪽 가독성을 위해 따로 이름 지어서 만듬, 인벤토리에 사용

    public List<DataAllItem> HaveAllItemList { get; set; } = new List<DataAllItem>();

    public Dictionary<string, DataAllItem> HaveAllItemList2 { get; set; } = new Dictionary<string, DataAllItem>();

    public int date = 1;
    public float hunterHp = 100;
    public float herbalistHp = 100;

}
