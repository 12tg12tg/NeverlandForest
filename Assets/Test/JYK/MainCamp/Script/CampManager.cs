using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CampManager : MonoBehaviour
{
    private static CampManager instance;
    public static CampManager Instance => instance;

    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public GameObject mapPos;

    public Camera campminimapCamera;
    public GameObject minimapPanel;
    public GameObject diaryRecipePanel;

    private int curDungeonRoomIndex;

    private bool isBlueMoon = false;
    public GameObject bluemoonObject;

    public TextMeshProUGUI days;
    //Camp BonTime
    public TextMeshProUGUI bonTime;
    public TextMeshProUGUI bonTimeRecovery;
    public TextMeshProUGUI bonFireTime;
    public GameObject bonTimePanel;
    public GameObject bonFireStatePanel;
    private float recoveryBonTime = 0;

    //Camp Sleeping
    public TextMeshProUGUI sleepTimeRecovery;
    public GameObject sleepTimePanel;
    private float recoverySleepTime = 0;

    //CampGathering
    private float gatheringTime = 0;
    public TextMeshProUGUI gatheringText;
    public GameObject gatehringPanel;
    public GameObject gatehringReWardPanel;
    public Image Reward0;
    public Image Reward1;
    public Image Reward2;
    public GameObject RewardParents;
    private AllItemTableElem item;
    public Button rewardbutton0;
    public Button rewardbutton1;
    public Button rewardbutton2;

    //CampCook
    Vector3 StartPos;
    Vector3 EndPos;
    public GameObject camera;
    public GameObject pot;

    public GameObject CookPanel;
    public float RecoverySleepTime => recoverySleepTime;

    public InventoryController inventoryController;
    private DataAllItem itemReward;

    public enum CampEvent
    {
        StartCook,
        StartGathering,
        StartSleep,
        StartBlueMoon,
        StartMaking,
    }
    public void OnEnable()
    {
        EventBus<CampEvent>.Subscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.Subscribe(CampEvent.StartBlueMoon, OpenBlueMoonScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartMaking, OpenMaking);
    }
    public void Awake()
    {
        instance = this;
        StartPos = camera.transform.position;
    }
    private void OnDisable()
    {
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartBlueMoon, OpenBlueMoonScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartMaking, OpenMaking);

        EventBus<CampEvent>.ResetEventBus();
    }
    public void ChangeDay()
    {
        days.text = Vars.UserData.uData.Date.ToString() + "¿œ";
    }
    public void ChangeBonTime()
    {
        bonTime.text = Vars.UserData.uData.BonfireHour.ToString() + "Ω√∞£";
        bonFireTime.text = bonTime.text;
    }

    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        // SceneManager.LoadScene("Wt_Scene");
        StartCookingInCamp();
    }

    public void StartCookingInCamp()
    {
        var potPos = pot.transform.position;
        EndPos = new Vector3(potPos.x+1f, potPos.y + 2f, potPos.z - 3f);
        StartCoroutine(Utility.CoTranslate(camera.transform,StartPos, EndPos, 3f, OpenCookInCamp));
    }
    public void CloseCookingCamp()
    {
        StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
    }
    public void OpenCookInCamp()
    {
        CookPanel.gameObject.SetActive(true);
        bonFireStatePanel.gameObject.SetActive(true);
    }
    public void OpenGatheringScene(object[] vals)
    {
        if (vals.Length != 0) return;
        OpenCampGathering();
    }
    public void OpenBlueMoonScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Open BlueMoon Scene ");
    }
    public  void OpenMaking(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Open OpenMaking ");
    }
    public void StartSleep(object[] vals)
    {
        if (vals.Length != 0) return;
        OpenSleepTimePlus();
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

    public void Start()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        CreateMiniMapObject();
        var first = mapPos.transform.GetChild(0);
        var count = mapPos.transform.childCount;
        var lastIdx = count - 1;
        var last = mapPos.transform.GetChild(lastIdx);

        var x = (first.position.x + last.position.x) / 2;

        campminimapCamera.transform.position = new Vector3(x, mapPos.transform.position.y + 10f, -47f);
        ChangeDay();
        ChangeBonTime();
        ChangeBonTimeText();
        ChangeGatheringTimeText();
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

    public void OnOffBluemoonObject()
    {
        isBlueMoon = !isBlueMoon;
        if (isBlueMoon)
        {
            bluemoonObject.SetActive(true);
        }
        else
        {
            bluemoonObject.SetActive(false);
        }
    }
    public void OpenBonTimePlus()
    {
        bonTimePanel.SetActive(true);
        recoveryBonTime = 0f;
        bonTimeRecovery.text = recoveryBonTime.ToString() + "∫–";
    }
    public void CloseBonPanel()
    {
        bonTimePanel.SetActive(false);
    }
    public void ChangeBonTimeText()
    {
        bonTimeRecovery.text = recoveryBonTime.ToString() + "∫–";
    }
    public void PlusRecoveryTime()
    {
        recoveryBonTime += 30;
        ChangeBonTimeText();
    }
    public void MinusRecoveryTime()
    {
        recoveryBonTime -= 30;
        if (recoveryBonTime < 0)
        {
            recoveryBonTime = 0;
        }
        ChangeBonTimeText();

    }
    public void RecoveryBonTime()
    {
        ConsumeManager.RecoveryBonFire(recoveryBonTime);
        bonTimePanel.SetActive(false);
    }
    public void OpenSleepTimePlus()
    {
        sleepTimePanel.SetActive(true);
        bonFireStatePanel.SetActive(true);
        recoverySleepTime = 0f;
        ChangeSleepTimeText();
    }
    public void ChangeSleepTimeText()
    {
        sleepTimeRecovery.text = recoverySleepTime.ToString() + "∫–";
    }
    public void ChangeGatheringTimeText()
    {
        gatheringText.text = gatheringTime.ToString() + "∫–";
    }
    public void PlusSleepTime()
    {
        recoverySleepTime += 30;
        ChangeSleepTimeText();
    }
    public void MinusSleepTime()
    {
        recoverySleepTime -= 30;
        if (recoverySleepTime < 0)
        {
            recoverySleepTime = 0;
        }
        ChangeSleepTimeText();
    }
    public void Recovery_SleepTime()
    {
        sleepTimePanel.SetActive(false);
        if (Vars.UserData.uData.BonfireHour * 60 >= recoverySleepTime)
        {
            ConsumeManager.RecoveryTiredness();
            ConsumeManager.ConsumeBonfireTime(recoverySleepTime);
        }
        else
        {
            Debug.Log("∏¥⁄∫“Ω√∞£¿Ã ∫Œ¡∑«’¥œ¥Ÿ");
        }
        ChangeBonTimeText();
    }

    public void OpenCampGathering()
    {
        gatehringPanel.SetActive(true);
        gatheringText.text = gatheringTime.ToString() + "∫–";

    }

    public void PlusGatheringTime()
    {
        gatheringTime += 30;
        if (gatheringTime > 90)
        {
            gatheringTime = 90;
        }
        ChangeGatheringTimeText();
    }
    public void MinusGatheringTime()
    {
        gatheringTime -= 30;
        if (recoverySleepTime < 0)
        {
            recoverySleepTime = 0;
        }
        ChangeGatheringTimeText();
    }
    public void OkGatheringInCamp()
    {
        var rimitTime = Vars.UserData.uData.BonfireHour * 60;
        if (gatheringTime < rimitTime)
        {
            if (gatheringTime == 30)
            {
                Reward0.gameObject.SetActive(true);
                SetRewardItemIcon(rewardbutton0.image);
            }
            else if (gatheringTime == 60)
            {
                Reward0.gameObject.SetActive(true);
                Reward1.gameObject.SetActive(true);
                SetRewardItemIcon(rewardbutton0.image);
                SetRewardItemIcon(rewardbutton1.image);
            }
            else if (gatheringTime == 90)
            {
                Reward0.gameObject.SetActive(true);
                Reward1.gameObject.SetActive(true);
                Reward2.gameObject.SetActive(true);
                SetRewardItemIcon(rewardbutton0.image);
                SetRewardItemIcon(rewardbutton1.image);
                SetRewardItemIcon(rewardbutton2.image);
            }
        }
        gatehringPanel.SetActive(false);
        gatehringReWardPanel.SetActive(true);
    }
    public void SetRewardItemIcon(Image buttonimage)
    {
        //≥™π´≈‰∏∑: 1 %
        //æææ—: 1 %
        //≥™πµ∞°¡ˆ3 %
        //æ‡√ : 5 %
        // πˆº∏: 5 %
        //≤Œ: 85 %
        var randNum = Random.Range(1, 101);
      
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var newItem = new DataAllItem();
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{randNum}";
        newItem.itemId = randNum;
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);

        Debug.Log($"buttonimage{buttonimage.name}");
        Debug.Log($"randNum{randNum}");


        if (randNum==1)
        {
            //≥™π´≈‰∏∑: 1 %
        itemReward = newItem;
        }
        else if(randNum ==100)
        {
            //æææ—: 1 %
            itemReward = newItem;
        }
        else if(randNum >=2 &&randNum<= 4)
        {
            //≥™πµ∞°¡ˆ3 %
            itemReward = newItem;
        }
        else if(randNum >= 5 && randNum <= 9)
        {
            //æ‡√ : 5 %
            itemReward = newItem;
        }
        else if (randNum >=10 && randNum<= 14)
        {
            // πˆº∏: 5 %
            itemReward = newItem;
        }
        else
        {
            //≤Œ: 85 %
            Debug.Log("≤Œ");
        }
        if (item!=null)
        {
            item = newItem.ItemTableElem;
            buttonimage.sprite = item.IconSprite;
        }
    }

    public void OkGathering()
    {
        if (itemReward!=null)
        {
            Vars.UserData.AddItemData(itemReward);
            inventoryController.Init();
        }
    }

}
