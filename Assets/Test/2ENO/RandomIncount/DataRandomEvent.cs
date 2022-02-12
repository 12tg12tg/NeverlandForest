using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

public class RandomEventSaveData_0 : SaveDataBase
{
    public List<string> useEventIDs = new List<string>();
    public List<DataRandomEvent> randomEventAllData = new List<DataRandomEvent>();
    public bool isTutorialRandomEvent;
    public bool isFirst;
}

public class DataRandomEvent
{
    public string eventID;
    public string eventName;
    public string eventDesc;
    public List<string> selectName = new List<string>();
    public List<string> sucessDesc = new List<string>();
    public List<string> failDesc = new List<string>();
    public string sucess1Info;
    public string fail1Info;
    public string sucess2Info;
    public string fail2Info;
    public string sucess3Info;
    public string fail3Info;

    private List<string> sucessInfo = new List<string>();
    public List<string> SucessInfo
    {
        get => sucessInfo;
        set => sucessInfo = value;
    }
    private List<string> failInfo = new List<string>();
    public List<string> FailInfo
    {
        get => failInfo;
        set => failInfo = value;
    }
    public int selectBtnCount;

    // �̺�Ʈ�� ������ Ŭ����, �ش� �������� ���� �ǵ�� ������ ���ĺ��ʹ� �����ȴ�.
    public List<string> selectInfos = new List<string>();

    private bool isSucessFeedBack;
    private List<bool> isInsufficiency = new List<bool>() { false, false, false };

    // ���� �����. UI���� ���
    public string resultInfo;
    public string selectResultDesc;
    public int curSelectNum;
    [System.NonSerialized]
    public List<DataAllItem> rewardItems = new();

    private RandomEventTableElem eventData;
    public RandomEventTableElem EventData => eventData;

    public bool isTutorialEvent = false;

    public DataRandomEvent() { }

    public DataRandomEvent(DataRandomEvent data)
    {
        selectName.Clear();
        sucessDesc.Clear();
        failDesc.Clear();

        eventID = data.eventID;
        eventName = data.eventName;
        eventDesc = data.eventDesc;
        selectName = data.selectName;
        sucessDesc = data.sucessDesc;
        failDesc = data.failDesc;
        sucess1Info = data.sucess1Info;
        fail1Info = data.fail1Info;
        sucess2Info = data.sucess2Info;
        fail2Info = data.fail2Info;
        sucess3Info = data.sucess3Info;
        fail3Info = data.fail3Info;

        selectBtnCount = data.selectBtnCount;

        SucessInfo = data.sucessInfo;
        FailInfo = data.failInfo;
        selectInfos = data.selectInfos;

        var randomTable = DataTableManager.GetTable<RandomEventTable>();
        eventData = randomTable.GetData<RandomEventTableElem>(eventID);
    }

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
        var sucess1InfoString = stringTable.GetData<LocalizationTableElem>(data.sucess1Info);
        var sucess2InfoString = stringTable.GetData<LocalizationTableElem>(data.sucess2Info);
        var sucess3InfoString = stringTable.GetData<LocalizationTableElem>(data.sucess3Info);
        var fail1InfoString = stringTable.GetData<LocalizationTableElem>(data.fail1Info);
        var fail2InfoString = stringTable.GetData<LocalizationTableElem>(data.fail2Info);
        var fail3InfoString = stringTable.GetData<LocalizationTableElem>(data.fail3Info);

        eventDesc = eventDescString.kor;
        //eventDesc = data.eventDesc;
        selectName.Add(select1NameString.kor);
        selectName.Add(select2NameString.kor);
        selectName.Add(select3NameString.kor);
        sucessDesc.Add(sucess1DescString.kor);
        sucessDesc.Add(sucess2DescString.kor);
        sucessDesc.Add(sucess3DescString.kor);
        failDesc.Add(fail1DescString.kor);
        failDesc.Add(fail2DescString.kor);
        failDesc.Add(fail3DescString.kor);

