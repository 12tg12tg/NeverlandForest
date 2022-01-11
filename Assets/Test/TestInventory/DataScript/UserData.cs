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

    public void AddItemData(DataItem newItem)
    {
        // ĭ �ϳ��� ���� ������ ����� ����
        int myInventoryFullCount = 0;
        // ĭ �ϳ��� �Ϻ� ������ ����� ����
        int myInventorySpaceCount = 0;
        
        foreach(var myItem in haveAllItemList)
        {
            if (myItem.SpareCount != 0)
                myInventorySpaceCount++;
            myInventoryFullCount += myItem.FullSpace;
        }

        // ���� �����۵��� Fullī��Ʈ���� �κ��丮 maxī��Ʈ�� �������� ���� ������, 1���� �� ������ ���ٴ� ��
        if (myInventoryFullCount == maxInventoryItemCount)
        {
            Debug.Log("�������� �ʰ��Ǿ� ���� ���� ���մϴ�");
            return;
            // ������ ������
        }
        else
        {
            var myItem = haveAllItemList.Find(x => (x.itemId == newItem.itemId) && (x.dataType == newItem.dataType));
            // ĭ �����ϴ� ����� �� ��� ���ļ� ���� �κ��丮ĭ�� �� ���ִ� ���
            if (maxInventoryItemCount == (myInventoryFullCount + myInventorySpaceCount))
            {
                // �������� �������� ������ ���� ��
                if (myItem != null)
                {
                    // ���� ���а����� �߰��� ������ �������� ���ų� Ŭ��
                    if (myItem.SpareCount >= newItem.OwnCount)
                    {
                        myItem.OwnCount += newItem.OwnCount;
                        newItem.OwnCount = 0;
                    }
                    else
                    {
                        newItem.OwnCount -= myItem.SpareCount; // �����߿�
                        myItem.OwnCount += myItem.SpareCount;
                        // �̶�, newItem�� ���� �� ������ ����
                        Debug.Log("�������� �ʰ��Ǿ� ���� ���� ���մϴ�");
                        return;
                    }
                }
                else
                {
                    // ������ ������
                    Debug.Log("�������� �ʰ��Ǿ� ���� ���� ���մϴ�");
                    return;
                }
            }
            // ���� �κ��丮ĭ�� ������ �ִ�
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
                // ���ο� ������ �߰�
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
                        // overOwnCount ��ŭ�� ���ο� item �����ؼ� ���
                        Debug.Log("�������� �ʰ��Ǿ� ���� ���� ���մϴ�");
                        return;
                    }
                }
            }
        }
        Debug.Log("������ �߰��Ϸ�");
    }
    public void RemoveItemData(DataItem removeItem)
    {
        var index = haveAllItemList.FindIndex(x => (x.itemId == removeItem.itemId) && (x.dataType == removeItem.dataType));
        if (index == -1)
        {
            Debug.Log("������ �������� ����");
            return;
        }
        else
        {
            haveAllItemList[index].OwnCount -= removeItem.OwnCount;
            if(haveAllItemList[index].OwnCount <= 0)
            {
                haveAllItemList.RemoveAt(index);
                Debug.Log("������ ������");
            }
            Debug.Log("������ ���ҵ�");
        }
    }
    // DataItem ���� �׸��� List�� �ٽ� ��ȯ�ؼ� ����غ���
}
