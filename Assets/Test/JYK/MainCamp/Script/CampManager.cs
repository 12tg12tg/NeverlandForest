using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private int curDungeonRoomIndex;
    public int CurDungeonRoomIndex
    {
        get => curDungeonRoomIndex;
        set { curDungeonRoomIndex = value; }
    }
    public MinimapGenerate minimpaGenerate;


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
    [Header("캠프 튜토리얼")]
    public CampTutorial camptutorial;

    public enum CampinitState { None, Tutorial }
    public static CampinitState curinitState;

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
        GameManager.Manager.State = GameState.Camp;

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

    public void Start()
    {
        if (GameManager.Manager.Production!=null)
        {
            GameManager.Manager.Production.FadeOut();
        }
        switch (curinitState)
        {
            case CampinitState.None:
                CampInit();
                camptutorial.gameObject.SetActive(false);
                camptutorial.blackPanel.SetActive(false);
                curDungeonRoomIndex = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData.roomIdx;
                minimpaGenerate.CreateMiniMapObject();
                minimpaGenerate.minimapCam.SetMinimapObjectInCamp();
                break;
            case CampinitState.Tutorial:
                CampInit();
                // 스토리 챕터 4

                camptutorial.blackPanel.SetActive(true);
                GameManager.Manager.State = GameState.Tutorial;
                StartCoroutine(camptutorial.CoCampTutorial());
                break;
            default:
                break;
        }
        SoundManager.Instance.Play(SoundType.BG_Camp);

    }

    public void CampInit()
    {
        instance = this;
        StartPos = camera.transform.position;
        Vars.UserData.uData.BonfireHour = 3;
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
        ConsumeManager.SaveConsumableData();
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
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        ConsumeManager.SaveConsumableData();
        recoverySleepTime = 0;
        SetSleepTime();
        SetBonTime();
    }
    public void StartSleep(object[] vals)
    {
        if (vals.Length != 0) return;
        StartSleepInCamp();
        SoundManager.Instance.Play(SoundType.Se_Button);

    }
    public void StartSleepInCamp()
    {
        OffBonButton();
        var tentPos = tent.transform.position;
        EndPos = new Vector3(tentPos.x, tentPos.y + 2f, tentPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenSleepInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            camptutorial.TutorialSleepingTouch = true;
        }
    }
    public void OpenSleepInCamp()
    {
        isSleepMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
        diaryManager.OpenSleeping();
        diaryManager.curdiaryType = DiaryType.Sleep;

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
        SoundManager.Instance.Play(SoundType.Se_Button);

    }

    public void StartCookingInCamp()
    {
        OffBonButton();
        var potPos = pot.transform.position;
        EndPos = new Vector3(potPos.x + 1f, potPos.y + 2f, potPos.z - 3f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenCookInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            camptutorial.TutorialCookingTouch = true;
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
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f, OnBonButton));
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
        diaryManager.curdiaryType = DiaryType.Cook;
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
            if (diaryManager.recipeIcon.Isfireok &&
                diaryManager.recipeIcon.Iscondimentok&&
                diaryManager.recipeIcon.Ismaterialok)
            {
                diaryManager.CallMakeCook();
            }
        }
    }
    public void ReCook()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        OpenCookInCamp();
        diaryManager.CloseCookingReward();
    }
    public void CallMakeCook()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        diaryManager.CallMakeCook();
    }

    //GatheringInCamp
    public void OpenGatheringScene(object[] vals)
    {
        if (vals.Length != 0) return;
        StartGatheringInCamp();
        SoundManager.Instance.Play(SoundType.Se_Button);

    }
    public void StartGatheringInCamp()
    {
        OffBonButton();
        var bushPos = bush.transform.position;
        EndPos = new Vector3(bushPos.x, bushPos.y + 2f, bushPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenGatheringInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            camptutorial.TutorialGahteringingTouch = true;
        }
    }
    public void OpenGatheringInCamp()
    {
        isGatheringMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
        diaryManager.OpenGatheringInCamp();
        diaryManager.curdiaryType = DiaryType.GatheringInCamp;

        newBottomUi.SetActive(false);
    }
    public void CloseGatheringInCamp()
    {
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (gatheringRewardList[i].Item != null)
            {
                haveitemCount++;
            }
        }
        if (haveitemCount == 0)
        {
            if (isGatheringMove)
            {
                StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f, OnBonButton));
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
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        SoundManager.Instance.Play(SoundType.Se_Button);

        reconfirmPanelManager.gameObject.SetActive(false);
    }
    //BlueMoonInCamp
    public void OpenBlueMoonScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Open BlueMoon Scene ");
        SoundManager.Instance?.Play(SoundType.Se_Button);

    }
    //ProducingInCamp
    public void OpenMaking(object[] vals)
    {
        if (vals.Length != 0) return;
        StartProduceInCamp();
        SoundManager.Instance.Play(SoundType.Se_Button);

    }
    public void StartProduceInCamp()
    {
        OffBonButton();
        var workPos = workshop.transform.position;
        EndPos = new Vector3(workPos.x, workPos.y + 2f, workPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 1.5f, OpenProduceInCamp));
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            camptutorial.TutorialCraftTouch = true;
        }
    }
    public void OpenProduceInCamp()
    {
        isProduceMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.campBonfire.SetActive(false);
        diaryManager.OpenProduce();
        diaryManager.curdiaryType = DiaryType.Craft;

        newBottomUi.SetActive(false);
    }

    public void MakeProduce()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        diaryManager.CallMakeProduce();
    }
    public void CloseProduceInCamp()
    {
        if (isProduceMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 1.5f, OnBonButton));
            isProduceMove = false;
        }
        newBottomUi.SetActive(true);
    }
    public void ReProduce()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        diaryManager.CloseProduceReward();
        reconfirmPanelManager.inventoryFullPopup.SetActive(false);
        reconfirmPanelManager.gameObject.SetActive(false);
        OpenProduceInCamp();
    }
    //SceneChange
    public void GoDungeonCheck()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        if (Vars.UserData.uData.BonfireHour != 0)
        {
            wtreconfirmPanelManager.gameObject.SetActive(true);
            wtreconfirmPanelManager.inventoryFull_NotToProducePopup.SetActive(false);
            wtreconfirmPanelManager.bonfireTimeRemainPopup.SetActive(true);
            bonTimeText.gameObject.SetActive(false);
        }
        else
        {
            Vars.UserData.uData.BonfireHour = 0;
            ConsumeManager.SaveConsumableData();
            GoDungeon();
        }
    }

    public void GoDungeon()
    {
        Vars.UserData.trapPos.Clear(); // 블루문 저장 정보 증발
        Vars.UserData.trapType.Clear();

        SoundManager.Instance.Play(SoundType.Se_Button);
        GameManager.Manager.LoadScene(GameScene.Dungeon);
    }
    public void NoIdonGO()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        reconfirmPanelManager.gameObject.SetActive(false);
        bonTimeText.gameObject.SetActive(true);

    }
    public void SetGatheringTime()
    {
        gatheringTimeText.text = gatheringTime.ToString() + "분";
    }
    public void PlusGatheringTime()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        ConsumeManager.SaveConsumableData();

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
            if (gatheringRewardList[i].Item == null)
                continue;

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
        SoundManager.Instance.Play(SoundType.Se_Button);

        var bottomui = newBottomUi.GetComponent<BottomUIManager>();
        for (int i = 0; i < bottomui.itemButtons.Count; i++)
        {
            if (bottomui.itemButtons[i].DataItem != null &&
                bottomui.itemButtons[i].DataItem.ItemTableElem.isBurn == false)
            {
                bottomui.itemButtons[i].DisableItem(true);
            }
        }
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            camptutorial.TutorialBonfirecheckButtonClick = true;
            camptutorial.IscookingFinish = true;

        }
    }
    public void QuickButtonClick()
    {
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            camptutorial.IsquitbuttonClick = true;
        }
        diaryManager.curdiaryType = DiaryType.None;
        SoundManager.Instance.Play(SoundType.Se_Button);

    }
    /* public void OnGUI()
   {
       if (GUI.Button(new Rect(100, 200, 100, 75), "cost reset"))
       {
           ConsumeManager.CostDataReset();
       }
   }*/
}