        sucess1Info = sucess1InfoString.kor;
        sucess2Info = sucess2InfoString.kor;
        sucess3Info = sucess3InfoString.kor;
        fail1Info = fail1InfoString.kor;
        fail2Info = fail2InfoString.kor;
        fail3Info = fail3InfoString.kor;
        if (string.IsNullOrEmpty(sucess1Info))
            sucess1Info = "�Ҹ� ����";
        if (string.IsNullOrEmpty(sucess2Info))
            sucess2Info = "�Ҹ� ����";
        if (string.IsNullOrEmpty(sucess3Info))
            sucess3Info = "�Ҹ� ����";
        if (string.IsNullOrEmpty(fail1Info))
            fail1Info = "�Ҹ� ����";
        if (string.IsNullOrEmpty(fail2Info))
            fail2Info = "�Ҹ� ����";
        if (string.IsNullOrEmpty(fail3Info))
            fail3Info = "�Ҹ� ����";

        eventID = data.id;
        eventData = data;
        eventName = data.name;

        sucessInfo.Add("�߰ߵ��� ���� �̺�Ʈ�Դϴ�");
        sucessInfo.Add("�߰ߵ��� ���� �̺�Ʈ�Դϴ�");
        sucessInfo.Add("�߰ߵ��� ���� �̺�Ʈ�Դϴ�");
        failInfo.Add("�߰ߵ��� ���� �̺�Ʈ�Դϴ�");
        failInfo.Add("�߰ߵ��� ���� �̺�Ʈ�Դϴ�");
        failInfo.Add("�߰ߵ��� ���� �̺�Ʈ�Դϴ�");

        if (data.sucess1Chance == 0)
            selectBtnCount = 0;
        else if (data.sucess2Chance == 0)
            selectBtnCount = 1;
        else if (data.sucess3Chance == 0)
            selectBtnCount = 2;
        else
            selectBtnCount = 3;

        if (data.sucess1Chance == 100)
            FailInfo[0] = string.Empty;
        else if (data.sucess2Chance == 100)
            FailInfo[1] = string.Empty;
        else if (data.sucess3Chance == 100)
            FailInfo[2] = string.Empty;

        for (int i = 0; i < 3; i++)
        {
            var sb = new StringBuilder();
            sb.Append(SucessInfo[i]);
            if(!string.IsNullOrEmpty(FailInfo[i]))
                sb.Append($" || {FailInfo[i]}");

            selectInfos.Add(sb.ToString());
        }

