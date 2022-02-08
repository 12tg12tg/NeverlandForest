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
        totalMap = GameManager.Manager.wm.worldMapMaker.GetComponentsInChildren<WorldMapNode>();
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
        // TODO : �� �ϳ��� ��ġ�� �Ʒ��ڵ�� �������� �ɵ� ��
        totalMap = GameManager.Manager.wm.GetComponentsInChildren<WorldMapNode>();

        var data = Vars.UserData.WorldMapPlayerData;
        if (data == null) // ������ ó�� ���� ��
        {
            GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.WorldMapPlayerData);
            data = Vars.UserData.WorldMapPlayerData;
            if (data == null) 
            {
                Init(); // �÷��̾ �׾ �÷��̾� ������ �ʱ�ȭ ���� ��
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

            // ������ ó�� �״µ� �������̰� �����ʿ��� �������ٸ� �� �ѹ��� �����ϸ� �Ǵµ�.. 
            // �ٸ� ��� ������ �ȳ��� �̷� �صл���
            GameManager.Manager.cm.mainCamera.GetComponent<WorldMapCamera>().RunDungoen();
        }
    }

    public void PlayerWorldMap(WorldMapNode node)
    {
        if (coMove != null)
            return;
        var goal = node.transform.position + new Vector3(0f, 0.5f, 0f);
        transform.LookAt(node.transform);
        GetComponent<Animator>().SetTrigger("Walk");
        mapGenerator = GameObject.FindWithTag("Dungeon").GetComponent<DunGeonMapGenerate>();
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

        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.RandomEvent);
        if(!Vars.UserData.isFirst)
        {
            Vars.UserData.isRandomDataLoad = true;
        }
        RandomEventManager.Instance.init();
        Vars.UserData.isFirst = false;

        // �̹� ���� ����� ������
        if (Vars.UserData.AllDungeonData.ContainsKey(goalIndex))
        {
            if (Vars.UserData.curDungeonIndex == goalIndex)
            {
                Vars.UserData.dungeonReStart = false;
            }
            else
            {
                Vars.UserData.curDungeonIndex = goalIndex;
                Vars.UserData.dungeonReStart = true;
            }

            var range = (int)(Vars.UserData.WorldMapPlayerData.goalIndex.y - Vars.UserData.WorldMapPlayerData.currentIndex.y);
            mapGenerator.DungeonGenerate(range, Vars.UserData.AllDungeonData[goalIndex].dungeonRoomArray,
                () =>
                {
                    coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, goal, 0.5f, "AS_RandomMap", () => coMove = null));
                    GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
                });
        }
        // ó�� ���鋚
        else
        {
            if (goalIndex.y - Vars.UserData.curDungeonIndex.y > 0)
            {
                Vars.UserData.AllDungeonData.Clear();
            }

            Vars.UserData.curDungeonIndex = goalIndex;
            Vars.UserData.dungeonReStart = true;
            Vars.UserData.AllDungeonData.Add(goalIndex, new DungeonData());
            var range = (int)(Vars.UserData.WorldMapPlayerData.goalIndex.y - Vars.UserData.WorldMapPlayerData.currentIndex.y);
            mapGenerator.DungeonGenerate(range, null, () =>
            {
                coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, goal, 0.5f, "AS_RandomMap", () => coMove = null));
                GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.WorldMapPlayerData);
            });

            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.RandomEvent);
        }
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
    public void PlayerDeathChack() // �Ȱ��� ����� ��
    {
        var y = (int)currentIndex.y;
        if (y <= Vars.UserData.uData.Date - 3)
        {
            GameManager.Manager.GameOver(GameOverType.WitchCaught);
        }
    }
}
