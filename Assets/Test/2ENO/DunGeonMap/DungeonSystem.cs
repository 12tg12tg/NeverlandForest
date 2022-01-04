using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// �ۻ�� ����
public class DungeonSystem : MonoBehaviour
{
    public RoomCtrl[] roomPrefab;
    public GameObject eventObjectPrefab;
    public TextMeshProUGUI text;

    public PlayerDungeonUnit dungeonPlayer;
    public GatheringSystem GatheringSystem;

    DunGeonMapGenerate dungeonGenerator;
    RoomManager roomManager;

    // ���� ����, �ҷ����⿡ �ʿ��� ��� �����͸� �̰����� ����!
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
    // �������� �ϼ��� �Ŀ� ������ ���� �� ����, �ݹ� �޼ҵ�� ����
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

    // �渶�� ��ġ���ִ� Ʈ���� �ߵ��Ҷ� ����
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
          DungeonSystemData.curRoomData.evnetObjPos, DungeonSystemData.curSubRoomList);

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
    // �̺�Ʈ ������Ʈ Ŭ����, ����
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
