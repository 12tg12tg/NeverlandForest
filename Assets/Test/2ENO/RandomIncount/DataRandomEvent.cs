using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataRandomEvent
{
    private string eventID;
    // 각종 문자열들 이지만 실제로는 stringTable의 ID
    public string eventName;
    public string eventDesc;
    public List<string> selectName = new List<string>();
    public List<string> sucessDesc = new List<string>();
    public List<string> failDesc = new List<string>();

    // 이벤트의 선택지 클릭시, 해당 선택지에 대한 피드백 정보가 이후부터는 공개된다.
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
                selectInfos.Add("선택되지 않은 선택지입니다");
                selectInfos.Add("선택되지 않은 선택지입니다");
                selectInfos.Add("선택되지 않은 선택지입니다");
            }
            return selectInfos;
        }
        set => selectInfos = value;
    }
    // 최종 결과들. UI에서 사용
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

    // 피드백 함수들, 피드백 함수로 인해 값의 변화가 없는 케이스는 return
    public void SelectFeedBack(int selectNum)
    {
        if (selectNum < 1 || selectNum > 3)
        {
            Debug.Log("잘못된 선택번호 들어옴");
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
        // 성공 또는 실패 경우
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

        // 이거 유동적으로 바뀌는 설명들도 문자 테이블로 사용해야 되자나? ㅎㄷㄷ;
        for (int i = 0; i < eventTypes.Count; i++)
        {
            switch (eventTypes[i])
            {
                case EventFeedBackType.Stamina:
                    // 스테미나 조정 함수에 값 넘겨줌
                    var stamina = eventVals[i];

                    if(isSucessFeedBack)
                        tempStr = $"스테미나수치 {stamina} 증가\n";
                    else
                        tempStr = $"스테미나수치 {stamina} 감소\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.Hp:
                    var hp = eventVals[i];

                    if (isSucessFeedBack)
                        tempStr = $"HP수치 {hp} 증가\n";
                    else
                        tempStr = $"HP수치 {hp} 감소\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.Item:
                    // 아이템 획득 또는 감소 - val값에 따라 ownCount 조정해서 넣음
                    var newItemTable = DataTableManager.GetTable<AllItemDataTable>();
                    var newItem = new DataAllItem();

                    newItem.itemId = eventfeedbackIDs[i];
                    var stringId = $"{eventfeedbackIDs[i]}";
                    newItem.itemTableElem = newItemTable.GetData<AllItemTableElem>(stringId);
                    newItem.dataType = DataType.AllItem;
                    newItem.OwnCount = eventVals[i];

                    if (newItem.OwnCount < 0)
                    {
                        tempStr = $"아이템 {newItem.ItemTableElem.name} 감소\n";
                        Vars.UserData.RemoveItemData(newItem);
                    }
                    else
                    {
                        tempStr = $"아이템 {newItem.ItemTableElem.name} 획득\n";
                        rewardItems.Add(newItem);
                    }
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.LanternGage:
                    var lanternGage = eventVals[i];

                    if (isSucessFeedBack)
                        tempStr = $"랜턴수치 증가\n";
                    else
                        tempStr = $"랜턴수치 감소\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.TurnConsume:
                    var turnConsume = eventVals[i];

                    if (isSucessFeedBack)
                        tempStr = $"성공 - 턴 소비\n";
                    else
                        tempStr = $"실패 - 턴 소비\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.MostItemLose:
                    // 현재 가지고 있는 인벤토리 아이템 OwnCount 검색

                    if (isSucessFeedBack)
                        tempStr = $"성공 - 가장많은 아이템 감소\n";
                    else
                        tempStr = $"실패 - 가장많은 아이템 감소\n";
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.RandomMaterial:
                    // 현재 가지고있는 인벤토리 아이템 Type Linq로 검색 -> 재료아이템 리스트만 뽑아서 그중 하나랜덤으로 고르고 적용!

                    if (isSucessFeedBack)
                        tempStr = $"성공 - 랜덤재료 획득\n";
                    else
                        tempStr = $"실패 - 랜덤재료 감소\n";
                    sb.Append(tempStr);

                    break;
                case EventFeedBackType.AnotherEvent:
                    // 다른 이벤트 해금 - 랜덤매니저 함수 호출
                    var newRndEvent = RandomEventManager.Instance.allDataList.Find(x => x.EventData.id == $"{eventfeedbackIDs[i]}");
                    if (eventVals[i] == 1)
                        RandomEventManager.Instance.AddEventInPool(newRndEvent);
                    else
                        RandomEventManager.Instance.RemoveEventInPool(newRndEvent);

                    tempStr = $"이벤트 해금\n";
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
    //                // 스테미나 조정 함수에 값 넘겨줌
    //                var stamina = eventData.sucess1Val[i];
    //                break;
    //            case EventFeedBackType.Hp:
    //                var hp = eventData.sucess1Val[i];
    //                break;
    //            case EventFeedBackType.Item:
    //                // 아이템 획득 또는 감소 - val값에 따라 ownCount 조정해서 넣음
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
    //                // 현재 가지고 있는 인벤토리 아이템 OwnCount 검색
    //                break;
    //            case EventFeedBackType.RandomMaterial:
    //                // 현재 가지고있는 인벤토리 아이템 Type Linq로 검색 -> 재료아이템 리스트만 뽑아서 그중 하나랜덤으로 고르고 적용!
    //                break;
    //            case EventFeedBackType.AnotherEvent:
    //                // 다른 이벤트 해금 - 랜덤매니저 함수 호출
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
