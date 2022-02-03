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
    // 채집
    public GatheringObjectType gatheringtype;
    // 랜덤인카운트
    public string randomEventID;
}
public class GatheringData : EventData
{
    public int offSetBasePos;
    GatheringObject gatheringObj;
    public GatheringObject CreateObj(GatheringObject obj, GatheringSystem system)
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
        switch (obj.objectType)
        {
            case GatheringObjectType.Tree:
                gatheringObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                break;
            case GatheringObjectType.Pit:
                gatheringObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(90f, 0f, 0f)));
                break;
            case GatheringObjectType.Herbs:
                gatheringObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                break;
            case GatheringObjectType.Mushroom:
                gatheringObj = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                break;
            default:
                break;
        }
        gatheringObj.Init(system, this, roomIndex);
        obj.objectType = gatheringtype;
        objectPosition = objPos;
        isCreate = true;
        return gatheringObj;
    }
}
public class HuntingData : EventData
{
    // 임시 이벤트 오브젝트 클래스
    public HuntingObject CreateObj(GameObject obj)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if (isCreate)
        {
            var gameObj = Object.Instantiate(obj, objectPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            gameObj.layer = LayerMask.NameToLayer("EventObject");
            // TODO : 박스컬라이더 임시로 만들어서 붙여줌
            var boxCol = gameObj.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;
            boxCol.center = new Vector3(0f, 1f, 0f);
            boxCol.size = new Vector3(2f, 1f, 1.3f);
            var huntingObj2 = gameObj.AddComponent<HuntingObject>();
            huntingObj2.Init(this, roomIndex);
            return huntingObj2;
        }
        var objPos = new Vector3(eventBasePos.x - 2.5f, eventBasePos.y + 1f, eventBasePos.z);
        var gameObj2 = Object.Instantiate(obj, objPos, Quaternion.Euler(new Vector3(0f,90f,0f)));
        gameObj2.layer = LayerMask.NameToLayer("EventObject");
        var boxCol2 = gameObj2.AddComponent<BoxCollider>();
        boxCol2.isTrigger = true;
        boxCol2.center = new Vector3(0f, 1f, 0f);
        boxCol2.size = new Vector3(2f, 1f, 1.3f);
        var huntingObj = gameObj2.AddComponent<HuntingObject>();
        huntingObj.Init(this, roomIndex);
        objectPosition = objPos;
        isCreate = true;
        return huntingObj;
    }
}
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