using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UserData
{
    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    !!      �������� �ٸ� ������       !!
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    //Item Info
    public List<DataConsumable> ConsumableItemList { get; set; } = new List<DataConsumable>();

    //World Info
    public List<MapNodeStruct_0> WorldMapNodeStruct { get; set; } = new List<MapNodeStruct_0>();
    public WorldMapPlayerData WorldMapPlayerData { get; set; }

    // ������ ������, ���̺� �ε�
    public SerializeDictionary<Vector2, DungeonData> CurAllDungeonData { get; set; } = new SerializeDictionary<Vector2, DungeonData>();
    public Vector2 curDungeonIndex;
    public int curDungeonRoomIdx;
    public int dungeonStartIdx;
    public bool dungeonReStart;
    public int currentDundeonRoomIndex;
    //Experienced Recipe
    public List<string> HaveRecipeIDList { get; set; } = new List<string>();

    //??? - Vars�� �̻簡�� �Ǵ°�?
    public List<DataMaterial> HaveMaterialList { get; set; } = new List<DataMaterial>();

    //ConsumeManager
    //�Ƿε��� ���������� �پ��� ��ġ
    public CostData uData = new CostData();

    // �κ��丮�� ���
    public int maxInventoryItemCount = 21;
    private readonly List<DataItem> haveAllItemList = new List<DataItem>();
    public ReadOnlyCollection<DataItem> HaveAllItemList => haveAllItemList.AsReadOnly();
    public bool AddItemData(DataItem newItem)
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
            var myItem = haveAllItemList.Find(x => (x.itemId == newItem.itemId) && (x.dataType == newItem.dataType));
            // ĭ �����ϴ� ����� �� ��� ���ļ� ���� �κ��丮ĭ�� �� ���ִ� ���
            if (maxInventoryItemCount == (myInventoryFullCount + myInventorySpaceCount))
            {
                // �������� �������� ������ ���� ��
                if (myItem != null && myItem.InvenRemainCount != 0)
                {
                    // ���� ���а����� �߰��� ������ �������� ���ų� Ŭ��
                    if (myItem.LimitCount - myItem.InvenRemainCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        newItem.OwnCount -= (myItem.LimitCount - myItem.InvenRemainCount); // �����߿�
                        myItem.OwnCount += (myItem.LimitCount - myItem.InvenRemainCount);
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
    public bool RemoveItemData(DataItem removeItem)
    {
        var index = haveAllItemList.FindIndex(x => (x.itemId == removeItem.itemId) && (x.dataType == removeItem.dataType));
        if (index == -1)
        {
            Debug.Log("������ �������� ����");
            return false;
        }
        else
        {
            haveAllItemList[index].OwnCount -= removeItem.OwnCount;
            if(haveAllItemList[index].OwnCount <= 0)
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
