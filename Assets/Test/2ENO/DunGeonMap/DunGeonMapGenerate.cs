using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public enum DirectionInho
{
    Right,
    Left,
    Top,
    Bottom,
    Count,
}

//[DefaultExecutionOrder(13)]
public class DunGeonMapGenerate : MonoBehaviour
{
    private float distance = 2f;
    private int col = 20;

    private int roomCount = 4;
    private int remainMainRoom;

    public DirectionInho direction;

    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    public DungeonRoom[] tutorialRoomArray = new DungeonRoom[4];

    //테스트용 (내용 확인용)
    //public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    private void Start()
    {
        if (Vars.UserData.mainTutorial == MainTutorialStage.Clear)
            return;

        var manager = GameManager.Manager;



        if (manager.TutoManager == null || manager.TutoManager.mainTutorial == null)
            return;

        if (manager.TutoManager.mainTutorial.MainTutorialStage != MainTutorialStage.Clear)
        {
            manager.Production.FadeOut(() => TutorialDungeonGenerate());
        }
    }

    public void OnGUI()
    {
        //if (GUI.Button(new Rect(100, 300, 100, 75), "tutorialStart"))
        //{
        //    TutorialDungeonGenerate();
        //}
    }

    public void TutorialDungeonGenerate()
    {
        CreateTutorialMapArray();
        Vars.UserData.tutorialDungeonData.dungeonRoomArray = tutorialRoomArray;

        DungeonSystem.Instance.TutorialInit();
        DungeonSystem.Instance.Init();
    }

    public void DungeonGenerate(int range , DungeonRoom[] mapArrayData, UnityAction action)
    {
        Debug.Log(range);
        switch (range)
        {
            case 1:
                roomCount = 4;
                break;
            case 2:
                roomCount = 6;
                break;
            case 3:
                roomCount = 8;
                break;
        }
        if(mapArrayData == null)
        {
            StartCoroutine(MapGenerateCorutine(action));
        }
        else
        {
            dungeonRoomArray = mapArrayData;
            action?.Invoke();
        }
        Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray = dungeonRoomArray;

        int curIdx = Vars.UserData.dungeonStartIdx;
        while(dungeonRoomArray[curIdx].nextRoomIdx != -1)
        {
            curIdx = dungeonRoomArray[curIdx].nextRoomIdx;
        }
        Vars.UserData.dungeonLastIdx = dungeonRoomArray[curIdx].roomIdx;
        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }
    public void DungeonEventGenerate(DungeonRoom[] dungeonArray)
    {
        int curIdx;
        if (Vars.UserData.isTutorialDungeon)
            curIdx = 0;
        else
            curIdx = Vars.UserData.dungeonStartIdx;

        while(dungeonArray[curIdx].nextRoomIdx != -1)
        {
            // 현재방의 이벤트 리스트 (1개~2개) 돌면서 이벤트 오브젝트 정보 셋
            for (int i = 0; i < dungeonArray[curIdx].eventList.Count; i++)
            {
                EventDataInit(dungeonArray[curIdx], dungeonArray[curIdx].eventObjDataList, dungeonArray[curIdx].eventList[i]);
            }
            curIdx = dungeonArray[curIdx].nextRoomIdx;
        }
    }

    public void EventDataInit(DungeonRoom curRoom ,List<EventData> eventData, DunGeonEvent evnetTypeList)
    {
        switch (evnetTypeList)
        {
            case DunGeonEvent.Empty:
                break;
            case DunGeonEvent.Battle:
                var battleData = new BattleData();
                battleData.eventType = DunGeonEvent.Battle;
                battleData.roomIndex = curRoom.roomIdx;
                eventData.Add(battleData);
                break;
            case DunGeonEvent.Gathering:
                if (curRoom.RoomType == DunGeonRoomType.MainRoom)
                {
                    curRoom.gatheringCount = 2;
                    for (int i = 0; i < curRoom.gatheringCount; i++)
                    {
                        var gatheringData1 = new GatheringData();
                        gatheringData1.eventType = DunGeonEvent.Gathering;
                        gatheringData1.offSetBasePos = (-8 + (i * 16));
                        gatheringData1.roomIndex = curRoom.roomIdx;
                        gatheringData1.gatheringtype = (GatheringObjectType)Random.Range(0, 4);
                        eventData.Add(gatheringData1);
                    }
                }
                else
                {
                    var gatheringData = new GatheringData();
                    gatheringData.eventType = DunGeonEvent.Gathering;
                    gatheringData.roomIndex = curRoom.roomIdx;
                    if (Vars.UserData.isTutorialDungeon)
                        gatheringData.gatheringtype = GatheringObjectType.Pit;
                    else
                        gatheringData.gatheringtype = (GatheringObjectType)Random.Range(0, 4);
                    curRoom.gatheringCount = 1;
                    gatheringData.offSetBasePos = 2;
                    eventData.Add(gatheringData);
                }
                break;
            case DunGeonEvent.Hunt:
                var huntingData = new HuntingData();
                huntingData.eventType = DunGeonEvent.Hunt;
                huntingData.roomIndex = curRoom.roomIdx;
                eventData.Add(huntingData);
                break;
            case DunGeonEvent.RandomIncount:
                var randomData = new RandomIncountData();
                randomData.eventType = DunGeonEvent.RandomIncount;
                randomData.roomIndex = curRoom.roomIdx;
                eventData.Add(randomData);
                RandomEventManager.Instance.CreateRandomEvent(randomData);
                break;
            case DunGeonEvent.SubStory:
                break;
            case DunGeonEvent.Count:
                break;
        }
        
    }

