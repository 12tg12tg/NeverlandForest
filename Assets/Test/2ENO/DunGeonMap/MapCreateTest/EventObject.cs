using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public DungeonRoom roomInfo;

    public void Init(DungeonRoom curRoomInfo, DunGeonEvent curEvnet)
    {
        roomInfo = curRoomInfo;

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
    }
}
