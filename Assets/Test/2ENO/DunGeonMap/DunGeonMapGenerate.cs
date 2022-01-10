using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum DirectionInho
{
    Right,
    Left,
    Top,
    Bottom,
    Count,
}

public class DunGeonMapGenerate : MonoBehaviour
{
    private float distance = 2f;
    private int startIdx = 100;
    private int col = 20;

    private int roomCount = 4;
    private int remainMainRoom;

    public DirectionInho direction;

    public DungeonRoom[] dungeonRoomArray = new DungeonRoom[400];
    //public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();
    //테스트용 (내용 확인용)
    //public List<DungeonRoom> dungeonRoomList = new List<DungeonRoom>();

    public void Init(DungeonSystem system)
    {
        //if(dungeonSystem == null)
        //    dungeonSystem = system;
    }

    public void DungeonGenerate(Difficulty difficulty , DungeonRoom[] mapArrayData, UnityAction action)
    {
        switch (difficulty)
        {
            case Difficulty.easy:
                roomCount = 4;
                break;
            case Difficulty.normal:
                roomCount = 6;
                break;
            case Difficulty.hard:
                roomCount = 8;
                break;
        }
        Vars.UserData.dungeonStartIdx = startIdx;
        if(mapArrayData == null)
        {
            StartCoroutine(MapCorutine(action));
        }
        else
        {
            dungeonRoomArray = mapArrayData;
            action?.Invoke();
            GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
        }
        Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray = dungeonRoomArray;
        Vars.UserData.CurAllDungeonData[Vars.UserData.curDungeonIndex].dungeonStartIdx = startIdx;

    }
    public void DungeonEventGenerate(DungeonRoom[] dungeonArray)
    {
        int curIdx = startIdx;
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
                    curRoom.gatheringCount = Random.Range(0, 3);
                    for (int i = 0; i < curRoom.gatheringCount; i++)
                    {
                        var gatheringData1 = new GatheringData();
                        gatheringData1.eventType = DunGeonEvent.Gathering;
                        gatheringData1.offSetBasePos = (-3 + (i * 3));
                        gatheringData1.roomIndex = curRoom.roomIdx;
                        eventData.Add(gatheringData1);
                    }
                }
                else
                {
                    var gatheringData = new GatheringData();
                    gatheringData.eventType = DunGeonEvent.Gathering;
                    gatheringData.roomIndex = curRoom.roomIdx;
                    curRoom.gatheringCount = 1;
                    gatheringData.offSetBasePos = 2;
                }
                break;
            case DunGeonEvent.Hunt:
                var huntingData = new HuntingData();
                huntingData.eventType = DunGeonEvent.Hunt;
                huntingData.roomIndex = curRoom.roomIdx;
                eventData.Add(huntingData);
                break;
            case DunGeonEvent.RandomIncount:
                break;
            case DunGeonEvent.SubStory:
                break;
            case DunGeonEvent.Count:
                break;
        }
    }

    IEnumerator MapCorutine(UnityAction action)
    {
        MapInit();
        //dungeonSystem.DungeonSystemData.dungeonStartIdx = startIdx;
        while (remainMainRoom > 0)
        {
            // 다시 초기화
            MapInit();
            TestMapSet(startIdx, -1, DirectionInho.Right, 0);
            yield return null;
        }
        DunGeonRoomSetting.DungeonRoadCount(dungeonRoomArray[startIdx], dungeonRoomArray);
        DunGeonRoomSetting.DungeonPathRoomCountSet(dungeonRoomArray[startIdx], dungeonRoomArray);
        DungeonEventGenerate(dungeonRoomArray);

        action?.Invoke();

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.DungeonMap);
    }
    //public void OnGUI()
    //{
    //    if (GUILayout.Button("reStart"))
    //    {
    //        SceneManager.LoadScene(0);
    //    }
    //    if (GUILayout.Button("SaveMap"))
    //    {
    //        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.DungeonMap);
    //    }
    //    if (GUILayout.Button("LoadMap"))
    //    {
    //        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.DungeonMap);
    //        dungeonRoomArray = Vars.UserData.DungeonMapData;
    //        DunGeonRoomSetting.DungeonRoadCount(dungeonRoomArray[startId], dungeonRoomList);
    //        CreateMapObject();
    //    }
    //    if(GUILayout.Button("Success"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = true;
    //        SceneManager.LoadScene("WorldMap");
    //    }
    //    if (GUILayout.Button("Run"))
    //    {
    //        Vars.UserData.WorldMapPlayerData.isClear = false;
    //        SceneManager.LoadScene("WorldMap");
    //    }
    //}

    public void MapInit()
    {
        for (int i = 0; i < dungeonRoomArray.Length; i++)
        {
            dungeonRoomArray[i] = new DungeonRoom();
            dungeonRoomArray[i].IsCheck = false;
            if (i == 0)
            {
                dungeonRoomArray[i].Pos = Vector2.zero;
            }
            else
            {
                var row = i % col;
                var colum = i / col;
                dungeonRoomArray[i].Pos = new Vector2(row * distance, colum * distance);
            }
        }
        remainMainRoom = roomCount;
    }

    public void TestMapSet(int curIdx,int beforeIdx, DirectionInho lastDir, int roadCount)
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
            TestMapSet(nextId, curIdx, curDir, rndCount);
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
            TestMapSet(nextId, curIdx, curDir, roadCount);
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
//public void CreateMapObject()
//{
//    int curIdx = startIdx;
//    while(dungeonRoomArray[curIdx].nextRoomIdx != -1)
//    {
//        var room = dungeonRoomArray[curIdx];
//        if (room.RoomType == DunGeonRoomType.MainRoom)
//        {
//            var obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
//                 , Quaternion.identity, mapPos.transform);
//            var objectInfo = obj.GetComponent<RoomObject>();
//            objectInfo.text.SetText(GetText(room));
//            objectInfo.roomIdx = room.roomIdx;
//            dungeonRoomObjectList.Add(obj);
//        }
//        else
//        {
//            var obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
//            , Quaternion.identity, mapPos.transform);
//            var objectInfo = obj.GetComponent<RoomObject>();
//            objectInfo.text.SetText(GetText(room));
//            objectInfo.roomIdx = room.roomIdx;
//            dungeonRoomObjectList.Add(obj);
//        }
//        curIdx = dungeonRoomArray[curIdx].nextRoomIdx;
//    }
//    mapPos.transform.position = mapPos.transform.position + new Vector3(0f, 30f, 0f);
//}