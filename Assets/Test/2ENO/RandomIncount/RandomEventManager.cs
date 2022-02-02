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

        // �ϴ��� �̺�Ʈ Ǯ�� �Ŵ����� allData�� ���� ������ �����ϰ�
        randomEventPool.AddRange(list);
    }


    public void LoadEventData()
    {
        // ���̺� �ε� �Ŵ������� �����ϴ°� ��������
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
    // �ڷ�ƾ���� �ٽ� �Ϲ����� �ٲ㺽
    public void CreateRandomEvent(EventData roomData)
    {
        int count = 0;
        while (string.IsNullOrEmpty(roomData.randomEventID))
        {
            count++;
            if(count > 100)
            {
                Debug.Log("���ѿ���");
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

            // 1�� ��з� ��Ȯ���� ��
            var list1 = from data in randomEventPool
                        where data.EventData.eventFrequency == eventFre
                        select data;
            var templist = list1.ToList();
            // 2�� �Һз� ��Ȯ���� ��
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

            // Ư�� �̺�Ʈ Ȯ����ȯ �׽�Ʈ�ڵ�
            //roomData.randomEventID = "4";
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

