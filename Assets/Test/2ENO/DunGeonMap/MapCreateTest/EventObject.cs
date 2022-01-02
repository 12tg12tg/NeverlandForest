using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventObjectData
{
    public DungeonRoom roomInfo;
    public DunGeonEvent eventType;
    public Vector3 objectPosition;

    public EventObjectData(EventObject obj)
    {
        roomInfo = obj.roomInfo;
        eventType = obj.eventType;
        objectPosition = obj.objectPosition;
    }
}

public class EventObject : MonoBehaviour, IPointerClickHandler
{
    EventObjectData eventObjInfo;
    public DungeonRoom roomInfo;
    public DunGeonEvent eventType;
    public Vector3 objectPosition;
    private DungeonSystem dungeonSystem;

    public EventObject() { }

    public EventObject(EventObject obj)
    {
        roomInfo = obj.roomInfo;
        eventType = obj.eventType;
        objectPosition = obj.objectPosition;
    }

    public void Init(DungeonRoom curRoomInfo, DunGeonEvent curEvnet, DungeonSystem system, Vector3 objectPos)
    {
        objectPosition = objectPos;
        dungeonSystem = system;
        roomInfo = curRoomInfo;
        eventType = curEvnet;
        var mesh = gameObject.GetComponent<MeshRenderer>();
        switch (curEvnet)
        {
            case DunGeonEvent.Empty:
                Destroy(gameObject);
                break;
            case DunGeonEvent.Battle:
                mesh.material.color = Color.red;
                break;
            case DunGeonEvent.Gathering:
                mesh.material.color = Color.green;
                break;
            case DunGeonEvent.Hunt:
                mesh.material.color = Color.blue;
                break;
            case DunGeonEvent.RandomIncount:
                mesh.material.color = Color.white;
                break;
            case DunGeonEvent.SubStory:
                mesh.material.color = Color.black;
                break;
            case DunGeonEvent.Count:
                break;
        }
        
        if (dungeonSystem.DungeonSystemData.curEventObjList.FindIndex( x => x.objectPosition.Equals(objectPosition)) == -1)
        {
            EventObjectData obj = new EventObjectData(this);
            dungeonSystem.DungeonSystemData.curEventObjList.Add(obj);
            eventObjInfo = obj;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭이동!");
        dungeonSystem.EventObjectClickEvent(eventType, this);
        dungeonSystem.DungeonSystemData.curEventObjList.Remove(eventObjInfo);
        Destroy(gameObject);

    }
}
