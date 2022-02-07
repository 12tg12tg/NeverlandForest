using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectGenerate : MonoBehaviour
{
    public BattleObject battleObjPrefab;
    public GameObject huntingObjPrefab;
    public RandomEventObject randomEventObjPrefab;
    public GatheringObject treeObj;
    public GatheringObject pitObj;
    public GatheringObject herbsObj;
    public GatheringObject mushroomObj;
    private List<GameObject> eventObjInstanceList = new List<GameObject>();
    private GatheringSystem gatheringSystem;

    public void Init()
    {
        gatheringSystem = DungeonSystem.Instance.gatheringSystem;
    }

    public void EventObjectClear()
    {
        foreach (var obj in eventObjInstanceList)
        {
            Destroy(obj);
        }
        eventObjInstanceList.Clear();
    }

    public void EventObjectCreate(DungeonRoom roomData)
    {
        // 길방의 오브젝트들 생성
        if (roomData.RoomType == DunGeonRoomType.SubRoom)
        {
            while (roomData.RoomType != DunGeonRoomType.MainRoom)
            {
                foreach (var eventObj in roomData.eventObjDataList)
                {
                    {
                        switch (eventObj.eventType)
                        {
                            case DunGeonEvent.Battle:
                                var createBt = eventObj as BattleData;
                                var obj = createBt.CreateObj(battleObjPrefab);
                                eventObjInstanceList.Add(obj.gameObject);
                                break;
                            case DunGeonEvent.Gathering:
                                var createGt = eventObj as GatheringData;
                                GatheringObject obj2;
                                switch (eventObj.gatheringtype)
                                {
                                    case GatheringObjectType.Tree:
                                        obj2 = createGt.CreateObj(treeObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                    case GatheringObjectType.Pit:
                                        obj2 = createGt.CreateObj(pitObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                    case GatheringObjectType.Herbs:
                                        obj2 = createGt.CreateObj(herbsObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                    case GatheringObjectType.Mushroom:
                                        obj2 = createGt.CreateObj(mushroomObj, gatheringSystem);
                                        eventObjInstanceList.Add(obj2.gameObject);
                                        break;
                                }
                                break;
                            case DunGeonEvent.Hunt:
                                var createHt = eventObj as HuntingData;
                                var obj3 = createHt.CreateObj(huntingObjPrefab);
                                eventObjInstanceList.Add(obj3.gameObject);
                                break;
                            case DunGeonEvent.RandomIncount:
                                var createRi = eventObj as RandomIncountData;
                                var obj4 = createRi.CreateObj(randomEventObjPrefab);
                                eventObjInstanceList.Add(obj4.gameObject);
                                break;
                            case DunGeonEvent.SubStory:
                                break;
                        }
                    }
                }
                roomData = DungeonSystem.Instance.roomTool.GetNextRoom(roomData);
            }
        }
        else
        {
            foreach (var eventObj in roomData.eventObjDataList)
            {
                {
                    switch (eventObj.eventType)
                    {
                        case DunGeonEvent.Battle:
                            var createBt = eventObj as BattleData;
                            var obj = createBt.CreateObj(battleObjPrefab);
                            eventObjInstanceList.Add(obj.gameObject);
                            break;
                        case DunGeonEvent.Gathering:
                            var createGt = eventObj as GatheringData;
                            GatheringObject obj2;
                            switch (eventObj.gatheringtype)
                            {
                                case GatheringObjectType.Tree:
                                    obj2 = createGt.CreateObj(treeObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                                case GatheringObjectType.Pit:
                                    obj2 = createGt.CreateObj(pitObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                                case GatheringObjectType.Herbs:
                                    obj2 = createGt.CreateObj(herbsObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                                case GatheringObjectType.Mushroom:
                                    obj2 = createGt.CreateObj(mushroomObj, gatheringSystem);
                                    eventObjInstanceList.Add(obj2.gameObject);
                                    break;
                            }

                            break;
                        case DunGeonEvent.Hunt:
                            var createHt = eventObj as HuntingData;
                            var obj3 = createHt.CreateObj(huntingObjPrefab);
                            eventObjInstanceList.Add(obj3.gameObject);
                            break;
                        case DunGeonEvent.RandomIncount:
                            var createRi = eventObj as RandomIncountData;
                            var obj4 = createRi.CreateObj(randomEventObjPrefab);
                            eventObjInstanceList.Add(obj4.gameObject);
                            break;
                        case DunGeonEvent.SubStory:
                            break;
                    }
                }
            }
        }
    }
}
