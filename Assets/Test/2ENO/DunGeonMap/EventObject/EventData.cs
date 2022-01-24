using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventData
{
    public DunGeonEvent eventType;
    public Vector3 eventBasePos;
    // ù ��������, �̹� �������� �ִ� �̺�Ʈ����
    public bool isCreate;
    public Vector3 objectPosition;
    // �̺�Ʈ�� ������ �� ����, ������Ʈ ������ �� ������ �ְ� �����Ҷ� �� ���� ���� �����ϱ�����
    public int roomIndex;
    public GatheringObjectType gatheringtype;

    // ���ڿ��� ������ �ȵǳ�????
    public string randomEventID;
}
[System.Serializable]
public class GatheringData : EventData
{
    public int offSetBasePos;

    public GatheringObject Createobj(GatheringObject obj, GatheringSystem system, DungeonSystem dgSystem)
    {
        if(eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("���̽� �������� �ʱ�ȭ���� ����!");
            return null;
        }
        if(isCreate)
        {
            var gatheringObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            gatheringObj2.Init(system, this, dgSystem, roomIndex);
            obj.objectType = gatheringtype;
            return gatheringObj2;
        }
        var objPos = new Vector3(eventBasePos.x + offSetBasePos, eventBasePos.y, eventBasePos.z );
        var gatheringObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        obj.objectType = gatheringtype;
        gatheringObj.Init(system, this, dgSystem, roomIndex);

        objectPosition = objPos;
        isCreate = true;
        return gatheringObj;
    }
}
[System.Serializable]
public class HuntingData : EventData
{
    // �ӽ� �̺�Ʈ ������Ʈ Ŭ����
    public HuntingObject Createobj(HuntingObject obj, DungeonSystem dgSystem)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("���̽� �������� �ʱ�ȭ���� ����!");
            return null;
        }
        if (isCreate)
        {
            var huntingObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            huntingObj2.Init(dgSystem, this, roomIndex);
            return huntingObj2;
        }
        var objPos = new Vector3(eventBasePos.x - 2.5f, eventBasePos.y + 1f, eventBasePos.z);
        var huntingObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f,90f,0f)));
        huntingObj.Init(dgSystem, this, roomIndex);
        objectPosition = objPos;
        isCreate = true;
        return huntingObj;
    }
}
[System.Serializable]
public class BattleData : EventData
{
    public BattleObject CreateObj(BattleObject obj, DungeonSystem dgSystem)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("���̽� �������� �ʱ�ȭ���� ����!");
            return null;
        }
        if (isCreate)
        {
            var battleObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            battleObj2.Init(dgSystem, this, roomIndex);
            return battleObj2;
        }
        var objPos = new Vector3(eventBasePos.x + 5f, eventBasePos.y, eventBasePos.z);
        var battleObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        battleObj.Init(dgSystem, this, roomIndex);
        objectPosition = objPos;
        isCreate = true;
        return battleObj;
    }
}

public class RandomIncountData : EventData
{
    public RandomEventObject CreateObj(RandomEventObject obj, DungeonSystem dgSystem)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("���̽� �������� �ʱ�ȭ���� ����!");
            return null;
        }
        if (isCreate)
        {
            var randomObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            randomObj2.Init(dgSystem, this, roomIndex, randomEventID);
            return randomObj2;
        }
        var objPos = new Vector3(eventBasePos.x - 5f, eventBasePos.y, eventBasePos.z);
        var randomObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        randomObj.Init(dgSystem, this, roomIndex, randomEventID);
        objectPosition = objPos;
        isCreate = true;
        return randomObj;
    }
}