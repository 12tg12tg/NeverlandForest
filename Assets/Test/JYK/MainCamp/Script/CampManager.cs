using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CampManager : MonoBehaviour
{
    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public GameObject mapPos;

    public Camera campminimapCamera;
    public GameObject minimapPanel;
    public GameObject diaryRecipePanel;
    private bool isminimap = false;
    private bool isdiary = false;
    private int curDungeonRoomIndex;
    public enum CampEvent
    {
        StartCook,
        StartGathering,
        StartSleep,
    }
    public void OnEnable()
    {
        EventBus<CampEvent>.Subscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartSleep, StartSleep);
        
    }
    private void OnDisable()
    {
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.ResetEventBus();
    }
    public void Update()
    {
        ChangeminimapCameraState();
    }
    public void ChangeminimapCameraState()
    {
        if (isminimap)
            minimapPanel.SetActive(true);
        else
            minimapPanel.SetActive(false);
        if (isdiary)
            diaryRecipePanel.SetActive(true);
        else
            diaryRecipePanel.SetActive(false);
    }
    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Cooking Scene");
        SceneManager.LoadScene("Wt_Scene");
    }

    public void OpenGatheringScene(object[] vals)
    {
        if (vals.Length != 0) return;
        //SceneManager.LoadScene("WorldMap");
        Debug.Log($"Open OpenGathering Scene");
    }

    public void StartSleep(object[] vals)
    {
        if (vals.Length != 0) return;
        ConsumeManager.RecoveryTiredness();
        Debug.Log($"Go Sleep ");
    }
    public void OpenDiary()
    {
        isdiary = !isdiary;
    }
    public void OpenInventory()
    {
        Debug.Log($"Open Inventory window");
    }
    public void GoWorldMap()
    {
        SceneManager.LoadScene("AS_WorldMap");
    }
    public void GoDungeon()
    {
       SceneManager.LoadScene("AS_RandomMap");
    }
   
    public void OpenMiniMap()
    {
        isminimap = !isminimap;
    }
    public void Start()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        CreateMiniMapObject();
        var first = mapPos.transform.GetChild(0);
        var count = mapPos.transform.childCount;
        var lastIdx = count - 1;
        var last = mapPos.transform.GetChild(lastIdx);

        var x = (first.position.x + last.position.x) / 2;

        campminimapCamera.transform.position = new Vector3(x ,mapPos.transform.position.y+10f, -47f);

    }
    public void CreateMiniMapObject()
    {
        curDungeonRoomIndex = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData.roomIdx;
        var array = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray;

        int curIdx = Vars.UserData.dungeonStartIdx;
        var curRoomIndex = curDungeonRoomIndex;
        while (array[curIdx].nextRoomIdx != -1)
        {
            var room = array[curIdx];
            if (room.RoomType == DunGeonRoomType.MainRoom)
            {
                //var mainRoomPrefab = DungeonSystem.
                var obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
                     , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                if (room.roomIdx == curRoomIndex)
                {
                    var mesh = obj.gameObject.GetComponent<MeshRenderer>();
                    mesh.material.color = Color.blue;
                }
            }
            else
            {
                var obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
                , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                if (room.roomIdx == curRoomIndex)
                {
                    var mesh = obj.gameObject.GetComponent<MeshRenderer>();
                    mesh.material.color = Color.blue;
                }
            }
            curIdx = array[curIdx].nextRoomIdx;
        }
        var lastRoom = array[curIdx];
        var lastObj = Instantiate(mainRoomPrefab, new Vector3(lastRoom.Pos.x, lastRoom.Pos.y, 0f)
                     , Quaternion.identity, mapPos.transform);
        var objectInfo2 = lastObj.GetComponent<RoomObject>();
        objectInfo2.roomIdx = lastRoom.roomIdx;
        mapPos.transform.position = mapPos.transform.position + new Vector3(0f, 30f, 0f);
    }
}
