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

}

