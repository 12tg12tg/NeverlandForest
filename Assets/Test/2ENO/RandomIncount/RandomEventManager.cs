using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: �ӽ�
public enum CurrentGameScene
{
    Dungeon,
    Hunting,
    Camp,
    Battle,
}

public class RandomEventManager : MonoBehaviour
{
    private static RandomEventManager instance;
    public static RandomEventManager Instance => instance;
    // TODO: �ӽ�
    public CurrentGameScene curGameState;

    public List<DataRandomEvent> allDataList = new List<DataRandomEvent>();
    private List<DataRandomEvent> randomEventPool = new List<DataRandomEvent>();
    public List<string> curDungeonRandomEventIDList = new List<string>();

    private DataRandomEvent beforeEventData;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
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

        // TODO : �׽�Ʈ�� �ӽ��ڵ�
        //var testEvent = randomTable.GetData<RandomEventTableElem>("11");
        //for (int i = 0; i < 20; i++)
        //{
        //    var data = new DataRandomEvent(testEvent);
        //    randomEventPool.Add(data);
        //}

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
    }

    public void AddEventInPool(DataRandomEvent evtData)
    {

        randomEventPool.Add(evtData);
    }

    public DataRandomEvent GetEventData(string eventID)
    {
        return randomEventPool.Find(x => x.EventData.id == eventID);
    }

    public IEnumerator CreateRandomEvent(EventData roomData)
    {
        while (string.IsNullOrEmpty(roomData.randomEventID))
        {
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

            // 2�� �Һз� ��Ȯ���� ��

            var pList = PercentPick(list1.ToList());
            var sList = PerCentSum(pList);

            var rndVal2 = Random.Range(0, sList.Last());
            var index = 0;
            foreach (var data in sList)
            {
                if (rndVal2 <= data)
                    break;
                index++;
            }

            if (beforeEventData == null)
            {
                roomData.randomEventID = randomEventPool[index].EventData.id;
                beforeEventData = randomEventPool[index];
                break;
            }
            else if (beforeEventData.EventData.id != randomEventPool[index].EventData.id)
            {
                roomData.randomEventID = randomEventPool[index].EventData.id;
                beforeEventData = randomEventPool[index];
                break;
            }

            // ������ �׽�Ʈ�ڵ�
            //roomData.randomEventID = randomEventPool[0].EventData.id;
            yield return null;
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

