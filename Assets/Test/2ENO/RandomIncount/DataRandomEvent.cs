using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
public class DataRandomEvent
{
    private string eventID;
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

    private List<string> sucessInfo;
    public List<string> SucessInfo
    {
        get
        {
            if(sucessInfo == null)
            {
                sucessInfo = new List<string>();
                sucessInfo.Add("발견하지 않은 이벤트입니다");
                sucessInfo.Add("발견하지 않은 이벤트입니다");
                sucessInfo.Add("발견하지 않은 이벤트입니다");
            }
            return sucessInfo;
        }
    }
    private List<string> failInfo;
    public List<string> FailInfo
    {
        get
        {
            if (failInfo == null)
            {
                failInfo = new List<string>();
                failInfo.Add("발견하지 않은 이벤트입니다");
                failInfo.Add("발견하지 않은 이벤트입니다");
                failInfo.Add("발견하지 않은 이벤트입니다");
            }
            return failInfo;
        }
    }

    // 이벤트의 선택지 클릭시, 해당 선택지에 대한 피드백 정보가 이후부터는 공개된다.
    public List<string> selectInfos = new List<string>();
    //public List<string> SelectInfos
    //{
    //    get
    //    {
    //        if (selectInfos == null)
    //        {
    //            selectInfos = new List<string>();
    //            selectInfos.Add("선택되지 않은 선택지입니다");
    //            selectInfos.Add("선택되지 않은 선택지입니다");
    //            selectInfos.Add("선택되지 않은 선택지입니다");
    //        }
    //        return selectInfos;
    //    }
    //    set => selectInfos = value;
    //}

    private bool isSucessFeedBack;
    private List<bool> isInsufficiency = new List<bool>() { false, false, false };

    // 최종 결과들. UI에서 사용
    public string resultInfo;
    public string selectResultDesc;
    public int curSelectNum;
    public List<DataAllItem> rewardItems = new();
    public int selectBtnCount;

    private List<string> feedBackStringSelect1 = new();
    private List<string> feedBackStringSelect2 = new();
    private List<string> feedBackStringSelect3 = new();

    private RandomEventTableElem eventData;
    public RandomEventTableElem EventData => eventData;

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
            sucess1Info = "소모값 없음";
        if (string.IsNullOrEmpty(sucess2Info))
            sucess2Info = "소모값 없음";
        if (string.IsNullOrEmpty(sucess3Info))
            sucess3Info = "소모값 없음";
        if (string.IsNullOrEmpty(fail1Info))
            fail1Info = "소모값 없음";
        if (string.IsNullOrEmpty(fail2Info))
            fail2Info = "소모값 없음";
        if (string.IsNullOrEmpty(fail3Info))
            fail3Info = "소모값 없음";

        eventID = data.id;
        eventData = data;
        eventName = data.name;

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
                sb.Append($" / {FailInfo[i]}");

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

        // TODO : tempStr도 문자 테이블로 해야댐
        for (int i = 0; i < eventTypes.Count; i++)
        {
            switch (eventTypes[i])
            {
                case EventFeedBackType.Stamina:
                    // 소비값 부족시 예외처리
                    // 컨숨 매니저에 우탁이가 추가한 클래스 활용해서 값 수정
                    var stamina = eventVals[i];

                    if (stamina < 0)
                    {
                        if (Vars.UserData.uData.Tiredness + stamina <= 0)
                            isInsufficiency[selectNum-1] = true;
                        else
                            ConsumeManager.RecoveryTiredness(stamina);
                    }
                    
                    tempStr = $"스테미나수치 : {stamina}\n";
                    sb.Append(tempStr);

                    tempStr = $"스테미나수치 : {stamina} ";
                    break;
                case EventFeedBackType.Hp:
                    // 컨숨 매니저에 우탁이가 추가한 클래스 활용해서 값 수정
                    var hp = eventVals[i];
                    Vars.UserData.uData.HunterHp += hp;
                    tempStr = $"HP수치 : {hp}\n";
                    sb.Append(tempStr);


                    break;
                case EventFeedBackType.Item:
                    // 아이템 획득 또는 감소 - val값에 따라 ownCount 조정해서 넣음
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
                        Debug.Log("아이템감소");
                        tempStr = $"아이템 {newItem.ItemTableElem.name} 감소\n";
                    }
                    else
                    {
                        Debug.Log("아이템획득");
                        tempStr = $"아이템 {newItem.ItemTableElem.name} 획득\n";
                        rewardItems.Add(newItem);
                    }
                    sb.Append(tempStr);
                    break;
                case EventFeedBackType.LanternGage:
                    // 컨숨 매니저에 우탁이가 추가한 클래스 활용해서 값 수정
                    var lanternGage = eventVals[i];
                    ConsumeManager.ConsumeLantern(-lanternGage);
                    tempStr = $"랜턴수치 : {lanternGage}\n";
                    sb.Append(tempStr);

                    break;
                case EventFeedBackType.TurnConsume:
                    // 컨숨 매니저에 우탁이가 추가한 클래스 활용해서 값 수정
                    var turnConsume = eventVals[i];
                    tempStr = $"턴 소비 {turnConsume}\n";
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
                        tempStr = $"{materialList[index].ItemTableElem.name}를 : {eventVals[i]}만큼 잃음\n";
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
                    tempStr = $"{getMaterialList[index2].ItemTableElem.name}를 : {eventVals[i]}만큼 얻음\n";
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

        resultInfo = sb.ToString();
        if (!isInsufficiency[selectNum - 1])
            RandomEventUIManager.Instance.NextPage();
        else
        {
            Debug.Log("조건 불충분");
            return;
        }

        var sb2 = new StringBuilder();
        sb2.Append(SucessInfo[selectNum - 1]);
        if (!string.IsNullOrEmpty(FailInfo[selectNum - 1]))
            sb2.Append($" / {FailInfo[selectNum - 1]}");

        selectInfos[selectNum - 1] = sb2.ToString();
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
