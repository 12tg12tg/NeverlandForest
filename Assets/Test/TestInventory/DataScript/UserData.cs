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
    public DungeonData CurAllDungeonData { get; set; } = new DungeonData();
    public int dungeonStartIdx;

    public Dictionary<Vector2, DungeonRoom[]> curLevelDungeonMaps = new Dictionary<Vector2, DungeonRoom[]>();

    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();

    //??? - Vars로 이사가도 되는가?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    // 위랑 사실 비슷한 내용인데 보는쪽 가독성을 위해 따로 이름 지어서 만듬, 인벤토리에 사용

    public List<DataAllItem> HaveAllItemList { get; set; } = new List<DataAllItem>();

    public Dictionary<string, DataAllItem> HaveAllItemList2 { get; set; } = new Dictionary<string, DataAllItem>();



    public int maxStamina { get; set; } = 100;
    public int curStamina { get => baseStamina + hunger + tiredness; set { } }
    public int baseStamina { get; set; } = 20; //임시의 스태미나 기본수치
    public int hunger { get; set; } = 40;
    public int tiredness { get; set; } = 40;
    public int curIngameHour { get; set; } = 0;
    public int curIngameMinute { get; set; } = 0;

    public int date = 1;
}
