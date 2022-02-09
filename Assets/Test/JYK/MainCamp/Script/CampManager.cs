using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CampManager : MonoBehaviour
{
    private static CampManager instance;
    public static CampManager Instance => instance;
    private bool isBlueMoon = false;
    private float recoveryBonTime = 0;
    private float recoverySleepTime = 0;
    private float gatheringTime = 0;
    private int isBlankCheck = 0;
    private int haveitemCount = 0;
    private Vector3 StartPos;
    private Vector3 EndPos;
    [SerializeField] private List<GatheringInCampRewardObject> gatheringRewardList = new List<GatheringInCampRewardObject>();
    private GameObject reward;

    private bool isCookMove;
    private bool isProduceMove;
    private bool isSleepMove;
    private bool isGatheringMove;
    [Header("미니맵 셋팅")]
    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public GameObject mapPos;
    public Camera campminimapCamera;
    public GameObject minimapPanel;
    private int curDungeonRoomIndex;
    public GameObject camera;

    int left, right, top, bottom;
    Vector3 leftPos = Vector3.zero;
    Vector3 rightPos = Vector3.zero;
    Vector3 topPos = Vector3.zero;
    Vector3 bottomPos = Vector3.zero;


    [Header("다이어리 셋팅")]
    public DiaryManager diaryManager;
    public GameObject diaryRecipePanel;
    public GameObject testPrehab;
    public GameObject newBottomUi;
    public GameObject CookPanel;
    public ReconfirmPanelManager reconfirmPanelManager;
    public ReconfirmPanelManager wtreconfirmPanelManager;

    public float RecoverySleepTime => recoverySleepTime;
    [Header("텍스트 관련")]
    public TextMeshProUGUI bonTimeText;
    public TextMeshProUGUI diarybonTimeText;
    public TextMeshProUGUI sleepTimeText;
    public TextMeshProUGUI gatheringTimeText;
    public TextMeshProUGUI gatheringRewardText;
    public TextMeshProUGUI cookingText;
    public TextMeshProUGUI producingText;

    [Header("캠프오브젝트")]
    public GameObject pot;
    public GameObject workshop;
    public GameObject tent;
    public GameObject bush;
    public GameObject bluemoonObject;
    public RecipeIcon diaryRecipeIcon;
    public GameObject campbornfire;

    [Header("요리제스처")]
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
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.ConsumableData);
        GameManager.Manager.State = GameState.Camp;
    }
    public void Start()
    {
        CampInit();

    }
   
    public void CampInit()
    {
        instance = this;
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        StartPos = camera.transform.position;

        CreateMiniMapObject();
        SetMinimapCamera();
        SetBonTime();
        SetSleepTime();
        SetGatheringTime();
    }
    //BonTime
    public void SetBonTime()
    {
        bonTimeText.text = Vars.UserData.uData.BonfireHour.ToString() + "시간";
        diarybonTimeText.text = Vars.UserData.uData.BonfireHour.ToString() + "시간";
    }
    public void RecoveryBonTime()
    {
        ConsumeManager.RecoveryBonFire(recoveryBonTime);
    }

    public void OnBonButton()
    {
        campbornfire.SetActive(true);
        bonTimeText.gameObject.SetActive(true);
    }
    public void OffBonButton()
    {
        campbornfire.SetActive(false);
        bonTimeText.gameObject.SetActive(false);
    }

    //Sleeping
    public void SetSleepTime()
    {
        sleepTimeText.text = recoverySleepTime.ToString() + "분";
    }
    public void PlusSleepTime()
    {
        recoverySleepTime += 30;
        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        if (haveMinute < recoverySleepTime)
        {
            recoverySleepTime = haveMinute;
        }
        SetSleepTime();
    }
    public void MinuseSleepTime()
    {
        recoverySleepTime -= 30;
        if (recoverySleepTime < 0)
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
        OffBonButton();
        var tentPos = tent.transform.position;
        EndPos = new Vector3(tentPos.x, tentPos.y + 2f, tentPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenSleepInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            GameManager.Manager.tm.mainTutorial.tutorialCamp.TutorialSleepingTouch = true;
        }
    }
    public void OpenSleepInCamp()
    {
        isSleepMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
        diaryManager.OpenSleeping();
        newBottomUi.SetActive(false);
    }
    public void CloseSleepInCamp()
    {
        if (isSleepMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f, OnBonButton));
            isSleepMove = false;
        }
        newBottomUi.SetActive(true);
    }

    //CookingInCamp
    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        diaryRecipeIcon.Init();
        StartCookingInCamp();
    }

    public void StartCookingInCamp()
    {
        OffBonButton();
        var potPos = pot.transform.position;
        EndPos = new Vector3(potPos.x + 1f, potPos.y + 2f, potPos.z - 3f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenCookInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            GameManager.Manager.tm.mainTutorial.tutorialCamp.TutorialCookingTouch = true;
        }
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
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f,OnBonButton));
            CloseRotationPanel();
            isCookMove = false;
        }
        newBottomUi.SetActive(true);
    }
    public void OpenCookInCamp()
    {
        isCookMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
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
        OffBonButton();
        var bushPos = bush.transform.position;
        EndPos = new Vector3(bushPos.x, bushPos.y + 2f, bushPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenGatheringInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            GameManager.Manager.tm.mainTutorial.tutorialCamp.TutorialGahteringingTouch = true;
        }
    }
    public void OpenGatheringInCamp()
    {
        isGatheringMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
        diaryManager.OpenGatheringInCamp();
        newBottomUi.SetActive(false);
    }
    public void CloseGatheringInCamp()
    {
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (gatheringRewardList[i].Item !=null)
            {
                haveitemCount++;
            }
        }
        if (haveitemCount == 0)
        {
            if (isGatheringMove)
            {
                StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f,OnBonButton));
                isGatheringMove = false;
            }
            for (int i = 0; i < gatheringRewardList.Count; i++)
            {
                gatheringRewardList[i].rewardIcon.sprite = null;
                gatheringRewardList[i].rewardIcon.color = Color.clear;
                gatheringRewardList[i].IsSelect = false;
                gatheringRewardList[i].Item = null;
            }
            isBlankCheck = 0;
            diaryManager.gatheringInCampRewardPanel.SetActive(false);
            diaryManager.gameObject.SetActive(false);
            newBottomUi.SetActive(true);
        }
        else
        {
            reconfirmPanelManager.gameObject.SetActive(true);
            reconfirmPanelManager.rewardNotEmptyPopup.SetActive(true);
            haveitemCount = 0;
        }
    }

    public void YesifinishGathering()
    {
        if (isGatheringMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f));
            isGatheringMove = false;
        }
        campbornfire.SetActive(true);
        bonTimeText.gameObject.SetActive(true);
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            gatheringRewardList[i].rewardIcon.sprite = null;
            gatheringRewardList[i].rewardIcon.color = Color.clear;
            gatheringRewardList[i].IsSelect = false;
            gatheringRewardList[i].Item = null;
        }

        haveitemCount = 0;
        isBlankCheck = 0;
        reconfirmPanelManager.gameObject.SetActive(false);
        diaryManager.gatheringInCampRewardPanel.SetActive(false);
        diaryManager.gameObject.SetActive(false);
        newBottomUi.SetActive(true);
    }
    public void NotYetGetItem()
    {
        reconfirmPanelManager.gameObject.SetActive(false);
    }

    //BlueMoonInCamp
    public void OpenBlueMoonScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Open BlueMoon Scene ");
    }

    //ProducingInCamp
    public void OpenMaking(object[] vals)
    {
        if (vals.Length != 0) return;
        StartProduceInCamp();
    }
    public void StartProduceInCamp()
    {
        OffBonButton();
        var workPos = workshop.transform.position;
        EndPos = new Vector3(workPos.x, workPos.y + 2f, workPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenProduceInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            GameManager.Manager.tm.mainTutorial.tutorialCamp.TutorialCraftTouch = true;
        }
    }
    public void OpenProduceInCamp()
    {
        isProduceMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
        diaryManager.OpenProduce();
        newBottomUi.SetActive(false);
    }

    public void MakeProduce()
    {
        diaryManager.CallMakeProduce();
    }
    public void CloseProduceInCamp()
    {
        if (isProduceMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f,OnBonButton));
            isProduceMove = false;
        }
        newBottomUi.SetActive(true);
    }
    public void ReProduce()
    {
        diaryManager.CloseProduceReward();
        reconfirmPanelManager.inventoryFullPopup.SetActive(false);
        reconfirmPanelManager.gameObject.SetActive(false);
        OpenProduceInCamp();
    }
    //SceneChange
    public void GoWorldMap()
    {
        SceneManager.LoadScene("AS_WorldMap");
    }
    public void GoDungeonCheck()
    {
        if (Vars.UserData.uData.BonfireHour != 0)
        {
            wtreconfirmPanelManager.gameObject.SetActive(true);
            wtreconfirmPanelManager.bonfireTimeRemainPopup.SetActive(true);
            bonTimeText.gameObject.SetActive(false);
        }
        else
        {
            GoDungeon();
        }
    }

    public void GoDungeon()
    {
        Vars.UserData.uData.BonfireHour = 0;
        SceneManager.LoadScene("AS_RandomMap");
    }
    public void NoIdonGO()
    {
        reconfirmPanelManager.gameObject.SetActive(false);
        bonTimeText.gameObject.SetActive(true);

    }

    //MinimapCreate
    public void CreateMiniMapObject()
    {
        curDungeonRoomIndex = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData.roomIdx;
        var array = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray;

        int curIdx = Vars.UserData.dungeonStartIdx;
        var curRoomIndex = curDungeonRoomIndex;

        left = curIdx % 20;
        right = curIdx % 20;
        top = curIdx / 20;
        bottom = curIdx / 20;
        RoomObject obj;
        while (array[curIdx].nextRoomIdx != -1)
        {
            var room = array[curIdx];
            if (room.RoomType == DunGeonRoomType.MainRoom)
            {
                //var mainRoomPrefab = DungeonSystem.
                obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, 0f, room.Pos.y)
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
                obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, 0f, room.Pos.y)
                , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                if (room.roomIdx == curRoomIndex)
                {
                    var mesh = obj.gameObject.GetComponent<MeshRenderer>();
                    mesh.material.color = Color.blue;
                }
            }
            if (curIdx == Vars.UserData.dungeonStartIdx)
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
        //campminimapCamera.transform.position = new Vector3((leftPos.x + rightPos.x) / 2, 150f, ((topPos.z + bottomPos.z) / 2) - 5f);
        //new Vector3(x, mapPos.transform.position.y + 10f, -47f);
    }

    public void SetGatheringTime()
    {
        gatheringTimeText.text = gatheringTime.ToString() + "분";
    }
    public void PlusGatheringTime()
    {
        gatheringTime += 30;
        if (gatheringTime > 180)
        {
            gatheringTime = 180;
        }
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
    public void GatheringInCamp()
    {
        gatheringRewardText.text = "탐색 성공!";

        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        haveMinute -= gatheringTime;
        Vars.UserData.uData.BonfireHour = haveMinute / 60;
        diaryManager.OpenGatheringReward();

        Debug.Log($"gatheringTime{gatheringTime}");
        for (int i = 0; i < (int)(gatheringTime / 30); i++)
        {
            gatheringRewardList[i].SetRewardItemIcon();
            if (gatheringRewardList[i].IsBlank == true)
            {
                isBlankCheck++;
            }
        }
        ConsumeManager.TimeUp(gatheringTime);
        if (isBlankCheck == (int)(gatheringTime / 30))
        {
            Debug.Log("전부꽝");
            gatheringRewardText.text = "딱히 주울게 없네";
        }
        gatheringTime = 0;
        SetGatheringTime();
        SetBonTime();
    }

    public void GetItem()
    {
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (gatheringRewardList[i].IsSelect == true)
            {
                if (Vars.UserData.AddItemData((gatheringRewardList[i].Item)) != false)
                {
                    Vars.UserData.AddItemData(gatheringRewardList[i].Item);
                    Vars.UserData.ExperienceListAdd(gatheringRewardList[i].Item.itemId);
                    gatheringRewardList[i].Item = null;
                    gatheringRewardList[i].rewardIcon.sprite = null;
                    gatheringRewardList[i].rewardIcon.color = Color.clear;
                    gatheringRewardList[i].IsSelect = false;
                }
            }
        }
        diaryManager.gatheringInventory.ItemButtonInit();
        if (BottomUIManager.Instance != null)
        {
            BottomUIManager.Instance.ItemListInit();
        }
        if (DiaryInventory.Instance != null)
        {
            DiaryInventory.Instance.ItemButtonInit();
        }
    }
    public void AllItem()
    {
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (Vars.UserData.AddItemData(gatheringRewardList[i].Item) != false)
            {
                Vars.UserData.AddItemData(gatheringRewardList[i].Item);
                Vars.UserData.ExperienceListAdd(gatheringRewardList[i].Item.itemId);
            }
            gatheringRewardList[i].Item = null;
            gatheringRewardList[i].rewardIcon.sprite = null;
            gatheringRewardList[i].rewardIcon.color = Color.clear;
            gatheringRewardList[i].IsSelect = false;
        }
        if (BottomUIManager.Instance != null)
        {
            BottomUIManager.Instance.ItemListInit();
        }
        if (DiaryInventory.Instance != null)
        {
            DiaryInventory.Instance.ItemButtonInit();
        }
    }

    public void BurnItemCheck()
    {
        var bottomui = newBottomUi.GetComponent<BottomUIManager>();
        for (int i = 0; i < bottomui.itemButtons.Count; i++)
        {
            if (bottomui.itemButtons[i].DataItem != null &&
                bottomui.itemButtons[i].DataItem.ItemTableElem.isBurn == true)
            {
                bottomui.itemButtons[i].IsBurn = true;
            }
        }
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            GameManager.Manager.tm.mainTutorial.tutorialCamp.TutorialBonfirecheckButtonClick = true;
            GameManager.Manager.tm.mainTutorial.tutorialCamp.IscookingFinish = true;

        }
    }
    public void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 75), "BonFireUp"))
        {
            ConsumeManager.RecoveryBonFire(0, 1);
            SetBonTime();
        }
        if (GUI.Button(new Rect(100, 200, 100, 75), "cost reset"))
        {
            ConsumeManager.CostDataReset();
        }
    }
    public void QuickButtonClick()
    {
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            GameManager.Manager.tm.mainTutorial.tutorialCamp.IsquitbuttonClick = true;
        }
    }
}
