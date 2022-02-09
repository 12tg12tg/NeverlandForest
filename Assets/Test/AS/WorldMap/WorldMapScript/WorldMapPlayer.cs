using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class WorldMapPlayer : MonoBehaviour
{
    private WorldMapNode[] totalMap;
    private Coroutine coMove;

    private Vector2 currentIndex;
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
        totalMap = GameManager.Manager.WorldManager.worldMapMaker.GetComponentsInChildren<WorldMapNode>();
        //totalMap = transform.GetComponentsInChildren<WorldMapNode>();
        currentIndex = totalMap[0].index;
        transform.position = totalMap[0].transform.position + new Vector3(0f, 0.5f, 0f);

        var data = new WorldMapPlayerData();
        data.startPos = data.currentPos = transform.position;
        data.currentIndex = currentIndex;
        Vars.UserData.WorldMapPlayerData = data;
        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
    }

    public void Load()
    {
        var data = Vars.UserData.WorldMapPlayerData;
        transform.position = data.isClear ? data.goalPos : data.startPos;
        currentIndex = data.currentIndex;
    }

    public void ComeBackWorldMap()
    {
        // TODO : 씬 하나로 합치면 아래코드는 없어져도 될듯 함
        totalMap = GameManager.Manager.WorldManager.GetComponentsInChildren<WorldMapNode>();

        var data = Vars.UserData.WorldMapPlayerData;
        if (data == null) // 게임을 처음 켰을 때
        {
            GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapPlayerData);
            data = Vars.UserData.WorldMapPlayerData;
            if (data == null) 
            {
                Init(); // 플레이어가 죽어서 플레이어 데이터 초기화 됐을 때
            }
            else
            {
                Load();
            }
        }
        else if (data.isClear)
        {
            PlayerClearWorldMap();
        }
        else
        {
            PlayerRunWorldMap();

            // 게임을 처음 켰는데 던전맵이고 던전맵에서 도망갔다면 딱 한번만 실행하면 되는데.. 
            // 다른 방법 생각이 안나서 이래 해둔상태
            GameManager.Manager.CamManager.mainCamera.GetComponent<WorldMapCamera>().RunDungoen();
        }
    }

    public void Return()
    {
        if (!Vars.UserData.isDungeonClear)
            return;

        DungeonEnter(true, Vector3.zero, Vars.UserData.WorldMapPlayerData.currentIndex);
    }

    public void DungeonEnter(bool isReturn, Vector3 goal, Vector2 goalIndex)
    {
        mapGenerator = GameObject.FindWithTag("Dungeon").GetComponent<DunGeonMapGenerate>();

        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.RandomEvent);
        if (!Vars.UserData.isFirst)
        {
            Vars.UserData.isRandomDataLoad = true;
        }
        RandomEventManager.Instance.init();
        Vars.UserData.isFirst = false;

        // 이미 맵이 만들어 졌을때
        if (Vars.UserData.AllDungeonData.ContainsKey(goalIndex))
        {
            if (Vars.UserData.isDungeonClear && Vars.UserData.curDungeonIndex != goalIndex)
            {
                return;
            }
            if (!Vars.UserData.isDungeonClear)
            {
                Vars.UserData.isDungeonReStart = true;
                Vars.UserData.curDungeonIndex = goalIndex;
            }

            var range = (int)(Vars.UserData.WorldMapPlayerData.goalIndex.y - Vars.UserData.WorldMapPlayerData.currentIndex.y);
            mapGenerator.DungeonGenerate(range, Vars.UserData.AllDungeonData[goalIndex].dungeonRoomArray,
                () =>
                {
                    if (isReturn)
                    {
                        GameManager.Manager.LoadScene(GameScene.Dungeon);
                    }
                    else
                    {
                        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, goal, 0.5f, GameScene.Dungeon, () => coMove = null));
                        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
                    }
                });
        }
        // 처음 만들떄
        else
        {
            if (goalIndex.y - Vars.UserData.curDungeonIndex.y > 0)
            {
                Vars.UserData.AllDungeonData.Clear();
            }

            Vars.UserData.isDungeonClear = false;
            Vars.UserData.curDungeonIndex = goalIndex;
            Vars.UserData.isDungeonReStart = true;
            Vars.UserData.AllDungeonData.Add(goalIndex, new DungeonData());
            var range = (int)(Vars.UserData.WorldMapPlayerData.goalIndex.y - Vars.UserData.WorldMapPlayerData.currentIndex.y);
            mapGenerator.DungeonGenerate(range, null, () =>
            {
                if (isReturn)
                {
                    GameManager.Manager.LoadScene(GameScene.Dungeon);
                }
                else
                {
                    coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, goal, 0.5f, GameScene.Dungeon, () => coMove = null));
                    GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
                }
            });

            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.RandomEvent);
        }

    }
    public void PlayerWorldMap(WorldMapNode node)
    {
        Vars.UserData.curNode = node;
        var goal = node.transform.position + new Vector3(0f, 0.5f, 0f);
        transform.LookAt(node.transform);
        GetComponent<Animator>().SetTrigger("Walk");
       
        goalIndex = coMove == null ? node.index : goalIndex;
        goalPos = goal;
        startPos = transform.position;
        var x = transform.position.x + (Mathf.Abs(goal.x - transform.position.x) * distance);

        var z = transform.position.z > goal.z ? transform.position.z - (Mathf.Abs(goal.z - transform.position.z) * distance) :
                transform.position.z < goal.z ? transform.position.z + (Mathf.Abs(goal.z - transform.position.z) * distance) : goal.z;

        currentPos = goal = new Vector3(x, goal.y, z);

        var data = Vars.UserData.WorldMapPlayerData;
        data.goalIndex = goalIndex;
        data.currentPos = currentPos;
        data.goalPos = goalPos;
        data.startPos = startPos;

        DungeonEnter(false, goal, goalIndex);
    }
    private void PlayerClearWorldMap()
    {
        var ani = GetComponent<Animator>();
        ani.SetTrigger("Walk");
        var data = Vars.UserData.WorldMapPlayerData;
        data.currentIndex = currentIndex = data.goalIndex;
        transform.position = data.currentPos;

        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, data.goalPos, 1f, () =>
        {
            coMove = null;
            ani.SetTrigger("Idle");
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
        }));

        var trans = totalMap.Where(x => x.index.Equals(data.goalIndex))
                            .Select(x => x.transform).FirstOrDefault();
        transform.LookAt(trans);
    }
    private void PlayerRunWorldMap()
    {
        var ani = GetComponent<Animator>();
        ani.SetTrigger("Walk");
        var data = Vars.UserData.WorldMapPlayerData;
        currentIndex = data.currentIndex;
        transform.position = data.currentPos;
        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, data.startPos, 0.5f, () =>
        {
            coMove = null;
            ani.SetTrigger("Idle");
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
        }));

        var trans = totalMap.Where(x => x.index.Equals(currentIndex))
                            .Select(x => x.transform).FirstOrDefault();
        transform.LookAt(trans);
    }
    public void PlayerDeathChack() // 안개에 닿았을 때
    {
        var y = (int)currentIndex.y;
        if (y <= Vars.UserData.uData.Date - 3)
        {
            GameManager.Manager.GameOver(GameOverType.WitchCaught);
        }
    }
}
