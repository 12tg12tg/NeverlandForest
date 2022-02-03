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

    void init()
    {
        var randomTable = DataTableManager.GetTable<RandomEventTable>();

        foreach (var data in randomTable.data)
        {
            var elem = (data.Value) as RandomEventTableElem;
            var rndEvent = new DataRandomEvent(elem);
            allDataList.Add(rndEvent);
        }

        var list = from data in allDataList
                   where data.EventData.eventCondition == RandomEventCondition.Always
                   select data;

        // 일단은 이벤트 풀과 매니저의 allData는 같은 데이터 참조하게
        randomEventPool.AddRange(list);
    }


    public void LoadEventData()
    {
        // 세이브 로드 매니저에서 관리하는게 나을수도
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

            //var eventIndex = randomEventPool.FindIndex(x => x.EventData.id == templist[index].EventData.id);
            //if (beforeEventData == null)
            //{
            //    roomData.randomEventID = randomEventPool[eventIndex].EventData.id;
            //    beforeEventData = randomEventPool[eventIndex];
            //    break;
            //}
            //else if (beforeEventData.EventData.id != randomEventPool[eventIndex].EventData.id)
            //{
            //    roomData.randomEventID = randomEventPool[eventIndex].EventData.id;
            //    beforeEventData = randomEventPool[eventIndex];
            //    break;
            //}

            // 특정 이벤트 확정반환 테스트코드 28 24 11
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

