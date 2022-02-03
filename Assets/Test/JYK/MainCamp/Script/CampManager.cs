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
    private List<GameObject> isBlankCheckList = new List<GameObject>();
    private Vector3 StartPos;
    private Vector3 EndPos;
    private List<DataAllItem> rewardList = new List<DataAllItem>();
    private List<GameObject> rewardGameObjectList = new List<GameObject>();
    private List<GatheringInCampRewardObject> gatheringRewardList = new List<GatheringInCampRewardObject>();
    private GameObject reward;

    private bool isCookMove;
    private bool isProduceMove;
    private bool isSleepMove;
    private bool isGatheringMove;

    public List<DataAllItem> selectItemList = new List<DataAllItem>();

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

    public float RecoverySleepTime => recoverySleepTime;
    [Header("텍스트 관련")]
    public TextMeshProUGUI bonTimeText;
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
    public List<DataAllItem> RewardList
    {
        get => rewardList;
        set
        {
            rewardList = value;
        }
    }
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
    public void Start()
    {
        Init();
    }
    public void Init()
    {
        instance = this;
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        GameManager.Manager.State = GameState.Camp;
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
    }
    public void RecoveryBonTime()
    {
        ConsumeManager.RecoveryBonFire(recoveryBonTime);
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
        var potPos = pot.transform.position;
        EndPos = new Vector3(potPos.x + 1f, potPos.y + 2f, potPos.z - 3f);

        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenCookInCamp));
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

        if (rewardGameObjectList.Count > 0)
        {
            for (int i = rewardGameObjectList.Count - 1; i >= 0; i--)
            {
                Destroy(rewardGameObjectList[i]);
            }
        }
        if (rewardGameObjectList.Count == 0)
        {
            rewardGameObjectList.Clear();
        }
        diaryManager.gatheringInCampRewardPanel.SetActive(false);
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

    public void MakeProduce()
    {
        diaryManager.CallMakeProduce();
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
    public void ReProduce()
    {
        diaryManager.CloseProduceReward();
        reconfirmPanelManager.bagisFullReconfirm.gameObject.SetActive(false);
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
            reconfirmPanelManager.gameObject.SetActive(true);
            reconfirmPanelManager.OpenBonFireReconfirm();
            bonTimeText.gameObject.SetActive(false);
        }
    }

    public void GoDungeon()
    {
        SceneManager.LoadScene("AS_RandomMap");
    }
    public void NoIdonGO()
    {
        reconfirmPanelManager.gameObject.SetActive(false);
        reconfirmPanelManager.AllClose();
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
        campminimapCamera.transform.position = new Vector3((leftPos.x + rightPos.x) / 2, 150f, ((topPos.z + bottomPos.z) / 2) - 5f);
        //new Vector3(x, mapPos.transform.position.y + 10f, -47f);
    }

    public void SetGatheringTime()
    {
        gatheringTimeText.text = gatheringTime.ToString() + "분";
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
            reward = Instantiate(testPrehab, diaryManager.gatheringParent.transform);
            if (reward.GetComponent<GatheringInCampRewardObject>().IsBlank == true)
            {
                isBlankCheckList.Add(reward);
            }

            gatheringRewardList.Add(reward.GetComponent<GatheringInCampRewardObject>());
            rewardGameObjectList.Add(reward);
        }
        ConsumeManager.TimeUp(gatheringTime);
        if (rewardList.Count != 0)
        {
            for (int i = 0; i < rewardList.Count; i++)
            {
                Debug.Log($"itemReward{rewardList[i].ItemTableElem.name}");
            }
        }
        if (isBlankCheckList.Count == (int)(gatheringTime / 30))
        {
            Debug.Log("전부꽝");
            gatheringRewardText.text = "딱히 주울게 없네";
        }
        Debug.Log($"꽝 갯수 : {isBlankCheckList.Count}");
        Debug.Log($"보상갯수 : {rewardList.Count}");

        for (int i = isBlankCheckList.Count - 1; i >= 0; i--)
        {
            isBlankCheckList.RemoveAt(i);
        }
        isBlankCheckList.Clear();
        gatheringTime = 0;
        SetGatheringTime();
        SetBonTime();
    }

    public void GetItem()
    {
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            gatheringRewardList[i].IsSelect = false;
        }
        if (selectItemList.Count > 0)
        {
            for (int i = 0; i < selectItemList.Count; i++)
            {
                if (Vars.UserData.AddItemData(selectItemList[i]) != false)
                {
                    Vars.UserData.AddItemData(selectItemList[i]);
                    Vars.UserData.ExperienceListAdd(selectItemList[i].itemId);

                    for (int j = rewardList.Count-1; j>=0; j--)
                    {
                        if (rewardList[j] == selectItemList[i])
                        {
                            rewardList.RemoveAt(j);
                        }
                    }
                    if (rewardList.Count == 0)
                    {
                        rewardList.Clear();
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
            for (int i = 0; i < rewardGameObjectList.Count; i++)
            {
                for (int j = 0; j < selectItemList.Count; j++)
                {
                    if (selectItemList[j] == rewardGameObjectList[i].GetComponent<GatheringInCampRewardObject>().Item)
                    {
                        Destroy(rewardGameObjectList[i]);
                        rewardGameObjectList.RemoveAt(i);
                    }
                }
            }
            if (rewardGameObjectList.Count == 0)
            {
                rewardGameObjectList.Clear();
            }
            for (int i = selectItemList.Count - 1; i >= 0; i--)
            {
                selectItemList.RemoveAt(i);
            }
        }
    }
    public void AllItem()
    {
        if (rewardList.Count > 0)
        {
            for (int i = rewardList.Count - 1; i >= 0; i--)
            {
                Vars.UserData.AddItemData(rewardList[i]);
                Vars.UserData.ExperienceListAdd(rewardList[i].itemId);
                rewardList.RemoveAt(i);
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
        if (rewardList.Count == 0)
        {
            rewardList.Clear();

        }

        for (int i = rewardGameObjectList.Count - 1; i >= 0; i--)
        {
            Destroy(rewardGameObjectList[i]);
        }
        if (rewardGameObjectList.Count == 0)
        {
            rewardGameObjectList.Clear();
        }
        diaryManager.AllClose();
        diaryManager.gameObject.SetActive(false);
        newBottomUi.SetActive(true);
        CloseGatheringInCamp();
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

    }
}
