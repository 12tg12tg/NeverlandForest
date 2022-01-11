using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UserData
{
    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!      유저마다 다른 변수들       !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    //Item Info
    public List<DataConsumable> ConsumableItemList { get; set; } = new List<DataConsumable>();

    //World Info
    public List<MapNodeStruct_0> WorldMapNodeStruct { get; set; } = new List<MapNodeStruct_0>();
    public WorldMapPlayerData WorldMapPlayerData { get; set; }

    // 던전맵 데이터, 세이브 로드
    public SerializeDictionary<Vector2, DungeonData> CurAllDungeonData { get; set; } = new SerializeDictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;
    public int curDungeonRoomIdx;
    public int dungeonStartIdx;
    public bool dungeonReStart;
    public int currentDundeonRoomIndex;
    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();

    //??? - Vars로 이사가도 되는가?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    //ConsumeManager
    //피로도는 실질적으로 줄어드는 수치
    public CostData uData = new CostData();

    // 인벤토리에 사용
    public int maxInventoryItemCount = 21;
    private readonly List<DataItem> haveAllItemList = new List<DataItem>();
    public ReadOnlyCollection<DataItem> HaveAllItemList => haveAllItemList.AsReadOnly();

    public void AddItemData(DataItem newItem)
    {
        // 칸 하나를 전부 차지한 경우의 개수
        int myInventoryFullCount = 0;
        // 칸 하나를 일부 차지한 경우의 개수
        int myInventorySpaceCount = 0;
        
        foreach(var myItem in haveAllItemList)
        {
            if (myItem.SpareCount != 0)
                myInventorySpaceCount++;
            myInventoryFullCount += myItem.FullSpace;
        }

        // 현재 아이템들의 Full카운트합이 인벤토리 max카운트와 같을때는 절대 못넣음, 1개도 들어갈 여분이 없다는 뜻
        if (myInventoryFullCount == maxInventoryItemCount)
        {
            Debug.Log("아이템이 초과되어 전부 넣지 못합니다");
            return;
            // 아이템 못넣음
        }
        else
        {
            var myItem = haveAllItemList.Find(x => (x.itemId == newItem.itemId) && (x.dataType == newItem.dataType));
            // 칸 차지하는 경우의 수 모두 합쳐서 현재 인벤토리칸이 꽉 차있는 경우
            if (maxInventoryItemCount == (myInventoryFullCount + myInventorySpaceCount))
            {
                // 같은종류 아이템을 가지고 있을 떄
                if (myItem != null)
                {
                    // 남은 여분공간이 추가될 아이템 개수보다 같거나 클때
                    if (myItem.SpareCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        newItem.OwnCount -= myItem.SpareCount; // 서순중요
                        myItem.OwnCount += myItem.SpareCount;
                        // 이때, newItem은 아직 다 못넣은 상태
                        Debug.Log("아이템이 초과되어 전부 넣지 못합니다");
                        return;
                    }
                }
                else
                {
                    // 아이템 못넣음
                    Debug.Log("아이템이 초과되어 전부 넣지 못합니다");
                    return;
                }
            }
            // 아직 인벤토리칸에 여유가 있다
            else
            {
                if (myItem != null)
                {
                    if (myItem.SpareCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        while (newItem.OwnCount != 0 &&
                            maxInventoryItemCount > (myInventoryFullCount + myInventorySpaceCount))
                        {
                            myItem.OwnCount++;
                            newItem.OwnCount--;
                            if (myItem.SpareCount == 0)
                            {
                                myInventoryFullCount++;
                                if (newItem.OwnCount == 0)
                                    myInventorySpaceCount--;
                            }
                        }
                    }
                }
                // 새로운 아이템 추가
                else
                {
                    int overOwnCount = 0;
                    haveAllItemList.Add(newItem);
                    var newItemSpare = (newItem.SpareCount == 0) ? 0 : 1;
                    var tempFullCount = myInventoryFullCount + newItem.FullSpace;
                    var tempSpareCount = myInventorySpaceCount + newItemSpare;
                    if(maxInventoryItemCount - (tempFullCount + tempSpareCount) < 0)
                    {
                        var overInvenCount = (tempFullCount + tempSpareCount) - maxInventoryItemCount;
                        if(newItem.SpareCount == 0)
                        {
                            overOwnCount = newItem.LimitCount * overInvenCount;
                        }
                        else
                        {
                            overOwnCount = (newItem.LimitCount * overInvenCount - 1) + newItem.SpareCount;
                        }
                        newItem.OwnCount -= overOwnCount;
                        // overOwnCount 만큼의 새로운 item 생성해서 대기
                        Debug.Log("아이템이 초과되어 전부 넣지 못합니다");
                        return;
                    }
                }
            }
        }
        Debug.Log("아이템 추가완료");
    }
    public void RemoveItemData(DataItem removeItem)
    {
        var index = haveAllItemList.FindIndex(x => (x.itemId == removeItem.itemId) && (x.dataType == removeItem.dataType));
        if (index == -1)
        {
            Debug.Log("삭제할 아이템이 없음");
            return;
        }
        else
        {
            haveAllItemList[index].OwnCount -= removeItem.OwnCount;
            if(haveAllItemList[index].OwnCount <= 0)
            {
                haveAllItemList.RemoveAt(index);
                Debug.Log("아이템 삭제됨");
            }
            Debug.Log("아이템 감소됨");
        }
    }
    // DataItem 으로 그리고 List로 다시 변환해서 사용해보기
}
