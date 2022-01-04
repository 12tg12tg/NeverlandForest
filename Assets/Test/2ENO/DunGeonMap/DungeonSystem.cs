using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// 퍼사드 느낌
public class DungeonSystem : MonoBehaviour
{
    public RoomCtrl[] roomPrefab;
    public GameObject eventObjectPrefab;
    public TextMeshProUGUI text;

    public PlayerDungeonUnit dungeonPlayer;
    public GatheringSystem GatheringSystem;

    DunGeonMapGenerate dungeonGenerator;
    RoomManager roomManager;

    // 던전 세팅, 불러오기에 필요한 모든 데이터를 이걸통해 관리!
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
    // 던전맵이 완성된 후에 정보를 토대로 방 세팅, 콜백 메소드로 실행
    private void RoomSetMethod()
    {
        dungeonPlayer.gameObject.SetActive(true);
        roomManager.init(DungeonSystemData, this);
        if (dungeonSystemData.curPlayerData == null)
        {
            dungeonPlayer.transform.position = roomPrefab[0].spawnPos.transform.position;
        }
        else
        {
            dungeonPlayer.SetPlayerData(dungeonSystemData.curPlayerData);
        }

        for (int i = 0; i < dungeonSystemData.curEventObjList.Count; i++)
        {
            var eventObj = Instantiate(eventObjectPrefab, dungeonSystemData.curEventObjList[i].objectPosition, Quaternion.identity);
            eventObj.GetComponent<EventObject>().Init(dungeonSystemData.curEventObjList[i].roomInfo
                , dungeonSystemData.curEventObjList[i].eventType, this, dungeonSystemData.curEventObjList[i].objectPosition);
            dungeonSystemData.curRoomData.eventObjList.Add(eventObj);
        }
    }

    // 방마다 위치해있는 트리거 발동할때 실행
    public void ChangeRoomEvent(bool isRoomEnd, bool isGoForward)
    {        
        //ConsumeManager.TimeUp(1,0);
        if (isRoomEnd)
        {
            GatheringSystem.DeleteObj();
            dungeonSystemData.curRoomData.DestroyAllEventObject();
            dungeonSystemData.curEventObjList.Clear();

            roomManager.ChangeRoomForward(isRoomEnd);
            dungeonSystemData.curRoomData.CreateAllEventObject(dungeonSystemData.curIncludeRoomList, eventObjectPrefab);
            dungeonPlayer.transform.position = dungeonSystemData.curRoomData.spawnPos.transform.position;

            GatheringSystem.CreateGathering(DungeonSystemData.curDungeonData.RoomType,
          DungeonSystemData.curRoomData.evnetObjPos, DungeonSystemData.curIncludeRoomList);

        }
        else
        {
            if(isGoForward)
            {
                roomManager.ChangeRoomForward(isRoomEnd);
            }
            else
            {
                roomManager.ChangeRoomGoBack();
            }
        }
    }
    // 이벤트 오브젝트 클릭시, 실행
    public void EventObjectClickEvent(DunGeonEvent eventType, EventObject eventObject)
    {
        dungeonSystemData.curPlayerData.SetUnitData(dungeonPlayer);

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
