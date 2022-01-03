using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// ÆÛ»çµå ´À³¦
public class DungeonSystem : MonoBehaviour
{
    public RoomCtrl[] roomPrefab;
    public GameObject eventObjectPrefab;
    public TextMeshProUGUI text;

    public PlayerDungeonUnit dungeonPlayer;
    public GatheringSystem GatheringSystem;

    DunGeonMapGenerate dungeonGenerator;
    RoomManager roomManager;


    private DungeonData dungeonSystemData;
    public DungeonData DungeonSystemData
    {
        get => dungeonSystemData;
    }

    void Start()
    {
        dungeonPlayer.gameObject.SetActive(false);
        for (int i = 0; i < roomPrefab.Length; i++)
        {
            roomPrefab[i].gameObject.SetActive(false);
        }
        if (Vars.UserData.curAllDungeonData != null)
        {
            dungeonSystemData = Vars.UserData.curAllDungeonData;
        }

        dungeonGenerator = gameObject.GetComponent<DunGeonMapGenerate>();
        dungeonGenerator.Init(this);

        roomManager = new RoomManager();
        roomManager.text = text;
        dungeonGenerator.DungeonGenerate(dungeonSystemData.dungeonRoomArray, RoomSetMethod);
    }

    private void RoomSetMethod()
    {
        roomManager.init(DungeonSystemData, this);
        dungeonPlayer.gameObject.SetActive(true);
        if (dungeonSystemData.curPlayerPosition == null)
        {
            dungeonPlayer.transform.position = roomPrefab[0].spawnPos.transform.position;
        }
        else
        {
            dungeonPlayer.transform.position = dungeonSystemData.curPlayerPosition;
        }

        for (int i = 0; i < dungeonSystemData.curEventObjList.Count; i++)
        {
            var eventObj = Instantiate(eventObjectPrefab, dungeonSystemData.curEventObjList[i].objectPosition, Quaternion.identity);
            eventObj.GetComponent<EventObject>().Init(dungeonSystemData.curEventObjList[i].roomInfo
                , dungeonSystemData.curEventObjList[i].eventType, this, dungeonSystemData.curEventObjList[i].objectPosition);
        }
    }

    public void ChangeRoomEvent(bool isRoomEnd)
    {
        if (isRoomEnd)
        {
            GatheringSystem.DeleteObj();
            dungeonSystemData.curRoomData.DestroyAllEventObject();
            dungeonSystemData.curEventObjList.Clear();

            roomManager.ChangeRoom(isRoomEnd);
            dungeonSystemData.curRoomData.CreateAllEventObject(dungeonSystemData.curSubRoomList, eventObjectPrefab);
            dungeonPlayer.transform.position = dungeonSystemData.curRoomData.spawnPos.transform.position;

            GatheringSystem.CreateGathering(DungeonSystemData.curDungeonData.RoomType,
          DungeonSystemData.curRoomData.evnetObjPos, DungeonSystemData.curSubRoomList);

        }
        else
        {
            roomManager.ChangeRoom(isRoomEnd);
        }
    }

    public void EventObjectClickEvent(DunGeonEvent eventType, EventObject eventObject)
    {
        dungeonSystemData.curPlayerPosition = dungeonPlayer.transform.position;

        switch (eventType)
        {
            case DunGeonEvent.Empty:
                break;
            case DunGeonEvent.Battle:
                break;
            case DunGeonEvent.Gathering:
                GatheringSystem.GoGatheringObject(eventObject.gameObject.transform.position);
                break;
            case DunGeonEvent.Hunt:
                Vars.UserData.curAllDungeonData = dungeonSystemData;
                dungeonPlayer.gameObject.SetActive(false);
                SceneManager.LoadScene("Hunting");
                break;
            case DunGeonEvent.RandomIncount:
                break;
            case DunGeonEvent.SubStory:
                break;
            case DunGeonEvent.Count:
                break;
        }
    }
}
