using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataRandomEvent
{
    private string eventID;
    // ���� ���ڿ��� ������ �����δ� stringTable�� ID
    public string eventName;
    public string eventDesc;
    public List<string> selectName = new List<string>();
    public List<string> sucessDesc = new List<string>();
    public List<string> failDesc = new List<string>();

    // �̺�Ʈ�� ������ Ŭ����, �ش� �������� ���� �ǵ�� ������ ���ĺ��ʹ� �����ȴ�.
    private List<bool> isSelectChecks;
    public List<bool> IsSelectChecks
    {
        get
        {
            if (isSelectChecks == null)
            {
                isSelectChecks = new List<bool>();
                isSelectChecks.Add(false);
                isSelectChecks.Add(false);
                isSelectChecks.Add(false);
            }
            return isSelectChecks;
        }
        set => isSelectChecks = value;
    }

    private List<string> selectInfos;
    public List<string> SelectInfos
    {
        get
        {
            if (selectInfos == null)
            {
                selectInfos = new List<string>();
                selectInfos.Add("���õ��� ���� �������Դϴ�");
                selectInfos.Add("���õ��� ���� �������Դϴ�");
                selectInfos.Add("���õ��� ���� �������Դϴ�");
            }
            return selectInfos;
        }
        set => selectInfos = value;
    }
    // ���� �����. UI���� ���
    public string resultInfo;
    public string selectResult;
    public int curSelectNum;
    public List<DataItem> rewardItems = new();

    private RandomEventTableElem eventData;
    public RandomEventTableElem EventData => eventData;

    private bool isSucessFeedBack;


    public DataRandomEvent(RandomEventTableElem data)
    {
        var stringTable = DataTableManager.GetTable<LocalizationTable>();
        var eventDescString = stringTable.GetData<LocalizationTableElem>(data.eventDesc);
        var select1NameString = stringTable.GetData<LocalizationTableElem>(data.select1Name);
        var select2NameString = stringTable.GetData<LocalizationTableElem>(data.select2Name);
        var select3NameString = stringTable.GetData<LocalizationTableElem>(data.select3Name);
        var sucess1DescString = stringTable.GetData<LocalizationTableElem>(data.sucess1Desc);
        var sucess2DescString = stringTable.GetData<LocalizationTableElem>(data.sucess2Desc);
        var sucess3DescString = stringTable.GetData<LocalizationTableElem>(data.sucess3Desc);
        var fail1DescString = stringTable.GetData<LocalizationTableElem>(data.fail1Desc);
        var fail2DescString = stringTable.GetData<LocalizationTableElem>(data.fail2Desc);
        var fail3DescString = stringTable.GetData<LocalizationTableElem>(data.fail3Desc);
        eventDesc = eventDescString.kor;
        selectName.Add(select1NameString.kor);
        selectName.Add(select2NameString.kor);
        selectName.Add(select3NameString.kor);
        sucessDesc.Add(sucess1DescString.kor);
        sucessDesc.Add(sucess2DescString.kor);
        sucessDesc.Add(sucess3DescString.kor);
        failDesc.Add(fail1DescString.kor);
        failDesc.Add(fail2DescString.kor);
        failDesc.Add(fail3DescString.kor);

        eventID = data.id;
        eventData = data;
        eventName = data.name;
    }

    private void DataInit()
    {
        resultInfo = string.Empty;
        selectResult = string.Empty;
        curSelectNum = -1;
        rewardItems.Clear();
    }

    // �ǵ�� �Լ���, �ǵ�� �Լ��� ���� ���� ��ȭ�� ���� ���̽��� return
    public void SelectFeedBack(int selectNum)
    {
        if (selectNum < 1 || selectNum > 3)
        {
            Debug.Log("�߸��� ���ù�ȣ ����");
            return;
        }
        DataInit();
        curSelectNum = selectNum;
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

        if (eventTypes[0] == EventFeedBackType.None || eventTypes[0] == EventFeedBackType.NoLose
    || eventTypes[0] == EventFeedBackType.GetNote || eventTypes[0] == EventFeedBackType.Battle)
            return;

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
                    newItem.dataType = DataType.AllItem;
                    newItem.OwnCount = eventVals[i];

                    if (newItem.OwnCount < 0)
                    {
                        tempStr = $"������ {newItem.ItemTableElem.name} ����\n";
                        Vars.UserData.RemoveItemData(newItem);
                    }
                    else
                    {
                        tempStr = $"������ {newItem.ItemTableElem.name} ȹ��\n";
                        rewardItems.Add(newItem);
                    }
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.LanternGage:
                    var lanternGage = eventVals[i];

                    if (isSucessFeedBack)
                        tempStr = $"���ϼ�ġ ����\n";
                    else
                        tempStr = $"���ϼ�ġ ����\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.TurnConsume:
                    var turnConsume = eventVals[i];

                    if (isSucessFeedBack)
                        tempStr = $"���� - �� �Һ�\n";
                    else
                        tempStr = $"���� - �� �Һ�\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.MostItemLose:
                    // ���� ������ �ִ� �κ��丮 ������ OwnCount �˻�

                    if (isSucessFeedBack)
                        tempStr = $"���� - ���帹�� ������ ����\n";
                    else
                        tempStr = $"���� - ���帹�� ������ ����\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.RandomMaterial:
                    // ���� �������ִ� �κ��丮 ������ Type Linq�� �˻� -> �������� ����Ʈ�� �̾Ƽ� ���� �ϳ��������� ���� ����!

                    if (isSucessFeedBack)
                        tempStr = $"���� - ������� ȹ��\n";
                    else
                        tempStr = $"���� - ������� ����\n";
                    sb.Append(tempStr);

                    break;
                case EventFeedBackType.AnotherEvent:
                    // �ٸ� �̺�Ʈ �ر� - �����Ŵ��� �Լ� ȣ��
                    var newRndEvent = RandomEventManager.Instance.allDataList.Find(x => x.EventData.id == $"{eventfeedbackIDs[i]}");
                    if (eventVals[i] == 1)
                        RandomEventManager.Instance.AddEventInPool(newRndEvent);
                    else
                        RandomEventManager.Instance.RemoveEventInPool(newRndEvent);

                    tempStr = $"�̺�Ʈ �ر�\n";
                    sb.Append(tempStr);
                    break;
            }
        }

        if (isSucessFeedBack)
            selectResult = sucessDesc[selectNum - 1];
        else
            selectResult = failDesc[selectNum - 1];

        resultInfo = sb.ToString();
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
