using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager
{
    private DungeonSystem dungeonSystem;

    // �� ������
    public RoomCtrl mainRoomPrefab;
    public RoomCtrl roadRoom2Prefab;
    public RoomCtrl roadRoom3Prefab;
    public RoomCtrl roadRoom4Prefab;
    public RoomCtrl roadRoom5Prefab;

    private RoomCtrl curRoomObjectInfo;
    public DungeonRoom curDungeonInfo;
    public DungeonRoom beforeDungeonInfo;

    public DunGeonMapGenerate dungeonGen;
    public TextMeshProUGUI text;

    private int curRoadCount;
    // ���� ���� = ������ �����⿡ ���� ������� �� �����ʿ� ���� ���� ex �ε���, �̺�Ʈ Ÿ�� ���
    // �� ���� = ���������� �̿��� �� ������, ���κ��� ������� �� 5��

    // �̴ϸ� ǥ��, ������ ������ �´� �� ������ ����, ���� ���� �� �� ���� �ʱ�ȭ
    public void init(DungeonData data, DungeonSystem system)
    {
        if (dungeonSystem == null)
            dungeonSystem = system;
    }

    public void SetCurRoad(DungeonRoom curRoom)
    {
        switch(curRoom.roadCount)
        {
            case 2:
                curRoomObjectInfo = roadRoom2Prefab;
                curRoadCount = 2;
                break;
            case 3:
                curRoomObjectInfo = roadRoom3Prefab;
                curRoadCount = 3;
                break;
            case 4:
                curRoomObjectInfo = roadRoom4Prefab;
                curRoadCount = 4;
                break;
            case 5:
                curRoomObjectInfo = roadRoom5Prefab;
                curRoadCount = 5;
                break;
        }
        //dungeonSystem.DungeonSystemData.curRoadCount = curRoadCount;
    }
    public void SetCurRoad(int curRoadCount)
    {
        switch (curRoadCount)
        {
            case 2:
                curRoomObjectInfo = roadRoom2Prefab;
                break;
            case 3:
                curRoomObjectInfo = roadRoom3Prefab;
                break;
            case 4:
                curRoomObjectInfo = roadRoom4Prefab;
                break;
            case 5:
                curRoomObjectInfo = roadRoom5Prefab;
                break;
        }
    }
    // �̴ϸʿ� ���� �ִ� �� ǥ���ϱ� ����
    public void SetCheckRoom(DungeonRoom curRoom, DungeonRoom beforeRoom)
    {
        var obj = dungeonSystem.DungeonSystemData.dungeonRoomObjectList.Find(x => x.roomIdx == curRoom.roomIdx);
        var mesh = obj.gameObject.GetComponent<MeshRenderer>();
        mesh.material.color = Color.blue;

        if (beforeRoom.IsCheck == true)
        {
            var obj2 = dungeonSystem.DungeonSystemData.dungeonRoomObjectList.Find(x => x.roomIdx == beforeRoom.roomIdx);
            var mesh2 = obj2.gameObject.GetComponent<MeshRenderer>();

            mesh2.material.color = (beforeRoom.RoomType == DunGeonRoomType.MainRoom) ?
            new Color(0.962f, 0.174f, 0.068f) : new Color(0.472f, 0.389f, 0.389f);
        }
    }
    public void ChangeRoomForward(bool isEnd)
    {
        beforeDungeonInfo = curDungeonInfo;
        if (isEnd)
        {
            //curRoomObjectInfo.gameObject.SetActive(false);
            if (curDungeonInfo.RoomType == DunGeonRoomType.MainRoom)
            {
                if(curDungeonInfo.nextRoomIdx == -1)
                {
                    Vars.UserData.WorldMapPlayerData.isClear = true;
                    SceneManager.LoadScene("WorldMap");
                    return;
                }
                //SetCurRoad(curDungeonInfo);
            }
            else
            {
                //curRoomObjectInfo = mainRoomPrefab;
            }
            CurOnRoomList(curDungeonInfo);
            //curRoomObjectInfo.gameObject.SetActive(true);
            curDungeonInfo = GetNextRoom(curDungeonInfo);

            //dungeonSystem.DungeonSystemData.curRoomData = curRoomObjectInfo;
        }
        else
        {
            curDungeonInfo = GetNextRoom(curDungeonInfo);
        }
        
        SetText(curDungeonInfo);
        //SetCheckRoom(curDungeonInfo, beforeDungeonInfo);
        //dungeonSystem.DungeonSystemData.curDungeonData = curDungeonInfo;
    }
    public void ChangeRoomGoBack()
    {
        beforeDungeonInfo = curDungeonInfo;
        curDungeonInfo = GetBeforeRoom(curDungeonInfo);

        SetText(curDungeonInfo);
        //SetCheckRoom(curDungeonInfo, beforeDungeonInfo);
        //dungeonSystem.DungeonSystemData.curRoomData = curRoomObjectInfo;
        //dungeonSystem.DungeonSystemData.curDungeonData = curDungeonInfo;
    }


    public void SetText(DungeonRoom roomInfo)
    {
        StringBuilder sb = new StringBuilder();
        for(int i=0; i < roomInfo.eventList.Count; i++)
        {
            sb.Append(roomInfo.eventList[i].ToString());
            sb.Append("\n");
        }
        text.SetText(sb.ToString());
    }
    
    // ���ο� ����� �� ��鿡 ���� ������ ����Ʈ�� �����ؼ� �Ѱ��ֱ� ����
    public void CurOnRoomList(DungeonRoom curRoomInfo)
    {
        var roomList = new List<DungeonRoom>();
        // ���� end�� ���� ���� => �������� �����, �ݴ��쵵 ��������
        // �������� ������� ��� ����Ʈ�� �̾Ƽ� �ѹ��� �̺�Ʈ ������Ʈ �ؾ���

        if(curRoomInfo.RoomType == DunGeonRoomType.MainRoom)
        {
            curRoomInfo = GetNextRoom(curRoomInfo);
            while (curRoomInfo.RoomType != DunGeonRoomType.MainRoom)
            {
                roomList.Add(curRoomInfo);
                curRoomInfo = GetNextRoom(curRoomInfo);
            }
        }
        else
        {
            curRoomInfo = GetNextRoom(curRoomInfo);
            roomList.Add(curRoomInfo);
        }

        //dungeonSystem.DungeonSystemData.curIncludeRoomList = roomList;
    }
    // + �񱳹��� next�� �־� ���ϴ¼��� �����̳� ���ư��°� �ľ� �������� ��������

    public DungeonRoom GetNextRoom(DungeonRoom curRoom)
    {
        if (curRoom.nextRoomIdx == -1)
            return null;
        var nextRoom = dungeonSystem.DungeonSystemData.dungeonRoomArray[curRoom.nextRoomIdx];
        return nextRoom;
    }

    public DungeonRoom GetBeforeRoom(DungeonRoom curRoom)
    {
        if (curRoom.beforeRoomIdx == -1)
            return null;
        var beforeRoom = dungeonSystem.DungeonSystemData.dungeonRoomArray[curRoom.beforeRoomIdx];
        return beforeRoom;
    }
}