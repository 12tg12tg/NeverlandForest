using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataRandomEvent
{
    private string eventID;
    // ���� ���ڿ��� ������ �����δ� stringTable�� ID
    private string eventName;
    private string eventDesc;
    private string select1Name;
    private string select2Name;
    private string select3Name;
    private string sucess1Desc;
    private string sucess2Desc;
    private string sucess3Desc;
    private string fail1Desc;
    private string fail2Desc;
    private string fail3Desc;

    private RandomEventTableElem eventData;
    public RandomEventTableElem EventData => eventData;
    private RandomEventManager eventManager;

    private bool isSucessFeedBack;

    // �̺�Ʈ�� ������ Ŭ����, �ش� �������� ���� �ǵ�� ������ ���ĺ��ʹ� �����ȴ�.
    private bool[] isSelectCheck = new bool[3];

    private string[] selectInfo = new string[3];

    private string resultInfo;

    public DataRandomEvent(RandomEventTableElem data, RandomEventManager manager)
    {
        eventManager = manager;
        eventID = data.id;
        eventData = data;
        eventName = data.name;
        eventDesc = data.eventDesc;
        select1Name = data.select1Name;
        select2Name = data.select2Name;
        select3Name = data.select3Name;
        sucess1Desc = data.sucess1Desc;
        sucess2Desc = data.sucess2Desc;
        sucess3Desc = data.sucess3Desc;
        fail1Desc = data.fail1Desc;
        fail2Desc = data.fail2Desc;
        fail3Desc = data.fail3Desc;
    }
    // �ǵ�� �Լ���, �ǵ�� �Լ��� ���� ���� ��ȭ�� ���� ���̽��� return

    public void SelectFeedBack(int selectNum)
    {
        if (selectNum < 1 || selectNum > 3)
        {
            Debug.Log("�߸��� ���ù�ȣ ����");
            return;
        }

        int eventSucessChance = 0;
        List<EventFeedBackType> eventTypes = null;
        List<int> eventfeedbackIDs = null;
        List<int> eventVals = null;
        switch(selectNum)
        {
            case 1:
                eventSucessChance = eventData.sucess1Chance;
                break;
            case 2:
                eventSucessChance = eventData.sucess2Chance;
                break;
            case 3:
                eventSucessChance = eventData.sucess3Chance;
                break;
        }

        if (eventTypes[0] == EventFeedBackType.None || eventTypes[0] == EventFeedBackType.NoLose
            || eventTypes[0] == EventFeedBackType.GetNote || eventTypes[0] == EventFeedBackType.Battle)
            return;

        if (eventSucessChance == 0)
            return;
        // ���� �Ǵ� ���� ���
        else
        {
            var rndNum = Random.Range(1, 100);
            if(rndNum <= eventSucessChance)
            {
                switch (selectNum)
                {
                    case 1:
                        eventTypes = eventData.sucess1Type;
                        eventfeedbackIDs = eventData.sucess1FeedBackID;
                        eventVals = eventData.sucess1Val;
                        break;
                    case 2:
                        eventfeedbackIDs = eventData.sucess2FeedBackID;
                        eventVals = eventData.sucess2Val;
                        eventTypes = eventData.sucess2Type;
                        break;
                    case 3:
                        eventfeedbackIDs = eventData.sucess3FeedBackID;
                        eventVals = eventData.sucess3Val;
                        eventTypes = eventData.sucess3Type;
                        break;
                }
                isSucessFeedBack = true;
            }
            else
            {
                switch (selectNum)
                {
                    case 1:
                        eventTypes = eventData.fail1Type;
                        eventfeedbackIDs = eventData.fail1FeedBackID;
                        eventVals = eventData.fail1Val;
                        break;
                    case 2:
                        eventTypes = eventData.fail2Type;
                        eventfeedbackIDs = eventData.fail2FeedBackID;
                        eventVals = eventData.fail2Val;
                        break;
                    case 3:
                        eventTypes = eventData.fail3Type;
                        eventfeedbackIDs = eventData.fail3FeedBackID;
                        eventVals = eventData.fail3Val;
                        break;
                }
                isSucessFeedBack = false;
            }
        }
        StringBuilder sb = new StringBuilder();
        string tempStr;

        // �̰� ���������� �ٲ�� ����鵵 ���� ���̺�� ����ؾ� ���ڳ�? ������;
        for (int i = 0; i < eventTypes.Count; i++)
        {
            switch (eventTypes[i])
            {
                case EventFeedBackType.Stamina:
                    // ���׹̳� ���� �Լ��� �� �Ѱ���
                    var stamina = eventVals[i];

                    if(isSucessFeedBack)
                        tempStr = $"���׹̳���ġ {stamina} ����\n";
                    else
                        tempStr = $"���׹̳���ġ {stamina} ����\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.Hp:
                    var hp = eventVals[i];

                    if (isSucessFeedBack)
                        tempStr = $"HP��ġ {hp} ����\n";
                    else
                        tempStr = $"HP��ġ {hp} ����\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.Item:
                    // ������ ȹ�� �Ǵ� ���� - val���� ���� ownCount �����ؼ� ����
                    var newItemTable = DataTableManager.GetTable<AllItemDataTable>();
                    var newItem = new DataAllItem();

                    newItem.itemId = eventfeedbackIDs[i];
                    var stringId = $"{eventfeedbackIDs[i]}";
                    newItem.itemTableElem = newItemTable.GetData<AllItemTableElem>(stringId);
                    newItem.OwnCount = eventVals[i];

                    break;
                case EventFeedBackType.LanternGage:
                    var lanternGage = eventVals[i];


                    break;
                case EventFeedBackType.TurnConsume:
                    var turnConsume = eventVals[i];
                    break;
                case EventFeedBackType.MostItemLose:
                    // ���� ������ �ִ� �κ��丮 ������ OwnCount �˻�
                    break;
                case EventFeedBackType.RandomMaterial:
                    // ���� �������ִ� �κ��丮 ������ Type Linq�� �˻� -> �������� ����Ʈ�� �̾Ƽ� ���� �ϳ��������� ���� ����!
                    break;
                case EventFeedBackType.AnotherEvent:
                    // �ٸ� �̺�Ʈ �ر� - �����Ŵ��� �Լ� ȣ��
                    var newRndEvent = eventManager.allDataList.Find(x => x.EventData.id == $"{eventfeedbackIDs[i]}");
                    if (eventVals[i] == 1)
                        eventManager.AddEventInPool(newRndEvent);
                    else
                        eventManager.RemoveEventInPool(newRndEvent);
                    break;
            }
        }
    }

    private void ResultInfoString(EventFeedBackType type)
    {
    }

    //public void Select1FeedBack()
    //{
    //    if (eventData.sucess1Chance == 0)
    //        return;
    //    else if (eventData.sucess1Type[0] == EventFeedBackType.None || eventData.sucess1Type[0] == EventFeedBackType.NoLose
    //        || eventData.sucess1Type[0] == EventFeedBackType.GetNote || eventData.sucess1Type[0] == EventFeedBackType.Battle)
    //        return;

    //    for (int i = 0; i < eventData.sucess1Type.Count; i++)
    //    {
    //        switch (eventData.sucess1Type[i])
    //        {
    //            case EventFeedBackType.Stamina:
    //                // ���׹̳� ���� �Լ��� �� �Ѱ���
    //                var stamina = eventData.sucess1Val[i];
    //                break;
    //            case EventFeedBackType.Hp:
    //                var hp = eventData.sucess1Val[i];
    //                break;
    //            case EventFeedBackType.Item:
    //                // ������ ȹ�� �Ǵ� ���� - val���� ���� ownCount �����ؼ� ����
    //                var newItemTable = DataTableManager.GetTable<AllItemDataTable>();
    //                var itemId = $"{eventData.sucess1FeedBackID[i]}";
    //                var newItem = new DataAllItem();
    //                newItem.itemTableElem = newItemTable.GetData<AllItemTableElem>(itemId);
    //                newItem.OwnCount = eventData.sucess1Val[i];

    //                break;
    //            case EventFeedBackType.LanternGage:
    //                var lanternGage = eventData.sucess1Val[i];
    //                break;
    //            case EventFeedBackType.TurnConsume:
    //                var turnConsume = eventData.sucess1Val[i];
    //                break;
    //            case EventFeedBackType.MostItemLose:
    //                // ���� ������ �ִ� �κ��丮 ������ OwnCount �˻�
    //                break;
    //            case EventFeedBackType.RandomMaterial:
    //                // ���� �������ִ� �κ��丮 ������ Type Linq�� �˻� -> �������� ����Ʈ�� �̾Ƽ� ���� �ϳ��������� ���� ����!
    //                break;
    //            case EventFeedBackType.AnotherEvent:
    //                // �ٸ� �̺�Ʈ �ر� - �����Ŵ��� �Լ� ȣ��
    //                var newRndEvent = eventManager.allDataList.Find(x => x.EventData.id == $"{eventData.sucess1FeedBackID[i]}");
    //                if (eventData.sucess1Val[i] == 1)
    //                    eventManager.AddEventInPool(newRndEvent);
    //                else
    //                    eventManager.RemoveEventInPool(newRndEvent);
    //                break;
    //        }
    //    }
    //}
}
