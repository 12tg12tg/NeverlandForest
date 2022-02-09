using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomEventManager : MonoBehaviour
{
    private static RandomEventManager instance;
    public static RandomEventManager Instance => instance;

    private static bool isStart;

    public List<DataRandomEvent> allDataList = new List<DataRandomEvent>();
    private List<DataRandomEvent> randomEventPool = new List<DataRandomEvent>();
    public List<string> curDungeonRandomEventIDList = new List<string>();

    private DataRandomEvent beforeEventData;

    public bool isFirstRandomEvent = true;
    public bool isTutorialRandomEvent = true;
    public DataRandomEvent tutorialEvent = new();
    private void Awake()
    {
        if (!isStart)
        {
            instance = this;
            DontDestroyOnLoad(this);
            init();
        }
        isStart = true;
    }
    public void init()
    {
        var tempRandomTable = DataTableManager.GetTable<RandomEventTable>();
        var tutoElem = tempRandomTable.GetData<RandomEventTableElem>("4");
        tutorialEvent = new DataRandomEvent(tutoElem);
        tutorialEvent.isTutorialEvent = true;

        if (Vars.UserData.isRandomDataLoad)
        {
            foreach (var data in Vars.UserData.randomEventDatas)
            {
                var newRndData = new DataRandomEvent(data);
                allDataList.Add(newRndData);
            }

            var list = from data in allDataList
                       join dt in Vars.UserData.useEventID on data.EventData.id equals dt
                       select data;

            randomEventPool.AddRange(list);

            if (randomEventPool.Count <= 0)
            {
                Vars.UserData.isRandomDataLoad = false;
                init();
            }
        }
        else
        {
            // all 데이터 로드, 기본 분기
            var randomTable = DataTableManager.GetTable<RandomEventTable>();

            foreach (var data in randomTable.data)
            {
                var elem = (data.Value) as RandomEventTableElem;
                var rndEvent = new DataRandomEvent(elem);
                allDataList.Add(rndEvent);
            }

            // Pool에 넣는건 실시간으로 진행상황에 따라 넣고, 새로 던전 시작할때마다 풀에있는걸 가져가서 세팅
            var list = from data in allDataList
                       where data.EventData.eventCondition == RandomEventCondition.Always
                       select data;

            // 일단은 이벤트 풀과 매니저의 allData는 같은 데이터 참조하게
            randomEventPool.AddRange(list);
        }
    }

    public void SaveEventData()
    {
        Vars.UserData.randomEventDatas.Clear();
        Vars.UserData.useEventID.Clear();

        Vars.UserData.randomEventDatas.AddRange(allDataList);
        foreach(var id in randomEventPool)
        {
            Vars.UserData.useEventID.Add(id.EventData.id);
        }

        GameManager.Manager.SaveLoad.Save(SaveLoadSystem.SaveType.RandomEvent);
    }

    public void RemoveEventInPool(DataRandomEvent evtData)
    {
        var idx = randomEventPool.FindIndex(x => x.EventData.id == evtData.EventData.id);
        randomEventPool.RemoveAt(idx);
        int a = 100;
    }

    public void AddEventInPool(DataRandomEvent evtData)
    {
        randomEventPool.Add(evtData);
        int a = 100;
    }

    public DataRandomEvent GetEventData(string eventID)
    {
        var eventt = randomEventPool.Find(x => x.EventData.id == eventID);

        return eventt;
    }
    // 코루틴에서 다시 일반으로 바꿔봄
    public void CreateRandomEvent(EventData roomData)
    {
        int count = 0;
        while (string.IsNullOrEmpty(roomData.randomEventID))
        {
            count++;
            if(count > 100)
            {
                Debug.Log("무한오류");
                return;
            }

            var rndVal = Random.Range(0, 101);
            RandomEventFrequency eventFre = RandomEventFrequency.None;
            if (rndVal < 40)
                eventFre = RandomEventFrequency.Usually;
            else if (rndVal < 70)
                eventFre = RandomEventFrequency.Often;
            else if (rndVal < 90)
                eventFre = RandomEventFrequency.SomeTime;
            else
                eventFre = RandomEventFrequency.Rarely;

            // 1차 대분류 빈도확률로 픽
            var list1 = from data in randomEventPool
                        where data.EventData.eventFrequency == eventFre
                        select data;
            var templist = list1.ToList();
            // 2차 소분류 빈도확률로 픽
            var pList = PercentPick(templist);
            var sList = PerCentSum(pList);

            var rndVal2 = Random.Range(0, sList.Last());
            var index = 0;
            foreach (var data in sList)
            {
                if (rndVal2 <= data)
                    break;
                index++;
            }

            if(isFirstRandomEvent && !Vars.UserData.isRandomDataLoad)
            {
                roomData.randomEventID = "4";
                isFirstRandomEvent = false;
                break;
            }

            var eventIndex = randomEventPool.FindIndex(x => x.EventData.id == templist[index].EventData.id);
            if (beforeEventData == null)
            {
                roomData.randomEventID = randomEventPool[eventIndex].EventData.id;
                beforeEventData = randomEventPool[eventIndex];
                break;
            }
            else if (beforeEventData.EventData.id != randomEventPool[eventIndex].EventData.id)
            {
                roomData.randomEventID = randomEventPool[eventIndex].EventData.id;
                beforeEventData = randomEventPool[eventIndex];
                break;
            }

            //특정 이벤트 확정반환 테스트코드 28 24 11
            roomData.randomEventID = "4";
        }
    }

    private List<int> PercentPick(List<DataRandomEvent> list)
    {
        var percentList = new List<int>();

        foreach(var data in list)
        {
            percentList.Add(data.EventData.eventFrequency2);
        }
        return percentList;
    }

    private List<int> PerCentSum(List<int> list)
    {
        var sumList = new List<int>();
        var sum = 0;
        foreach(var data in list)
        {
            sum += data;
            sumList.Add(sum);
        }
        return sumList;
    }
}