        //List<EventFeedBackType> tempList = new List<EventFeedBackType>();
        //tempList.AddRange(data.sucess1Type);
        //tempList.AddRange(data.sucess2Type);
        //tempList.AddRange(data.sucess3Type);
        //tempList.AddRange(data.fail1Type);
        //tempList.AddRange(data.fail2Type);
        //tempList.AddRange(data.fail3Type);
    }

    private void DataInit()
    {
        resultInfo = string.Empty;
        selectResultDesc = string.Empty;
        curSelectNum = -1;
        rewardItems.Clear();
    }

    // �ǵ�� �Լ���, �ǵ�� �Լ��� ���� ���� ��ȭ�� ���� ���̽��� return
    public void SelectFeedBack(int selectNum)
    {
        var randomEventUiManager = RandomEventUIManager.Instance;

        if (selectNum < 1 || selectNum > 3)
        {
            Debug.Log("�߸��� ���ù�ȣ ����");
            return;
        }
        if(randomEventUiManager.curOperatorFeedback == selectNum)
        {
            return;
        }

        DataInit();
        curSelectNum = selectNum;
        int eventSucessChance = 0;
        List<EventFeedBackType> eventTypes = null;
        List<int> eventfeedbackIDs = null;
        List<int> eventVals = null;
        //List<string> tempStrList = null;
        switch(selectNum)
        {
            case 1:
                eventSucessChance = eventData.sucess1Chance;
                //tempStrList = feedBackStringSelect1;
                break;
            case 2:
                eventSucessChance = eventData.sucess2Chance;
                //tempStrList = feedBackStringSelect2;
                break;
            case 3:
                eventSucessChance = eventData.sucess3Chance;
                //tempStrList = feedBackStringSelect3;
                break;
        }

        if (isTutorialEvent)
            eventSucessChance = 100;

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
                        SucessInfo[0] = sucess1Info;
                        break;
                    case 2:
                        eventfeedbackIDs = eventData.sucess2FeedBackID;
                        eventVals = eventData.sucess2Val;
                        eventTypes = eventData.sucess2Type;
                        SucessInfo[1] = sucess2Info;
                        break;
                    case 3:
                        eventfeedbackIDs = eventData.sucess3FeedBackID;
                        eventVals = eventData.sucess3Val;
                        eventTypes = eventData.sucess3Type;
                        SucessInfo[2] = sucess3Info;
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
                        FailInfo[0] = fail1Info;
                        break;
                    case 2:
                        eventTypes = eventData.fail2Type;
                        eventfeedbackIDs = eventData.fail2FeedBackID;
                        eventVals = eventData.fail2Val;
                        FailInfo[1] = fail2Info;
                        break;
                    case 3:
                        eventTypes = eventData.fail3Type;
                        eventfeedbackIDs = eventData.fail3FeedBackID;
                        eventVals = eventData.fail3Val;
                        FailInfo[2] = fail3Info;
                        break;
                }
                isSucessFeedBack = false;
            }
        }
        if (isSucessFeedBack)
            selectResultDesc = sucessDesc[selectNum - 1];
        else
            selectResultDesc = failDesc[selectNum - 1];

        StringBuilder sb = new StringBuilder();
        string tempStr;

        // TODO : tempStr�� ���� ���̺�� �ؾߴ�
        for (int i = 0; i < eventTypes.Count; i++)
        {
            switch (eventTypes[i])
            {
                case EventFeedBackType.GetNote:
                    var newMemoTable = DataTableManager.GetTable<MemoTable>();
                    var stringId2 = $"ME_0{Random.Range(1, 6)}";
                    var memo = newMemoTable.GetData<MemoTableElem>(stringId2);

                    var memoList = Vars.UserData.HaveMemoIDList;
                    if (!memoList.Contains(stringId2))
                    {
                        memoList.Add(stringId2);
                        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Memo);
                    }
                    selectResultDesc = string.Format(selectResultDesc, memo.desc);
                    break;
                case EventFeedBackType.Stamina:
                    // �Һ� ������ ����ó��
                    var stamina = eventVals[i];
                    if (stamina < 0)
                    {
                        if (Vars.UserData.uData.Tiredness + stamina <= 0)
                            isInsufficiency[selectNum-1] = true;
                        else
                            ConsumeManager.RecoveryTiredness(stamina);
                    }
                    tempStr = $"���׹̳���ġ : {stamina}\n";
                    sb.Append(tempStr);
                    tempStr = $"���׹̳���ġ : {stamina} ";
                    break;
                case EventFeedBackType.Hp:
                    var hp = eventVals[i];
                    if (hp > 0)
                        ConsumeManager.RecoverHp(hp);
                    else
                    {
                        if(Vars.UserData.uData.Hp + hp <= 0)
                            isInsufficiency[selectNum - 1] = true;
                        else
                            ConsumeManager.GetDamage(-hp);
                    }

                    tempStr = $"HP��ġ : {hp}\n";
                    sb.Append(tempStr);


                    break;
                case EventFeedBackType.Item:
                    // ������ ȹ�� �Ǵ� ���� - val���� ���� ownCount �����ؼ� ����
                    var newItemTable = DataTableManager.GetTable<AllItemDataTable>();
                    var stringId = $"ITEM_{eventfeedbackIDs[i]}";
                    var newItem = new DataAllItem(newItemTable.GetData<AllItemTableElem>(stringId));

                    newItem.OwnCount = eventVals[i];
                    if (newItem.OwnCount < 0)
                    {
                        var item = Vars.UserData.HaveAllItemList.ToList().Find(x => x.itemId == newItem.itemId);
                        if (item == null)
                            isInsufficiency[selectNum - 1] = true;
                        else if (item.OwnCount + newItem.OwnCount < 0)
                            isInsufficiency[selectNum - 1] = true;
                        else
                            Vars.UserData.RemoveItemData(newItem);

                        tempStr = $"������ {newItem.ItemTableElem.name} ����\n";
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
                    ConsumeManager.ConsumeLantern(-lanternGage);
                    tempStr = $"���ϼ�ġ : {lanternGage}\n";
                    sb.Append(tempStr);

                    break;
                case EventFeedBackType.TurnConsume:
                    var turnConsume = eventVals[i];
                    ConsumeManager.TimeUp(0, turnConsume);
                    tempStr = $"�� �Һ� {turnConsume}\n";
                    sb.Append(tempStr);

                    break;
                case EventFeedBackType.RandomMaterialLose:
                    var list = Vars.UserData.HaveAllItemList.ToList();
                    var materialList = list.Where(x => x.ItemTableElem.type == "MATERIAL").ToList();
                    var index = Random.Range(0, materialList.Count);

                    DataAllItem mtItem = null;
                    if (materialList.Count <= 0)
                        isInsufficiency[selectNum - 1] = true;
                    else
                    {
                        mtItem = new DataAllItem(materialList[index]);
                        mtItem.OwnCount = eventVals[i];
                        if (materialList[index].OwnCount - mtItem.OwnCount < 0)
                            isInsufficiency[selectNum - 1] = true;
                        else
                            Vars.UserData.RemoveItemData(mtItem);
                        tempStr = $"{materialList[index].ItemTableElem.name}�� : {eventVals[i]}��ŭ ����\n";
                        sb.Append(tempStr);
                    }
                    break;
                case EventFeedBackType.RandomMaterialGet:
                    var getMaterialList = new List<DataAllItem>();
                    var table = DataTableManager.GetTable<AllItemDataTable>();
                    for (int k = 0; k < table.data.Count; i++)
                    {
                        var strId = $"ITEM_{k + 1}";
                        var elem = table.data[strId] as AllItemTableElem;
                        if(elem.type == "MATERIAL")
                        {
                            var item = new DataAllItem(elem);
                            item.OwnCount = eventVals[i];
                            getMaterialList.Add(item);
                        }
                    }
                    var index2 = Random.Range(0, getMaterialList.Count);
                    rewardItems.Add(getMaterialList[index2]);
                    tempStr = $"{getMaterialList[index2].ItemTableElem.name}�� : {eventVals[i]}��ŭ ����\n";
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
                case EventFeedBackType.Battle:
                    randomEventUiManager.isBattleStart = true;
                    break;
            }
        }

        resultInfo = sb.ToString();
        if (!isInsufficiency[selectNum - 1])
            randomEventUiManager.NextPage();
        else
        {
            Debug.Log("���� �����");
            return;
        }

        var sb2 = new StringBuilder();
        sb2.Append(SucessInfo[selectNum - 1]);
        if (!string.IsNullOrEmpty(FailInfo[selectNum - 1]))
            sb2.Append($" || {FailInfo[selectNum - 1]}");

        selectInfos[selectNum - 1] = sb2.ToString();

        // �ι�Ŭ�� ����
        randomEventUiManager.curOperatorFeedback = selectNum;

        RandomEventManager.Instance.SaveEventData();
        GameManager.Instance.SaveLoad.Save(SaveLoadSystem.SaveType.ConsumableData);
    }

    public void DataDefaultEventExit()
    {
        for (int i = 0; i < isInsufficiency.Count; i++)
        {
            isInsufficiency[i] = false;
        }
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