    IEnumerator MapGenerateCorutine(UnityAction action)
    {
        MapInit();
        Debug.Log("3");
        while (remainMainRoom > 0)
        {
            // 다시 초기화
            MapInit();
            Debug.Log("4");
            CreateMapArray(Vars.UserData.dungeonStartIdx, -1, DirectionInho.Right, 0);
            yield return null;
        }
        DunGeonRoomSetting.DungeonRoadCount(dungeonRoomArray[Vars.UserData.dungeonStartIdx], dungeonRoomArray);
        DunGeonRoomSetting.DungeonPathRoomCountSet(dungeonRoomArray[Vars.UserData.dungeonStartIdx], dungeonRoomArray);
        DungeonEventGenerate(dungeonRoomArray);
        
        action?.Invoke();

        //GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }
    public void MapInit()
    {
        for (int i = 0; i < dungeonRoomArray.Length; i++)
        {
            dungeonRoomArray[i] = new DungeonRoom();
            dungeonRoomArray[i].IsCheck = false;
            if (i == 0)
            {
                dungeonRoomArray[i].Pos = new Vector2(-500f, -1200f);
            }
            else
            {
                var row = i % col;
                var colum = i / col;
                dungeonRoomArray[i].Pos = new Vector2(-500f + row * distance * 15f, -1200f + colum * distance * 15f);
            }
        }
        remainMainRoom = roomCount;
    }

    public void CreateTutorialMapArray()
    {
        // MapInit()
        for (int i = 0; i < tutorialRoomArray.Length; i++)
        {
            tutorialRoomArray[i] = new DungeonRoom();
            tutorialRoomArray[i].IsCheck = false;
            if (i == 0)
            {
                tutorialRoomArray[i].Pos = new Vector2(-500f, -1200f);
            }
            else
            {
                var row = i % col;
                var colum = i / col;
                tutorialRoomArray[i].Pos = new Vector2(-500f + row * distance * 15f, -1200f + colum * distance * 15f);
            }
        }

        tutorialRoomArray[0].IsCheck = true;
        tutorialRoomArray[0].RoomType = DunGeonRoomType.MainRoom;
        tutorialRoomArray[0].SetEvent(DunGeonEvent.Empty);
        tutorialRoomArray[0].beforeRoomIdx = -1;

        tutorialRoomArray[1].IsCheck = true;
        tutorialRoomArray[1].RoomType = DunGeonRoomType.SubRoom;
        tutorialRoomArray[1].SetEvent(DunGeonEvent.Empty);

        tutorialRoomArray[2].IsCheck = true;
        tutorialRoomArray[2].RoomType = DunGeonRoomType.SubRoom;
        tutorialRoomArray[2].SetEvent(DunGeonEvent.Gathering);

        tutorialRoomArray[3].IsCheck = true;
        tutorialRoomArray[3].RoomType = DunGeonRoomType.MainRoom;
        tutorialRoomArray[3].SetEvent(DunGeonEvent.Empty);
        tutorialRoomArray[3].nextRoomIdx = -1;

        for (int i = 0; i < 4; i++)
        {
            tutorialRoomArray[i].roomIdx = i;
            if (i != 0)
                tutorialRoomArray[i].beforeRoomIdx = i - 1;

            if(i != 3)
                tutorialRoomArray[i].nextRoomIdx = i + 1;
        }

        DunGeonRoomSetting.DungeonRoadCount(tutorialRoomArray[0], tutorialRoomArray);
        DunGeonRoomSetting.DungeonPathRoomCountSet(tutorialRoomArray[0], tutorialRoomArray);
        DungeonEventGenerate(tutorialRoomArray);

    }
    public void CreateMapArray(int curIdx,int beforeIdx, DirectionInho lastDir, int roadCount)
    {
        if (remainMainRoom <= 0)
            return;

        if (!RoomException(curIdx))
            return;

        // 메인방 생성
        if (roadCount <= 0)
        {
            dungeonRoomArray[curIdx].IsCheck = true;
            dungeonRoomArray[curIdx].RoomType = DunGeonRoomType.MainRoom;
            dungeonRoomArray[curIdx].roomIdx = curIdx;
            dungeonRoomArray[curIdx].beforeRoomIdx = beforeIdx;
            DunGeonRoomSetting.RoomEventSet(dungeonRoomArray[curIdx]);
            remainMainRoom--;

            // 랜덤 방향
            var rnd = Random.Range(0, (int)DirectionInho.Count);
            while (rnd == (int)oppsiteDir(lastDir)) // 왔던곳 되돌아가는 방향은 그냥 여기서 예외처리
            {
                rnd = Random.Range(0, (int)DirectionInho.Count);
                if (rnd == 1) // 왼쪽방향도 그냥 예외처리
                    rnd = (int)oppsiteDir(lastDir);
            }
            var curDir = (DirectionInho)rnd;
            // 랜덤 길방 개수 생성
            var rndCount = Random.Range(2, 6);

            var nextId = NextRoomId(curIdx, curDir);
            
            dungeonRoomArray[curIdx].nextRoomIdx = nextId;
            // 마지막 방
            if (remainMainRoom <= 0)
            {
                dungeonRoomArray[curIdx].nextRoomIdx = -1;
                dungeonRoomArray[curIdx].eventList.Clear();
                dungeonRoomArray[curIdx].eventList.Add(DunGeonEvent.Battle);
                return;
            }
            CreateMapArray(nextId, curIdx, curDir, rndCount);
        }
        // 길방 생성
        else
        {
            roadCount--;
            dungeonRoomArray[curIdx].IsCheck = true;
            dungeonRoomArray[curIdx].RoomType = DunGeonRoomType.SubRoom;
            dungeonRoomArray[curIdx].roomIdx = curIdx;
            dungeonRoomArray[curIdx].beforeRoomIdx = beforeIdx;
            DunGeonRoomSetting.RoomEventSet(dungeonRoomArray[curIdx]);

            var perCent = Random.Range(0, 10);
            // 이전 방향과 같은 방향, 즉 직진
            var curDir = lastDir;
            var nextId = NextRoomId(curIdx, curDir);
            // 확률적 방향 꺽기
            if (perCent > 7)
            {
                var rnd = Random.Range(0, (int)DirectionInho.Count);
                while (rnd == (int)oppsiteDir(lastDir))
                {
                    rnd = Random.Range(0, (int)DirectionInho.Count);
                    if (rnd == 1) // 왼쪽방향도 그냥 예외처리
                        rnd = (int)oppsiteDir(lastDir);
                }
                curDir = (DirectionInho)rnd;
                nextId = NextRoomId(curIdx, curDir);
            }
            dungeonRoomArray[curIdx].nextRoomIdx = nextId;
            CreateMapArray(nextId, curIdx, curDir, roadCount);
        }
        return;
    }

    public bool RoomException(int curId)
    {
        if (curId == -1)
            return false;

        if (dungeonRoomArray[curId].IsCheck == true)
            return false;

        return true;
    }

    public DirectionInho oppsiteDir(DirectionInho dir)
    {
        var direction = dir;
        switch (direction)
        {
            case DirectionInho.Right:
                direction = DirectionInho.Left;
                break;
            case DirectionInho.Left:
                direction = DirectionInho.Right;
                break;
            case DirectionInho.Top:
                direction = DirectionInho.Bottom;
                break;
            case DirectionInho.Bottom:
                direction = DirectionInho.Top;
                break;
            case DirectionInho.Count:
                break;
        }

        return direction;
    }

    public int NextRoomId(int currentId, DirectionInho dir)
    {
        int result = -1;
        switch (dir)
        {
            case DirectionInho.Right:
                if (currentId % col == col - 1)
                {
                    break;
                }
                result = currentId + 1;
                break;
            case DirectionInho.Left:
                //if (currentId % col == 0)  왼쪽으로 못가게 (기획 요청)
                //{
                //    break;
                //}
                //result = currentId - 1;
                break;
            case DirectionInho.Top:
                if (currentId < col)
                {
                    break;
                }
                result = currentId - col;
                break;
            case DirectionInho.Bottom:
                if (col * (col - 1) <= currentId && currentId < col * col)
                {
                    break;
                }
                result = currentId + col;
                break;
        }
        if (result < -1)
        {
            Debug.Log("d");
        }
        return result;
    }
}

