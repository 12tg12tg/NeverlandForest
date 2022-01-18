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
    public SerializeDictionary<Vector2, DungeonData> AllDungeonData { get; set; } = new SerializeDictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;
    public int dungeonStartIdx = 100;
    public bool dungeonReStart;

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
    public bool AddItemData(DataItem newItem)
    {
        // 칸 하나를 전부 차지한 경우의 개수
        int myInventoryFullCount = 0;
        // 칸 하나를 일부 차지한 경우의 개수
        int myInventorySpaceCount = 0;



        foreach (var myItem in haveAllItemList)
        {
            if (myItem.InvenRemainCount != 0)
                myInventorySpaceCount++;
            myInventoryFullCount += myItem.InvenFullCount;
        }

        // 현재 아이템들의 Full카운트합이 인벤토리 max카운트와 같을때는 절대 못넣음, 1개도 들어갈 여분이 없다는 뜻
        if (myInventoryFullCount == maxInventoryItemCount)
        {
            Debug.Log("아이템이 초과되어 전부 넣지 못합니다");
            return false;
            // 아이템 못넣음
        }
        else
        {
            var myItem = haveAllItemList.Find(x => (x.itemId == newItem.itemId) && (x.dataType == newItem.dataType));
            // 칸 차지하는 경우의 수 모두 합쳐서 현재 인벤토리칸이 꽉 차있는 경우
            if (maxInventoryItemCount == (myInventoryFullCount + myInventorySpaceCount))
            {
                // 같은종류 아이템을 가지고 있을 떄
                if (myItem != null && myItem.InvenRemainCount != 0)
                {
                    // 남은 여분공간이 추가될 아이템 개수보다 같거나 클때
                    if (myItem.LimitCount - myItem.InvenRemainCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        newItem.OwnCount -= (myItem.LimitCount - myItem.InvenRemainCount); // 서순중요
                        myItem.OwnCount += (myItem.LimitCount - myItem.InvenRemainCount);
                        // 아이템 다 안들어감
                        return false;
                    }
                }
                else
                {
                    // 아이템 못넣음
                    return false;
                }
            }
            // 아직 인벤토리칸에 여유가 있다
            else
            {
                if (myItem != null)
                {
                    if (myItem.LimitCount - myItem.InvenRemainCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        int spaceCount = 0;
                        while (newItem.OwnCount != 0 &&
                            maxInventoryItemCount >= (myInventoryFullCount + myInventorySpaceCount + spaceCount))
                        {
                            myItem.OwnCount++;
                            newItem.OwnCount--;
                            if (myItem.InvenRemainCount == 0)
                            {
                                myInventoryFullCount++;
                                spaceCount = -1;
                            }
                            else
                                spaceCount = 0;

                            if(maxInventoryItemCount < (myInventoryFullCount + myInventorySpaceCount + spaceCount))
                            {
                                myItem.OwnCount--;
                                newItem.OwnCount++;
                                // 아이템 다 안들어감
                                return false;
                            }
                        }
                    }
                }
                // 새로운 아이템 추가
                else
                {
                    int overOwnCount = 0;
                    int spareInvenCount = maxInventoryItemCount - (myInventoryFullCount + myInventorySpaceCount);

                    switch (newItem.dataType)
                    {
                        case DataType.Consume:
                            break;
                        case DataType.AllItem:
                            var getItem = new DataAllItem(newItem);
                            haveAllItemList.Add(getItem);
                            break;
                        case DataType.Material:
                            var getmaterialItem = new DataMaterial(newItem);
                            haveAllItemList.Add(getmaterialItem);
                            break;
                    }

                    var newItemSpare = (newItem.InvenRemainCount == 0) ? 0 : 1;
                    if(spareInvenCount - (newItem.InvenFullCount + newItemSpare) < 0)
                    {
                        var overInvenCount = (newItem.InvenFullCount + newItemSpare) - spareInvenCount;
                        if(newItem.InvenRemainCount == 0)
                        {
                            overOwnCount = newItem.LimitCount * overInvenCount;
                        }
                        else
                        {
                            overOwnCount = (newItem.LimitCount * overInvenCount - 1) + newItem.InvenRemainCount;
                        }

                        newItem.OwnCount -= overOwnCount;
                        var getItem = haveAllItemList.Find(x => x.itemId == newItem.itemId && x.dataType == newItem.dataType);
                        getItem.OwnCount = newItem.OwnCount;
                        newItem.OwnCount = overOwnCount;
                        // 아이템 다 안들어감
                        return false;
                    }
                }
            }
        }
        // 전부 들어감!
        newItem.OwnCount = 0;
        return true;
    }
    public bool RemoveItemData(DataItem removeItem)
    {
        var index = haveAllItemList.FindIndex(x => (x.itemId == removeItem.itemId) && (x.dataType == removeItem.dataType));
        if (index == -1)
        {
            Debug.Log("삭제할 아이템이 없음");
            return false;
        }
        else
        {
            haveAllItemList[index].OwnCount -= removeItem.OwnCount;
            if(haveAllItemList[index].OwnCount <= 0)
            {
                haveAllItemList.RemoveAt(index);
                Debug.Log("아이템 삭제됨");
                return true;
            }
            Debug.Log("아이템 감소됨");
        }
        return false;
    }
    // DataItem 으로 그리고 List로 다시 변환해서 사용해보기
}
