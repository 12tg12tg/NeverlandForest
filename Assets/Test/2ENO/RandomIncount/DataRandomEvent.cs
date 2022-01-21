using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataRandomEvent
{
    private string eventID;
    // 각종 문자열들 이지만 실제로는 stringTable의 ID
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

    // 이벤트의 선택지 클릭시, 해당 선택지에 대한 피드백 정보가 이후부터는 공개된다.
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
    // 피드백 함수들, 피드백 함수로 인해 값의 변화가 없는 케이스는 return

    public void SelectFeedBack(int selectNum)
    {
        if (selectNum < 1 || selectNum > 3)
        {
            Debug.Log("잘못된 선택번호 들어옴");
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
                    newItem.OwnCount = eventVals[i];

                    break;
                case EventFeedBackType.LanternGage:
                    var lanternGage = eventVals[i];


                    break;
                case EventFeedBackType.TurnConsume:
                    var turnConsume = eventVals[i];
                    break;
                case EventFeedBackType.MostItemLose:
                    // 현재 가지고 있는 인벤토리 아이템 OwnCount 검색
                    break;
                case EventFeedBackType.RandomMaterial:
                    // 현재 가지고있는 인벤토리 아이템 Type Linq로 검색 -> 재료아이템 리스트만 뽑아서 그중 하나랜덤으로 고르고 적용!
                    break;
                case EventFeedBackType.AnotherEvent:
                    // 다른 이벤트 해금 - 랜덤매니저 함수 호출
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
