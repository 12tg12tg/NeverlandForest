using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapManagerTest : MonoBehaviour
{
    public RoomCtrl mainRoomPrefab;
    public RoomCtrl roadRoom2Prefab;
    public RoomCtrl roadRoom3Prefab;
    public RoomCtrl roadRoom4Prefab;
    public RoomCtrl roadRoom5Prefab;

    private RoomCtrl curDungeonRoom;
    public DungeonRoom curRoomInfo;
    public DungeonRoom beforeRoomInfo;

    public GameObject player;

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
            curRoomInfo = dungeonGen.dungeonRoomArray[100];
            curDungeonRoom = mainRoomPrefab;
            SetCheckRoom(curRoomInfo,beforeRoomInfo);
            curDungeonRoom.gameObject.SetActive(true);
            player.gameObject.SetActive(true);

            player.transform.position = mainRoomPrefab.spawnPos.transform.position;
            //curDungeonRoom.CreateAllEventObject(GetRoomInfoList(curRoomInfo));

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
                curDungeonRoom = roadRoom2Prefab;
                break;
            case 3:
                curDungeonRoom = roadRoom3Prefab;
                break;
            case 4:
                curDungeonRoom = roadRoom4Prefab;
                break;
            case 5:
                curDungeonRoom = roadRoom5Prefab;
                break;
        }
    }
    public void SetCheckRoom(DungeonRoom curRoom, DungeonRoom beforeRoom)
    {
        var obj = dungeonGen.dungeonRoomObjectList.Find(x => x.roomInfo.Pos.Equals(curRoom.Pos));
        var mesh = obj.gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;

        if (beforeRoom.IsCheck == true)
        {
            var obj2 = dungeonGen.dungeonRoomObjectList.Find(x => x.roomInfo.Pos.Equals(beforeRoom.Pos));
            var mesh2 = obj2.gameObject.GetComponent<MeshRenderer>();

            mesh2.material.color = (beforeRoom.RoomType == DunGeonRoomType.MainRoom) ?
            new Color(0.962f, 0.174f, 0.068f) : new Color(0.472f, 0.389f, 0.389f);
        }
    }
    public void ChangeRoom(bool isEnd)
    {
        ConsumeManager.TimeUp(1,0);

        if(isEnd)
        {
            beforeRoomInfo = curRoomInfo;
            if(curRoomInfo.RoomType == DunGeonRoomType.MainRoom)
            {
                if(curRoomInfo.nextRoomIdx == -1)
                {
                    SceneManager.LoadScene("WorldMap");
                    return;
                }
                bool temp = true;
                if (curRoomInfo.nextRoadCount == 5)
                    temp = false;

                curDungeonRoom.DestroyAllEventObject();
                curDungeonRoom.gameObject.SetActive(false);
                SetCurRoad(curRoomInfo);
                curDungeonRoom.gameObject.SetActive(true);

                curRoomInfo = curRoomInfo.nextRoom;
                SetText(curRoomInfo);
                player.transform.position = curDungeonRoom.spawnPos.transform.position;

                if(temp)
                    curDungeonRoom.CreateAllEventObject(GetRoomInfoList(curRoomInfo));
            }
            else
            {
                curDungeonRoom.DestroyAllEventObject();
                curDungeonRoom.gameObject.SetActive(false);
                curDungeonRoom = mainRoomPrefab;
                curDungeonRoom.gameObject.SetActive(true);

                curRoomInfo = curRoomInfo.nextRoom;
                SetText(curRoomInfo);
                player.transform.position = curDungeonRoom.spawnPos.transform.position;

                curDungeonRoom.CreateAllEventObject(GetRoomInfoList(curRoomInfo));
            }
        }
        else
        {
            beforeRoomInfo = curRoomInfo;
            curRoomInfo = curRoomInfo.nextRoom;
            SetText(curRoomInfo);
        }
        SetCheckRoom(curRoomInfo, beforeRoomInfo);
    }

    public void SetText(DungeonRoom roomInfo)
    {
        StringBuilder sb = new StringBuilder();
        for(int i=0; i<roomInfo.eventList.Count; i++)
        {
            sb.Append(roomInfo.eventList[i].ToString());
            sb.Append("\n");
        }
        text.SetText(sb.ToString());
    }
    
    // 메인에 연결된 길 방들에 대한 정보를 리스트로 정리해서 넘겨주기 위한
    // 비교문에 next를 넣는순간 추적이나 돌아가는거 파악 직관성이 떨어진다
    public List<DungeonRoom> GetRoomInfoList(DungeonRoom curRoomInfo)
    {
        var roomList = new List<DungeonRoom>();

        if(curRoomInfo.RoomType == DunGeonRoomType.SubRoom)
        {
            while(curRoomInfo.RoomType != DunGeonRoomType.MainRoom)
            {
                roomList.Add(curRoomInfo);
                curRoomInfo = curRoomInfo.nextRoom;
            }
        }
        else
        {
            roomList.Add(curRoomInfo);
        }
        return roomList;
    }
}
