using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapGenerate : MonoBehaviour
{
    public List<RoomObject> dungeonRoomObjectList = new List<RoomObject>();
    public MiniMapCamMove minimapCam;
    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public RoomObject lastRoomPrefab;

    public void CreateMiniMapObject()
    {
        int startIndex;
        if (Vars.UserData.mainTutorial != MainTutorialStage.Clear)
        {
            startIndex = 0;
        }
        else
        {
            startIndex = Vars.UserData.dungeonStartIdx;
        }
        int curIdx = startIndex;
        int left, right, top, bottom;
        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos = Vector3.zero;
        Vector3 topPos = Vector3.zero;
        Vector3 bottomPos = Vector3.zero;

        left = curIdx % 20;
        right = curIdx % 20;
        top = curIdx / 20;
        bottom = curIdx / 20;

        /*dungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx*/
        if (DungeonSystem.Instance !=null)
        {
            while (DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx != -1)
            {
                var room = DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[curIdx];
                RoomObject obj;
                if (room.RoomType == DunGeonRoomType.MainRoom)
                {
                    obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, 0f, room.Pos.y)
                         , Quaternion.identity, transform);
                    var objectInfo = obj.GetComponent<RoomObject>();
                    objectInfo.roomIdx = room.roomIdx;
                    dungeonRoomObjectList.Add(obj);
                }
                else
                {
                    obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, 0f, room.Pos.y)
                    , Quaternion.identity, transform);
                    var objectInfo = obj.GetComponent<RoomObject>();
                    objectInfo.roomIdx = room.roomIdx;
                    dungeonRoomObjectList.Add(obj);
                }
                if (curIdx == startIndex)
                {
                    leftPos = obj.transform.position;
                    rightPos = obj.transform.position;
                    topPos = obj.transform.position;
                    bottomPos = obj.transform.position;
                }

                if (curIdx != 0)
                {
                    if (left > curIdx % 20)
                    {
                        left = curIdx % 20;
                        leftPos = obj.transform.position;
                    }
                    if (right < curIdx % 20)
                    {
                        right = curIdx % 20;
                        rightPos = obj.transform.position;
                    }
                    if (top > curIdx / 20)
                    {
                        top = curIdx / 20;
                        topPos = obj.transform.position;
                    }
                    if (bottom < curIdx / 20)
                    {
                        bottom = curIdx / 20;
                        bottomPos = obj.transform.position;
                    }
                }



                curIdx = DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[curIdx].nextRoomIdx;
            }

            var room2 = DungeonSystem.Instance.DungeonSystemData.dungeonRoomArray[curIdx];
            var obj2 = Instantiate(lastRoomPrefab, new Vector3(room2.Pos.x, 0f, room2.Pos.y)
                         , Quaternion.Euler(0f, 180f, 0f), transform);
            var objectInfo2 = obj2.GetComponent<RoomObject>();
            objectInfo2.roomIdx = room2.roomIdx;
            dungeonRoomObjectList.Add(obj2);

            minimapCam.leftVec = leftPos;
            minimapCam.rightVec = rightPos;
            minimapCam.topVec = topPos;
            minimapCam.bottomVec = bottomPos;
        }
      
    }
}
