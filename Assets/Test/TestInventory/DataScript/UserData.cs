using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum ArrowType { None, Normal, Iron }

public class UserData
{
    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!      �������� �ٸ� ������       !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    //Item Info
    public ArrowType arrowType;

    //World Info
    public List<WorldMapNodeStruct> WorldMapNodeStruct { get; set; } = new List<WorldMapNodeStruct>();
    public List<WorldMapTreeInfo> WorldMapTree { get; set; } = new List<WorldMapTreeInfo>();
    public WorldMapPlayerData WorldMapPlayerData { get; set; }

    public WorldMapNode curNode;

    // ������ ������, ���̺� �ε�
    public SerializeDictionary<Vector2, DungeonData> AllDungeonData { get; set; } = new SerializeDictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;
    public int dungeonStartIdx = 100;
    public int dungeonLastIdx;
    public bool isDungeonReStart;
    public bool isDungeonClear;

    // �����̺�Ʈ ������, ���̺� �ε�
    public List<DataRandomEvent> randomEventDatas = new List<DataRandomEvent>();
    public List<string> useEventID = new List<string>();
    public bool isRandomDataLoad;
    public bool isFirst = true;

    // Ʃ�丮�� ����
    public bool isTutorialDungeon = true;
    public DungeonData tutorialDungeonData = new DungeonData();

    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();
    //Experienced Craft
    public List<string> HaveCraftIDList { get; set; } = new List<string>();

    //??? - Vars�� �̻簡�� �Ǵ°�?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    //ConsumeManager
    //�Ƿε��� ���������� �پ��� ��ġ
    public CostData uData = new CostData();

    // �κ��丮�� ���
    public int maxInventoryItemCount = 12;
    private readonly List<DataAllItem> haveAllItemList = new List<DataAllItem>();
    public ReadOnlyCollection<DataAllItem> HaveAllItemList => haveAllItemList.AsReadOnly();
    public List<string> experienceHaveItemList = new List<string>();
    //����ϴ� �Լ�

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
            Debug.Log("�̹� ȹ���غ�");
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
        // ĭ �ϳ��� ���� ������ ����� ����
        int myInventoryFullCount = 0;
        // ĭ �ϳ��� �Ϻ� ������ ����� ����
        int myInventorySpaceCount = 0;

        foreach (var myItem in haveAllItemList)
        {
            if (myItem.InvenRemainCount != 0)
                myInventorySpaceCount++;
            myInventoryFullCount += myItem.InvenFullCount;
        }

        // ���� �����۵��� Fullī��Ʈ���� �κ��丮 maxī��Ʈ�� �������� ���� ������, 1���� �� ������ ���ٴ� ��
        if (myInventoryFullCount == maxInventoryItemCount)
        {
            Debug.Log("�������� �ʰ��Ǿ� ���� ���� ���մϴ�");
            return false;
            // ������ ������
        }
        else
        {
            var myItem = haveAllItemList.Find(x => (x.itemId == newItem.itemId));
            // ĭ �����ϴ� ����� �� ��� ���ļ� ���� �κ��丮ĭ�� �� ���ִ� ���
            if (maxInventoryItemCount == (myInventoryFullCount + myInventorySpaceCount))
            {
                // �������� �������� ������ ���� ��
                if (myItem != null && myItem.InvenRemainCount != 0)
                {
                    // ���� ���а����� �߰��� ������ �������� ���ų� Ŭ��
                    if (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        newItem.OwnCount -= (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount); // �����߿�
                        myItem.OwnCount += (myItem.ItemTableElem.limitCount - myItem.InvenRemainCount);
                        // ������ �� �ȵ�
                        return false;
                    }
                }
                else
                {
                    // ������ ������
                    return false;
                }
            }
            // ���� �κ��丮ĭ�� ������ �ִ�
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
                                // ������ �� �ȵ�
                                return false;
                            }
                        }
                    }
                }
                // ���ο� ������ �߰�
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
                        // ������ �� �ȵ�
                        return false;
                    }
                }
            }
        }
        // ���� ��!
        newItem.OwnCount = 0;
        return true;
    }
    public bool RemoveItemData(DataAllItem removeItem)
    {
        var index = haveAllItemList.FindIndex(x => (x.itemId == removeItem.itemId));
        if (index == -1)
        {
            Debug.Log("������ �������� ����");
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
                Debug.Log("������ ������");
                return true;
            }
            Debug.Log("������ ���ҵ�");
        }
        return false;
    }
    // DataItem ���� �׸��� List�� �ٽ� ��ȯ�ؼ� ����غ���
}
