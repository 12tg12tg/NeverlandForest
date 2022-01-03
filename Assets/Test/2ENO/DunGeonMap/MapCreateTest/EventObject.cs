using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventObject : MonoBehaviour, IPointerClickHandler
{
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
                gameObject.AddComponent<GatheringObject>();
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
            var eventObj = new EventObject(this);
            dungeonSystem.DungeonSystemData.curEventObjList.Add(eventObj);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭이동!");
        //EventBus<DungeonMap>.Publish(DungeonMap.EventObjectClick, eventType, transform.position);
      
        switch (eventType)
        {
            case DunGeonEvent.Gathering:

                break;
            case DunGeonEvent.Empty:
            case DunGeonEvent.Battle:
            case DunGeonEvent.Hunt:
            case DunGeonEvent.RandomIncount:
            case DunGeonEvent.SubStory:
            case DunGeonEvent.Count:
                dungeonSystem.EventObjectClickEvent(eventType, this);
                break;
            default:
                break;
        }
    }
}
