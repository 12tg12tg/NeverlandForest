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
}
[System.Serializable]
public class GatheringData : EventData
{
    public int offSetBasePos;

    public GatheringObject Createobj(GatheringObject obj, GatheringSystem system, DungeonSystem dgSystem)
    {
        if(eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if(isCreate)
        {
            var gatheringObj2 = Object.Instantiate(obj, objectPosition, Quaternion.identity);
            gatheringObj2.Init(system, this, dgSystem, roomIndex);
            return gatheringObj2;
        }
        var objPos = new Vector3(eventBasePos.x, eventBasePos.y, eventBasePos.z + offSetBasePos);
        var gatheringObj = Object.Instantiate(obj, objPos, Quaternion.identity);
        gatheringObj.Init(system, this, dgSystem, roomIndex);

        objectPosition = objPos;
        isCreate = true;
        return gatheringObj;
    }
}
[System.Serializable]
public class HuntingData : EventData
{
    // 임시 이벤트 오브젝트 클래스
    public EventObject Createobj(EventObject obj, DungeonSystem dgSystem)
    {
        if (eventBasePos.Equals(Vector3.zero))
        {
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if (isCreate)
        {
            var huntingObj2 = Object.Instantiate(obj, objectPosition, Quaternion.identity);
            huntingObj2.Init(dgSystem, this, roomIndex);
            return huntingObj2;
        }
        var objPos = new Vector3(eventBasePos.x, eventBasePos.y, eventBasePos.z - 2);
        var huntingObj = Object.Instantiate(obj, objPos,Quaternion.identity);
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
            Debug.Log("베이스 포지션이 초기화되지 않음!");
            return null;
        }
        if (isCreate)
        {
            var battleObj2 = Object.Instantiate(obj, objectPosition, Quaternion.identity);
            return battleObj2;
        }
        var objPos = new Vector3(eventBasePos.x, eventBasePos.y, eventBasePos.z-3f);
        var battleObj = Object.Instantiate(obj, objPos, Quaternion.identity);
        //battleObj.Init(dgSystem, this, roomIndex);
        objectPosition = objPos;
        isCreate = true;
        return battleObj;
    }
}