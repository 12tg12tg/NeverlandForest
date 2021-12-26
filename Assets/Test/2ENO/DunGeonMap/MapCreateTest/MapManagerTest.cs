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
        // 1ȸ�� �ʱ�ȭ �ڷ�ƾ ��ٸ��� ����, ���߿� ������ ���� ������ ����?
        if(dungeonGen.isSet)
        {
            // 100 �� ��ŸƮ id
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
    // ���� room�� ���� ����, room�� next�� ���� ����
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
    
    // ���ο� ����� �� ��鿡 ���� ������ ����Ʈ�� �����ؼ� �Ѱ��ֱ� ����
    // �񱳹��� next�� �ִ¼��� �����̳� ���ư��°� �ľ� �������� ��������
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
