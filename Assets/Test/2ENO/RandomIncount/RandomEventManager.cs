using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomEventManager : MonoBehaviour
{
    public List<DataRandomEvent> allDataList = new List<DataRandomEvent>();
    private List<DataRandomEvent> randomEventPool = new List<DataRandomEvent>();
    void Start()
    {
        var randomTable = DataTableManager.GetTable<RandomEventTable>();

        foreach (var data in randomTable.data)
        {
            var elem = (data.Value) as RandomEventTableElem;
            var rndEvent = new DataRandomEvent(elem, this);
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
    }

    public void AddEventInPool(DataRandomEvent evtData)
    {

        randomEventPool.Add(evtData);
    }

}

