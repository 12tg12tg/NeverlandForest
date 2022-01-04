using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public int id;
    public string nickname;

    public List<DataCharacter> characterList = new List<DataCharacter>();

    public List<DataConsumable> consumableItemList = new List<DataConsumable>();
    public List<DataWeapon> weaponItemList = new List<DataWeapon>();
    public List<DataMaterial> HaveMaterialList = new List<DataMaterial>();
    public List<string> HaveRecipeIDList = new List<string>();
    public Dictionary<string, float> MakeList = new Dictionary<string, float>();
    public List<MapNodeStruct_0> WorldMapNodeStruct { get; set; } = new List<MapNodeStruct_0>();

    public List<DataPlayerSkill> boySkillList = new List<DataPlayerSkill>();
    public List<DataPlayerSkill> girlSkillList = new List<DataPlayerSkill>();

    public DungeonRoom[] dungeonMapData;
    public DungeonData curAllDungeonData = new DungeonData();
}
