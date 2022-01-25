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
    public DiaryManager diaryManager;
    public GameObject newBottomUi;

    public Camera campminimapCamera;
    public GameObject minimapPanel;
    public GameObject diaryRecipePanel;

    private int curDungeonRoomIndex;

    private bool isBlueMoon = false;
    public GameObject bluemoonObject;

    private float recoveryBonTime = 0;
    private float recoverySleepTime = 0;
    private float gatheringTime = 0;
    public float RecoverySleepTime => recoverySleepTime;
    public TextMeshProUGUI bonTimeText;
    public TextMeshProUGUI sleepTimeText;
    public TextMeshProUGUI gatheringTimeText;
    //CampCook
    Vector3 StartPos;
    Vector3 EndPos;
    public GameObject camera;
    public GameObject pot;
    public GameObject workshop;
    public GameObject tent;
    public GameObject bush;

    public GameObject CookPanel;

    private DataAllItem itemReward;
    private bool isCookMove;
    private bool isProduceMove;
    private bool isSleepMove;
    private bool isGatheringMove;

    public SimpleGesture simpleGesture;

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
    private void OnDisable()
    {
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartBlueMoon, OpenBlueMoonScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartMaking, OpenMaking);

        EventBus<CampEvent>.ResetEventBus();
    }
    public void Awake()
    {
        instance = this;
        StartPos = camera.transform.position;
        SetBonTime();
        SetSleepTime();
        SetGatheringTime();
    }
    public void Start()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        CreateMiniMapObject();
        SetMinimapCamera();
        GameManager.Manager.State = GameState.Camp;
    }
  
    //BonTime
    public void SetBonTime()
    {
        bonTimeText.text = Vars.UserData.uData.BonfireHour.ToString() + "½Ã°£";
    }
    public void RecoveryBonTime()
    {
        ConsumeManager.RecoveryBonFire(recoveryBonTime);
    }

    //Sleeping
    public void SetSleepTime()
    {
        sleepTimeText.text = recoverySleepTime.ToString() + "ºÐ";
    }
    public void PlusSleepTime()
    {
        recoverySleepTime += 30;
        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        if (haveMinute< recoverySleepTime)
        {
            recoverySleepTime = haveMinute;
        }
        SetSleepTime();
    }
    public void MinuseSleepTime()
    {
        recoverySleepTime -= 30;
        if (recoverySleepTime<0)
        {
            recoverySleepTime = 0;
        }
        SetSleepTime();
    }
    public void IGoSleep()
    {
        ConsumeManager.RecoveryTiredness();
        recoverySleepTime = 0;
        SetSleepTime();
        SetBonTime();
    }
    public void StartSleep(object[] vals)
    {
        if (vals.Length != 0) return;
        StartSleepInCamp();
    }
    public void StartSleepInCamp()
    {
        var tentPos = tent.transform.position;
        EndPos = new Vector3(tentPos.x, tentPos.y + 2f, tentPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenSleepInCamp));
    }
    public void OpenSleepInCamp()
    {
        isSleepMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenSleeping();
        newBottomUi.SetActive(false);
    }
    public void CloseSleepInCamp()
    {
        if (isSleepMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            newBottomUi.SetActive(true);
            isSleepMove = false;
        }

    }

    //CookingInCamp
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
    public void RotateButtonCheck()
    {
        diaryManager.IsRotation = !diaryManager.IsRotation;
        diaryManager.ChangeRotateButtonImage();
        Debug.Log(diaryManager.IsRotation);
    }

    public void CloseRotationPanel()
    {
        diaryManager.CloseCookingRotation();
    }

    public void CloseCookingCamp()
    {
        if (isCookMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            CloseRotationPanel();
            newBottomUi.SetActive(true);

            isCookMove = false;
        }
    }
    public void OpenCookInCamp()
    {
        isCookMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenCooking();
        newBottomUi.SetActive(false);
    }
    public void StartCooking()
    {
        if (diaryManager.IsRotation)
        {
            simpleGesture.Init();
            diaryManager.OpenCookingRotation();
        }
        else
        {
            if (diaryManager.recipeIcon.fire.sprite != null && 
                diaryManager.recipeIcon.condiment.sprite != null &&
                diaryManager.recipeIcon.material.sprite != null)
            {
                diaryManager.CallMakeCook();
            }
        }
    }
    public void ReCook()
    {
        OpenCookInCamp();
        diaryManager.CloseCookingReward();
    }
    public void CallMakeCook()
    {
        diaryManager.CallMakeCook();
    }

    //GatheringInCamp
    public void OpenGatheringScene(object[] vals)
    {
        if (vals.Length != 0) return;
        StartGatheringInCamp();
    }
    public void StartGatheringInCamp()
    {
        var bushPos = bush.transform.position;
        EndPos = new Vector3(bushPos.x, bushPos.y + 2f, bushPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenGatheringInCamp));
    }
    public void OpenGatheringInCamp()
    {
        isGatheringMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenGatheringInCamp();
        newBottomUi.SetActive(false);
    }
    public void CloseGatheringInCamp()
    {
        if (isGatheringMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            newBottomUi.SetActive(true);

            isGatheringMove = false;
        }
    }

    //BlueMoonInCamp
    public void OpenBlueMoonScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Open BlueMoon Scene ");
    }

    //ProducingInCamp
    public  void OpenMaking(object[] vals)
    {
        if (vals.Length != 0) return;
        StartProduceInCamp();
    }
    public void StartProduceInCamp()
    {
        var workPos = workshop.transform.position;
        EndPos = new Vector3(workPos.x, workPos.y + 2f, workPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenProduceInCamp));
    }
    public void OpenProduceInCamp()
    {
        isProduceMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenProduce();
        newBottomUi.SetActive(false);
    }
    public void CloseProduceInCamp()
    {
        if (isProduceMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            newBottomUi.SetActive(true);

            isProduceMove = false;
        }
    }
    //SceneChange
    public void GoWorldMap()
    {
        SceneManager.LoadScene("AS_WorldMap");
    }
    public void GoDungeon()
    {
        SceneManager.LoadScene("AS_RandomMap");
    }
    //MinimapCreate
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
    public void SetMinimapCamera()
    {
        var first = mapPos.transform.GetChild(0);
        var count = mapPos.transform.childCount;
        var lastIdx = count - 1;
        var last = mapPos.transform.GetChild(lastIdx);

        var x = (first.position.x + last.position.x) / 2;

        campminimapCamera.transform.position = new Vector3(x, mapPos.transform.position.y + 10f, -47f);
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
 
    public void SetRewardItemIcon(Image buttonimage)
    {
        //³ª¹«Åä¸·: 1 %
        //¾¾¾Ñ: 1 %
        //³ª¹µ°¡Áö3 %
        //¾àÃÊ: 5 %
        // ¹ö¼¸: 5 %
        //²Î: 85 %
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
      
        var randNum = Random.Range(1, 30);
        var stringId = $"ITEM_{randNum}";
        var newItem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringId));
        newItem.OwnCount = Random.Range(1, 3);

        if (randNum==1)
        {
            //³ª¹«Åä¸·: 1 %
        itemReward = newItem;
        }
        else if(randNum ==100)
        {
            //¾¾¾Ñ: 1 %
            itemReward = newItem;
        }
        else if(randNum >=2 &&randNum<= 4)
        {
            //³ª¹µ°¡Áö3 %
            itemReward = newItem;
        }
        else if(randNum >= 5 && randNum <= 9)
        {
            //¾àÃÊ: 5 %
            itemReward = newItem;
        }
        else if (randNum >=10 && randNum<= 14)
        {
            // ¹ö¼¸: 5 %
            itemReward = newItem;
        }
        else
        {
            //²Î: 85 %
            Debug.Log("²Î");
        }
    }
    public void SetGatheringTime()
    {
        gatheringTimeText.text = gatheringTime.ToString() + "ºÐ";
    }
    public void PlusGatheringTime()
    {
        gatheringTime += 30;
        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        if (haveMinute < gatheringTime)
        {
            gatheringTime = haveMinute;
        }
        SetGatheringTime();
    }
    public void MinusGatheringTime()
    {
        gatheringTime -= 30;
        if (gatheringTime < 0)
        {
            gatheringTime = 0;
        }
        SetGatheringTime();
    }
}
