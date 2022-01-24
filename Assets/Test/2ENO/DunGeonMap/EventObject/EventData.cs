using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventData
{
    public DunGeonEvent eventType;
    public Vector3 eventBasePos;
    // 첫 생성인지, 이미 생성한적 있던 이벤트인지
    public bool isCreate;
    public Vector3 objectPosition;
    // 이벤트를 생성한 방 정보, 오브젝트 생성시 이 정보를 넣고 삭제할때 그 방의 값에 적용하기위해
    public int roomIndex;
    public GatheringObjectType gatheringtype;

    // 문자열은 저장이 안되나????
    public string randomEventID;
}
[System.Serializable]
public class GatheringData : EventData
{
    public int offSetBasePos;

    public GatheringObject Createobj(GatheringObject obj, GatheringSystem system)
    {
        if(eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if(isCreate)
        {
            var gatheringObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            gatheringObj2.Init(system, this, roomIndex);
            obj.objectType = gatheringtype;
            return gatheringObj2;
        }
        var objPos = new Vector3(eventBasePos.x + offSetBasePos, eventBasePos.y, eventBasePos.z );
        var gatheringObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        obj.objectType = gatheringtype;
        gatheringObj.Init(system, this, roomIndex);

        objectPosition = objPos;
        isCreate = true;
        return gatheringObj;
    }
}
[System.Serializable]
public class HuntingData : EventData
{
    // 임시 이벤트 오브젝트 클래스
    public HuntingObject Createobj(HuntingObject obj)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if (isCreate)
        {
            var huntingObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            huntingObj2.Init(this, roomIndex);
            return huntingObj2;
        }
        var objPos = new Vector3(eventBasePos.x - 2.5f, eventBasePos.y + 1f, eventBasePos.z);
        var huntingObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f,90f,0f)));
        huntingObj.Init(this, roomIndex);
        objectPosition = objPos;
        isCreate = true;
        return huntingObj;
    }
}
[System.Serializable]
public class BattleData : EventData
{
    public BattleObject CreateObj(BattleObject obj)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if (isCreate)
        {
            var battleObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            battleObj2.Init(this, roomIndex);
            return battleObj2;
        }
        var objPos = new Vector3(eventBasePos.x + 5f, eventBasePos.y, eventBasePos.z);
        var battleObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        battleObj.Init(this, roomIndex);
        objectPosition = objPos;
        isCreate = true;
        return battleObj;
    }
}

public class RandomIncountData : EventData
{
    public RandomEventObject CreateObj(RandomEventObject obj)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if (isCreate)
        {
            var randomObj2 = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            randomObj2.Init(this, roomIndex, randomEventID);
            return randomObj2;
        }
        var objPos = new Vector3(eventBasePos.x - 5f, eventBasePos.y, eventBasePos.z);
        var randomObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        randomObj.Init(this, roomIndex, randomEventID);
        objectPosition = objPos;
        isCreate = true;
        return randomObj;
    }
}