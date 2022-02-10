using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum ArrowType { None, Normal, Iron }

public class UserData
{
    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!      유저마다 다른 변수들       !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    //Scene Datas
    public MainTutorialStage mainTutorial;
    public ContentsTutorialProceed contentsTutorial;

    //Item Info
    public ArrowType arrowType;

    //World Info
    public List<WorldMapNodeStruct> WorldMapNodeStruct { get; set; } = new List<WorldMapNodeStruct>();
    public List<WorldMapTreeInfo> WorldMapTree { get; set; } = new List<WorldMapTreeInfo>();
    public WorldMapPlayerData WorldMapPlayerData { get; set; }

    public WorldMapNode curNode;

    // 던전맵 데이터, 세이브 로드
    public SerializeDictionary<Vector2, DungeonData> AllDungeonData { get; set; } = new SerializeDictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;
    public int dungeonStartIdx = 100;
    public int dungeonLastIdx;
    public bool isDungeonReStart;
    public bool isDungeonClear;

    // 랜덤이벤트 데이터, 세이브 로드
    public List<DataRandomEvent> randomEventDatas = new List<DataRandomEvent>();
    public List<string> useEventID = new List<string>();
    public bool isRandomDataLoad;
    public bool isFirst = true;

    // 튜토리얼 던전
    public DungeonData tutorialDungeonData = new DungeonData();

    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();
    //Experienced Craft
    public List<string> HaveCraftIDList { get; set; } = new List<string>();

    //??? - Vars로 이사가도 되는가?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    //ConsumeManager
    //피로도는 실질적으로 줄어드는 수치
    public CostData uData = new CostData();

    // 인벤토리에 사용
    public int maxInventoryItemCount = 12;
    private readonly List<DataAllItem> haveAllItemList = new List<DataAllItem>();
    public ReadOnlyCollection<DataAllItem> HaveAllItemList => haveAllItemList.AsReadOnly();
    public List<string> experienceHaveItemList = new List<string>();
    //기록하는 함수

    public void ExperienceListAdd(string itemid) //ITEM_
    {
        if (!experienceHaveItemList.Contains(itemid))
        {
            experienceHaveItemList.Add(itemid);
            Debug.Log($"itemid{ itemid}");
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.ItemExperience);
            UpdateRecipe();
        }
        else
        {
            Debug.Log("이미 획득해봄");
        }
    }

    public void AddRecipeList(string resultId)  //RE_01
    {
        var userRecipeDataList = Vars.UserData.HaveRecipeIDList;
        if (!userRecipeDataList.Contains(resultId))
        {
            userRecipeDataList.Add(resultId);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);
        }
    }

    public void UpdateRecipe()
    {
        var recipeTable = DataTableManager.GetTable<RecipeDataTable>();
        var idList = recipeTable.allRecipeIdList;
        var resultIdDic = recipeTable.allRecipeDictionary;
      
        for (int i = 0; i < idList.Count; i++)
        {
            var resultid = resultIdDic[idList[i]];
            var materials = recipeTable.GetCombination(resultid);
            bool isExperience = true;
            for (int j = 0; j < materials.Length; j++)
            {
                if (materials[j] == "0")
                    continue;
                var stringid = $"ITEM_{materials[j]}";
                if (!experienceHaveItemList.Contains(stringid))
                {
                    isExperience = false;
                    break;
                }
            }
            if (isExperience)
            {   
                AddRecipeList(recipeTable.GetRecipeId(resultid));
            }
        }
    }


    public bool AddItemData(DataAllItem newItem)
    {
        if (newItem.OwnCount == 0)
            return false;

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
            var myItem = haveAllItemList.Find(x => (x.itemId == newItem.itemId));
            // 칸 차지하는 경우의 수 모두 합쳐서 현재 인벤토리칸이 꽉 차있는 경우
            if (maxInventoryItemCount == (myInventoryFullCount + myInventorySpaceCount))
            {
                // 같은종류 아이템을 가지고 있을 떄
                if (myItem != null && myItem.InvenRemainCount != 0)
                {
                    // 남은 여분공간이 추가될 아이템 개수보다 같거나 클때
                    if (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        newItem.OwnCount -= (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount); // 서순중요
                        myItem.OwnCount += (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount);
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
                    if (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount >= newItem.OwnCount)
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

                            if (maxInventoryItemCount < (myInventoryFullCount + myInventorySpaceCount + spaceCount))
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

                    var getItem = new DataAllItem(newItem);
                    haveAllItemList.Add(getItem);

                    var newItemSpare = (newItem.InvenRemainCount == 0) ? 0 : 1;
                    if (spareInvenCount - (newItem.InvenFullCount + newItemSpare) < 0)
                    {
                        var overInvenCount = (newItem.InvenFullCount + newItemSpare) - spareInvenCount;
                        if (newItem.InvenRemainCount == 0)
                        {
                            overOwnCount = newItem.ItemTableElem.limitCount * overInvenCount;
                        }
                        else
                        {
                            overOwnCount = (newItem.ItemTableElem.limitCount * (overInvenCount - 1)) + newItem.InvenRemainCount;
                        }

                        newItem.OwnCount -= overOwnCount;
                        myItem = haveAllItemList.Find(x => x.itemId == newItem.itemId);
                        myItem.OwnCount = newItem.OwnCount;
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
    public bool RemoveItemData(DataAllItem removeItem)
    {
        var index = haveAllItemList.FindIndex(x => (x.itemId == removeItem.itemId));
        if (index == -1)
        {
            Debug.Log("삭제할 아이템이 없음");
            return true;
        }
        else
        {
            if (removeItem.OwnCount < 0)
                haveAllItemList[index].OwnCount += removeItem.OwnCount;
            else
                haveAllItemList[index].OwnCount -= removeItem.OwnCount;
            if (haveAllItemList[index].OwnCount <= 0)
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
