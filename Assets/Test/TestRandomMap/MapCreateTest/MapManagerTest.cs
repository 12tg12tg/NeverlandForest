using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MapManagerTest : MonoBehaviour
{
    public MainRoomCtrl mainRoom;
    public MapRoadCtrl roadRoom2;
    public MapRoadCtrl roadRoom3;
    public MapRoadCtrl roadRoom4;
    public MapRoadCtrl roadRoom5;

    public DungeonRoom curRoom;
    public GameObject player;
    private MapRoadCtrl curRoad;

    public DunGeonMapGenerate dungeonGen;


    public TextMeshProUGUI text;
    void Start()
    {
        player.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1회성 초기화 코루틴 기다리기 위해, 나중에 옵저버 패턴 등으로 구현?
        if(dungeonGen.isSet)
        {
            // 100 은 스타트 id
            curRoom = dungeonGen.DungeonRoomList[100];
            SetCurRoad(curRoom);
            curRoad.gameObject.SetActive(false);
            mainRoom.gameObject.SetActive(true);
            player.gameObject.SetActive(true);

            player.transform.position = mainRoom.GetComponent<MainRoomCtrl>().SpawnPos.transform.position;
            dungeonGen.isSet = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            var pos = Vector3.forward * 7f * Time.deltaTime;
            player.transform.position += pos;
        }
    }

    public void SetCurRoad(DungeonRoom curRoom)
    {
        switch(curRoom.nextRoadCount)
        {
            case 2:
                curRoad = roadRoom2;
                break;
            case 3:
                curRoad = roadRoom3;
                break;
            case 4:
                curRoad = roadRoom4;
                break;
            case 5:
                curRoad = roadRoom5;
                break;
        }
    }

    public void changeRoom(bool isEnd)
    { 
        if(isEnd)
        {
            if(curRoom.RoomType == DunGeonRoomType.MainRoom)
            {
                curRoom = curRoom.nextRoom;
                text.SetText(curRoom.GetEvent().ToString());
                player.transform.position = curRoad.GetComponent<MapRoadCtrl>().SpawnPos.transform.position;
                mainRoom.gameObject.SetActive(false);
                curRoad.gameObject.SetActive(true);
            }
            else
            {
                curRoom = curRoom.nextRoom;
                text.SetText(curRoom.GetEvent().ToString());
                player.transform.position = mainRoom.GetComponent<MainRoomCtrl>().SpawnPos.transform.position;
                mainRoom.gameObject.SetActive(true);
                curRoad.gameObject.SetActive(false);
                SetCurRoad(curRoom);
            }
        }
        else
        {
            curRoom = curRoom.nextRoom;
            text.SetText(curRoom.GetEvent().ToString());
        }
    }
}
