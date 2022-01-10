using System.Collections;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class WorldMapPlayer : MonoBehaviour
{
    public WorldMapManager map;
    private WorldMapNode[] totalMap;
    private Coroutine coMove;

    public Vector2 currentIndex;
    private Vector2 goalIndex;

    private Vector3 startPos;
    private Vector3 currentPos;
    private Vector3 goalPos;

    public float distance;
    public string sceneName;

    public Vector2 CurrentIndex => currentIndex;

    private DunGeonMapGenerate mapGenerator;

    public void Init()
    {
        TotalMapInit();
        currentIndex = totalMap[0].index;
        transform.position = totalMap[0].transform.position + new Vector3(0f, 0.5f, 0f);

        var data = new WorldMapPlayerData();
        data.startPos = data.currentPos = transform.position;
        data.currentIndex = currentIndex;
        Vars.UserData.WorldMapPlayerData = data;
        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
    }

    private void TotalMapInit()
    {
        totalMap = new WorldMapNode[map.transform.childCount];
        for (int i = 0; i < map.transform.childCount; i++)
        {
            totalMap[i] = map.transform.GetChild(i).GetComponent<WorldMapNode>();
        }
        totalMap.OrderBy(n => n.level);
    }

    public void Load()
    {
        var data = Vars.UserData.WorldMapPlayerData;
        transform.position = data.isClear ? data.goalPos : data.startPos;
        currentIndex = data.currentIndex;
    }

    public void ComeBackWorldMap()
    {
        TotalMapInit();
        if (Vars.UserData.WorldMapPlayerData == null)
        {
            GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapPlayerData);
            if (Vars.UserData.WorldMapPlayerData == null)
                Init();
            else
                Load();
        }
        else if(Vars.UserData.WorldMapPlayerData.isClear)
            PlayerClearWorldMap();
        else
            PlayerRunWorldMap();
    }

    public void PlayerWorldMap(WorldMapNode node)
    {
        var goal = node.transform.position + new Vector3(0f, 0.5f, 0f);
        transform.LookAt(node.transform);
        GetComponent<Animator>().SetTrigger("Walk");
        mapGenerator = GameObject.FindWithTag("Dungeon").GetComponent<DunGeonMapGenerate>();
        goalIndex = coMove == null ? node.index : goalIndex;
        goalPos = goal;
        startPos = transform.position;
        var x = transform.position.x + (Mathf.Abs(goal.x - transform.position.x) * distance);
        var z = goal.z;

        if(transform.position.z > goal.z)
        {
            z = transform.position.z - (Mathf.Abs(goal.z - transform.position.z) * distance);
        }
        else if (transform.position.z < goal.z)
        {
            z = transform.position.z + (Mathf.Abs(goal.z - transform.position.z) * distance);
        }

        currentPos = goal = new Vector3(x, goal.y, z);

        var data = Vars.UserData.WorldMapPlayerData;
        data.goalIndex = goalIndex;
        data.currentPos = currentPos;
        data.goalPos = goalPos;
        data.startPos = startPos;

        // 이미 맵이 만들어 졌을때
        if (Vars.UserData.CurAllDungeonData.ContainsKey(goalIndex))
        {
            Vars.UserData.curDungeonIndex = goalIndex;
            Vars.UserData.dungeonReStart = true;
            mapGenerator.DungeonGenerate(node.difficulty ,Vars.UserData.CurAllDungeonData[goalIndex].dungeonRoomArray,
                () =>
                {
                    coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, goal, 0.5f, "AS_RandomMap", () => coMove = null));
                    GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
                }
                );
        }
        // 처음 만들떄
        else
        {
            Vars.UserData.curDungeonIndex = goalIndex;
            Vars.UserData.dungeonReStart = true;
            Vars.UserData.CurAllDungeonData.Add(goalIndex, new DungeonData());
            mapGenerator.DungeonGenerate(node.difficulty, null, () =>
            {
                coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, goal, 0.5f, "AS_RandomMap", () => coMove = null));
                GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
            }
            );
            //Vars.UserData.curLevelDungeonMaps.Add(goalIndex, mapGenerator.dungeonRoomArray);
        }
    }
    public void PlayerClearWorldMap()
    {
        var ani = GetComponent<Animator>();
        ani.SetTrigger("Walk");
        var data = Vars.UserData.WorldMapPlayerData;
        data.currentIndex = currentIndex = data.goalIndex;
        transform.position = data.currentPos;

        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, data.goalPos, 1f, () => { 
            coMove = null; 
            ani.SetTrigger("Idle");
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
        }));

        for (int i = 0; i < totalMap.Length; i++)
        {
            if (totalMap[i].index.Equals(data.goalIndex))
            {
                transform.LookAt(totalMap[i].transform);
                break;
            }
        }

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
    }

    public void PlayerRunWorldMap()
    {
        var ani = GetComponent<Animator>();
        ani.SetTrigger("Walk");
        var data = Vars.UserData.WorldMapPlayerData;
        currentIndex = data.currentIndex;
        transform.position = data.currentPos;
        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, data.startPos, 0.5f, () => { 
            coMove = null; 
            ani.SetTrigger("Idle");
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
        }));

        for (int i = 0; i < totalMap.Length; i++)
        {
            if (totalMap[i].index.Equals(data.currentIndex))
            {
                transform.LookAt(totalMap[i].transform);
                break;
            }
        }

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
    }
}